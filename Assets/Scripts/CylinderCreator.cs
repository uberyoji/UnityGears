using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CylinderCreator : MonoBehaviour
{
    public float Radius = 1f;
    public float Height = 1f;
    public int SideSegmentCount = 16;
    public int HeightSegmentCount = 4;

    public bool TopCap = false;
    public bool BottomCap = false;

    public bool ForceUpdateMesh = false;

    public Bone[] Bones;

    void Start()
    {
        //        ForceUpdateMesh = true;
    }

    void Update()
    {
        if (ForceUpdateMesh)
        {
            BuildMesh();
            ForceUpdateMesh = false;
        }
    }

    private void OnValidate()
    {
        Debug.Log("Something changed. Rebuilding mesh for " + gameObject.name + ".");
        ForceUpdateMesh = true;
    }

    void BuildMesh()
    {
        int i = 0;
        CombineInstance[] combine = new CombineInstance[1 + (TopCap ? 1 : 0) + (BottomCap ? 1 : 0)];

        combine[i].mesh = GetSideSurface();
        combine[i++].transform = Matrix4x4.identity;

        if (TopCap)
        {
            combine[i].mesh = GetCapSurface(true);
            combine[i++].transform = Matrix4x4.identity;
        }

        if (BottomCap)
        {
            combine[i].mesh = GetCapSurface(false);
            combine[i++].transform = Matrix4x4.identity;
        }
        
        SkinnedMeshRenderer SMR = GetComponent<SkinnedMeshRenderer>();
        if(SMR)
        {
            // build bone and binpose arrays
            Transform[] bones = new Transform[Bones.Length];
            Matrix4x4[] bindPoses = new Matrix4x4[Bones.Length];
            for (i = 0; i < Bones.Length; i++)
            {
                bones[i] = Bones[i].transform;
                bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
            }

            // combine mesh first
            Mesh skinnedMesh = new Mesh();            
            skinnedMesh.CombineMeshes(combine);

            // then weights
            skinnedMesh.boneWeights = GetBoneWeightFromInfluenceSpheres(skinnedMesh.vertices);
            skinnedMesh.bindposes = bindPoses;
            
            // assign to renderer
            SMR.bones = bones;
            SMR.sharedMesh = skinnedMesh;
        }
        else
        {
            GetComponent<MeshFilter>().sharedMesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        }
    }

    Vector3 GetVector(float r, float a, float w)
    {
        return new Vector3(r * Mathf.Cos(a), w, r * Mathf.Sin(a));
    }

    Mesh GetCapSurface(bool IsTop)
    {
        float angle, da;

        da = 2.0f * Mathf.PI / SideSegmentCount;

        int VertexCount = SideSegmentCount + 1;
        Vector3[] V = new Vector3[VertexCount];
        Vector3[] N = new Vector3[VertexCount];

        Vector3 Normal = new Vector3(0f, IsTop ? 1f : -1f, 0f);
        Vector3 Center = new Vector3(0f, IsTop ? Height : 0f, 0f);

        int n = 0;

        V[n] = Center;
        N[n] = Normal;
        n++;
        angle = 0f;
        for (int s = 0; s < SideSegmentCount; s++)
        {
            V[n] = GetVector(Radius, angle, Center.y);
            N[n] = Normal;

            n++;
            angle += da;
        }

        int FaceCount = SideSegmentCount;
        int[] I = new int[3 * FaceCount];

        int k = 0;

        if (IsTop)
        {
            for (int s = 0; s < SideSegmentCount; s++)
            {
                I[k++] = 0;
                I[k++] = (s + 1) % SideSegmentCount + 1;
                I[k++] = s + 1;
            }
        }
        else
        {
            for (int s = 0; s < SideSegmentCount; s++)
            {
                I[k++] = 0;
                I[k++] = s + 1;
                I[k++] = (s + 1) % SideSegmentCount + 1;
            }
        }

        Mesh M = new Mesh
        {
            vertices = V,
            normals = N,
            triangles = I
        };
        return M;
    }

    BoneWeight[] GetBoneWeightFromInfluenceSpheres( Vector3[] V )
    {
        if( Bones.Length == 0 )
            return null;

        BoneWeight[] B = new BoneWeight[V.Length];

        float Distance = 0f;
        float Influence = 0f;

        for (int i=0;i<V.Length;i++)
        {
            Distance = (V[i] - Bones[0].transform.position).magnitude;
            B[i].boneIndex0 = 0;
            B[i].weight0 = Mathf.Max((Bones[0].InfluenceRadius - Distance) / Bones[0].InfluenceRadius, 0f);

            Distance = (V[i] - Bones[1].transform.position).magnitude;
            B[i].boneIndex1 = 1;
            B[i].weight1 = Mathf.Max((Bones[1].InfluenceRadius - Distance) / Bones[1].InfluenceRadius, 0f);

            Distance = (V[i] - Bones[2].transform.position).magnitude;
            B[i].boneIndex2 = 2;
            B[i].weight2 = Mathf.Max((Bones[2].InfluenceRadius - Distance) / Bones[2].InfluenceRadius, 0f);

            Distance = (V[i] - Bones[3].transform.position).magnitude;
            B[i].boneIndex3 = 3;
            B[i].weight3 = Mathf.Max((Bones[3].InfluenceRadius - Distance) / Bones[3].InfluenceRadius, 0f);

            Influence = B[i].weight0 + B[i].weight1 + B[i].weight2 + B[i].weight3;
            B[i].weight0 /= Influence;
            B[i].weight1 /= Influence;
            B[i].weight2 /= Influence;
            B[i].weight3 /= Influence;
        }

        // todo collect closest bones

        return B;
    }

    

    // Generate gear side face including teeth on outside and inside
    Mesh GetSideSurface()
    {
        float angle, da, dh;

        dh = Height / (HeightSegmentCount-1);
        da = 2.0f * Mathf.PI / SideSegmentCount;

        int VertexCount = SideSegmentCount * HeightSegmentCount;
        Vector3[] V = new Vector3[VertexCount];
        Vector3[] N = new Vector3[VertexCount];
        BoneWeight[] B = new BoneWeight[VertexCount];

        int n = 0;
        for (int h = 0; h < HeightSegmentCount; h++)
        {
            angle = 0f;
            for (int s = 0; s < SideSegmentCount; s++)
            {
                V[n] = GetVector(Radius, angle, h * dh);
                N[n] = GetVector(1f, angle, 0f);

                n++;
                angle += da;
            }
        }

        int FaceCount = 2 * SideSegmentCount * (HeightSegmentCount-1);
        int[] I = new int[3 * FaceCount];

        int bv, bi, ti, k=0;
        for (int h = 0; h < HeightSegmentCount-1; h++)
        {
            ti = (h+1) * SideSegmentCount;
            bi = h * SideSegmentCount;
            for (int s = 0; s < SideSegmentCount; s++)
            {
                bv = h * SideSegmentCount + s;
                
                if(s==SideSegmentCount-1)
                {
                    // first face
                    I[k++] = bv;
                    I[k++] = bv + SideSegmentCount;
                    I[k++] = bi;
                    // second face
                    I[k++] = bi;
                    I[k++] = bv + SideSegmentCount;
                    I[k++] = ti;
                }
                else
                {
                    // first face
                    I[k++] = bv;
                    I[k++] = bv + SideSegmentCount;
                    I[k++] = bv + 1;
                    // second face
                    I[k++] = bv + 1;
                    I[k++] = bv + SideSegmentCount;
                    I[k++] = bv + SideSegmentCount + 1;
                }
                
            }
        }

        Mesh M = new Mesh
        {
            vertices = V,
            normals = N,
            triangles = I
        };
        return M;
    }
}
