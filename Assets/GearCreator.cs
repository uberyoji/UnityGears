using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 1.0, 4.0, 1.0, 20, 0.7 // red   -3.0, -2.0, 0.0
// 0.5, 2.0, 2.0, 10, 0.7 // green  3.1, -2.0, 0.0
// 1.3, 2.0, 0.5, 10, 0.7 // blue  -3.1, 4.2, 0.0

[ExecuteInEditMode]
public class GearCreator : MonoBehaviour
{
    public float InnerRadius = 1f;
    public float InnerToothDepth = 0.7f;
    public float OuterRadius = 4f;
    public float OuterToothDepth = 0.7f;
    public float Width = 4f;
    public int TeethCount = 20;
    
    private bool ForceUpdateMesh = false;

    void Start()
    {
//        ForceUpdateMesh = true;
    }

    void Update()
    {
        if(ForceUpdateMesh)
        {
            BuildMesh();
            ForceUpdateMesh = false;
        }
    }

    private void OnValidate()
    {
        Debug.Log("Something changed. Rebuilding mesh for "+gameObject.name+".");
        ForceUpdateMesh = true;
    }

    void BuildMesh()
    {
        int i = 0;
        CombineInstance[] combine = new CombineInstance[4];

        combine[i].mesh = GetSideSurface(true);
        combine[i++].transform = Matrix4x4.identity;

        combine[i].mesh = GetTeethSurface(true);
        combine[i++].transform = Matrix4x4.identity;

        combine[i].mesh = GetTeethSurface(false);
        combine[i++].transform = Matrix4x4.identity;

        combine[i].mesh = GetSideSurface(false);
        combine[i++].transform = Matrix4x4.identity;

        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);        
    }

    Vector3 GetVector( float r, float a, float w )
    {
         return new Vector3(r * Mathf.Cos(a), r * Mathf.Sin(a), w);
    }

    // Generate gear side face including teeth on outside and inside
    Mesh GetSideSurface(bool IsFront)
    {
        int i, k, bv, bi;
        float r0, r1, r2, r3;
        float angle, da;

        r0 = InnerRadius - InnerToothDepth / 2.0f;
        r1 = InnerRadius + InnerToothDepth / 2.0f;
        r2 = OuterRadius - OuterToothDepth / 2.0f;
        r3 = OuterRadius + OuterToothDepth / 2.0f;

        da = 2.0f * Mathf.PI / TeethCount / 4.0f;

        const int VertexCount = 10; // per teeth
        Vector3[] V = new Vector3[VertexCount * TeethCount];
        Vector3[] N = new Vector3[VertexCount * TeethCount];
        for (int n = 0; n < VertexCount * TeethCount; n++)
        {
            N[n] = new Vector3(0f, 0f, IsFront ? 1f : -1f);
        }

        const int FaceCount = 8;
        int[] I = new int[3 * FaceCount * TeethCount];

        int[,] F = new int[FaceCount, 3] { { 1, 0, 3 }, { 1, 3, 2 }, { 0, 5, 8 }, { 0, 8, 3 }, 
                                            { 5, 6, 7 },{ 5, 7, 8 },{ 3, 8, 9 },{ 3, 9, 4 } };

        float W = IsFront ? Width : -Width;

        for (i = 0; i < TeethCount; i++)
        {
            bv = i * VertexCount;
            bi = i * FaceCount * 3;

            angle = i * 2.0f * Mathf.PI / TeethCount;
            V[bv + 0] = GetVector(r1, angle + 0 * da, W * 0.5f);
            V[bv + 1] = GetVector(r0, angle + 1 * da, W * 0.5f);
            V[bv + 2] = GetVector(r0, angle + 2 * da, W * 0.5f);
            V[bv + 3] = GetVector(r1, angle + 3 * da, W * 0.5f);
            V[bv + 4] = GetVector(r1, angle + 4 * da, W * 0.5f);
            V[bv + 5] = GetVector(r2, angle + 0 * da, W * 0.5f);
            V[bv + 6] = GetVector(r3, angle + 1 * da, W * 0.5f);
            V[bv + 7] = GetVector(r3, angle + 2 * da, W * 0.5f);
            V[bv + 8] = GetVector(r2, angle + 3 * da, W * 0.5f);
            V[bv + 9] = GetVector(r2, angle + 4 * da, W * 0.5f);

            k = 0;
            for (int f = 0; f < FaceCount; f++)
            {
                I[bi + k++] = bv + F[f, 0];
                I[bi + k++] = bv + F[f, IsFront ? 1 : 2];
                I[bi + k++] = bv + F[f, IsFront ? 2 : 1];
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

    // Generate gear faces
    Mesh GetTeethSurface( bool IsOuter )
    {
        int i, k, bv, bi;
        float r1, r2;
        float angle, da;
        float u, v, len;

        if( IsOuter )
        {
            r1 = OuterRadius - OuterToothDepth / 2.0f;
            r2 = OuterRadius + OuterToothDepth / 2.0f;
        }
        else
        {
            r1 = InnerRadius + InnerToothDepth / 2.0f;
            r2 = InnerRadius - InnerToothDepth / 2.0f;
        }        

        da = 2.0f * Mathf.PI / TeethCount / 4.0f;

        const int VertexCount = 16; // per teeth
        Vector3[] V = new Vector3[VertexCount * TeethCount];
        Vector3[] N = new Vector3[VertexCount * TeethCount];

        const int FaceCount = 8;
        int[] I = new int[3 * FaceCount * TeethCount];

        int[,] F = new int[FaceCount, 3] { { 0, 2, 3 }, { 0, 3, 1 }, { 4, 6, 7 }, { 4, 7, 5 }, 
                                           { 8, 10, 11 }, { 8, 11, 9 }, { 12, 14, 15 }, { 12, 15, 13 } };
        
        for (i = 0; i < TeethCount; i++)
        {
            bv = i * VertexCount;
            bi = i * FaceCount * 3;
            angle = i * 2.0f * Mathf.PI / TeethCount;

            V[bv + 0] = GetVector(r1, angle + 0 * da, Width * -0.5f);
            V[bv + 1] = GetVector(r1, angle + 0 * da, Width * 0.5f);
            V[bv + 2] = GetVector(r2, angle + 1 * da, Width * -0.5f);
            V[bv + 3] = GetVector(r2, angle + 1 * da, Width * 0.5f);

            u = r2 * Mathf.Cos(angle + da) - r1 * Mathf.Cos(angle);
            v = r2 * Mathf.Sin(angle + da) - r1 * Mathf.Sin(angle);
            len = Mathf.Sqrt(u * u + v * v) * (IsOuter ? 1f : -1f);
            u /= len;
            v /= len;
            N[bv + 0] = N[bv + 1] = N[bv + 2] = N[bv + 3] = new Vector3(v,-u,0f);

            V[bv + 4] = GetVector(r2, angle + 1 * da, Width * -0.5f);
            V[bv + 5] = GetVector(r2, angle + 1 * da, Width * 0.5f);
            V[bv + 6] = GetVector(r2, angle + 2 * da, Width * -0.5f);
            V[bv + 7] = GetVector(r2, angle + 2 * da, Width * 0.5f);

            N[bv + 4] = N[bv + 5] = N[bv + 6] = N[bv + 7] 
                = new Vector3( (Mathf.Cos(angle+ 1*da)+Mathf.Cos(angle + 2 * da))/2f, (Mathf.Sin(angle + 1 * da) + Mathf.Sin(angle + 2 * da)) / 2f, 0f) * (IsOuter ? 1f : -1f);
            

            V[bv + 8] = GetVector(r2, angle + 2 * da, Width * -0.5f);
            V[bv + 9] = GetVector(r2, angle + 2 * da, Width * 0.5f);
            V[bv +10] = GetVector(r1, angle + 3 * da, Width * -0.5f);
            V[bv +11] = GetVector(r1, angle + 3 * da, Width * 0.5f);

            u = r1 * Mathf.Cos(angle + 3 * da) - r2 * Mathf.Cos(angle + 2 * da);
            v = r1 * Mathf.Sin(angle + 3 * da) - r2 * Mathf.Sin(angle + 2 * da);
            len = Mathf.Sqrt(u * u + v * v) * (IsOuter ? 1f : -1f);
            u /= len;
            v /= len;
            N[bv + 8] = N[bv + 9] = N[bv + 10] = N[bv + 11] = new Vector3(v, -u, 0f);

            V[bv +12] = GetVector(r1, angle + 3 * da, Width * -0.5f);
            V[bv +13] = GetVector(r1, angle + 3 * da, Width * 0.5f);
            V[bv +14] = GetVector(r1, angle + 4 * da, Width * -0.5f);
            V[bv +15] = GetVector(r1, angle + 4 * da, Width * 0.5f);

            N[bv + 12] = N[bv + 13] = N[bv + 14] = N[bv + 15]
                = new Vector3((Mathf.Cos(angle + 3 * da) + Mathf.Cos(angle + 4 * da)) / 2f, (Mathf.Sin(angle + 3 * da) + Mathf.Sin(angle + 4 * da)) / 2f, 0f) * (IsOuter ? 1f : -1f);

            // build indice buffer for tooth face
            k = 0;
            for (int f = 0; f < FaceCount; f++)
            {
                I[bi + k++] = bv + F[f, 0];
                I[bi + k++] = bv + F[f, IsOuter ? 1 : 2];
                I[bi + k++] = bv + F[f, IsOuter ? 2 : 1];
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



    /* Face diagram per teeth

       r2  1da .__. 2da
              /    \
             /      \
            /        \
     r1  a .          \.______.
          /           3da    / 4da
         /                  / 
        /                  / 
   r0  /                  /

   */

    // Generate gear face including outer teeth (no inner teeth)
    /*
    Mesh GetSideFace( bool IsFront )
    {
        int i,k,bv,bi;
        float r0, r1, r2;
        float angle, da;

        r0 = InnerRadius;
        r1 = OuterRadius - OuterToothDepth / 2.0f;
        r2 = OuterRadius + OuterToothDepth / 2.0f;

        da = 2.0f * Mathf.PI / TeethCount / 4.0f;

        const int VertexCount = 7; // per teeth
        Vector3[] V = new Vector3[VertexCount * TeethCount];
        Vector3[] N = new Vector3[VertexCount * TeethCount];
        for (int n = 0; n < VertexCount * TeethCount; n++)
        {
            N[n] = new Vector3(0f, 0f, IsFront ? 1f : -1f );
        }

        const int FaceCount = 5;
        int[] I = new int[3 * FaceCount * TeethCount];

        int[,] F = new int[FaceCount, 3] { { 6, 0, 1 }, { 1, 2, 3 }, { 1, 3, 4 }, { 6, 1, 4 }, { 6, 4, 5 } };

        float W = IsFront ? Width : -Width;
            
        for (i = 0; i < TeethCount; i++)
        {
            bv = i * VertexCount;
            bi = i * FaceCount * 3;

            angle = i * 2.0f * Mathf.PI / TeethCount;
            V[bv + 0] = GetVector(r0, angle + 0 * da, W * 0.5f);
            V[bv + 1] = GetVector(r1, angle + 0 * da, W * 0.5f);
            V[bv + 2] = GetVector(r2, angle + 1 * da, W * 0.5f);
            V[bv + 3] = GetVector(r2, angle + 2 * da, W * 0.5f);
            V[bv + 4] = GetVector(r1, angle + 3 * da, W * 0.5f);
            V[bv + 5] = GetVector(r1, angle + 4 * da, W * 0.5f);
            V[bv + 6] = GetVector(r0, angle + 4 * da, W * 0.5f);

            k = 0;
            for (int f = 0; f < FaceCount; f++)
            {
                I[bi + k++] = bv + F[f, 0];
                I[bi + k++] = bv + F[f, IsFront ? 1 : 2];
                I[bi + k++] = bv + F[f, IsFront ? 2 : 1];
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
    
    // Generate inner gear faces (cylinder)
    Mesh GetInnerCylinder()
    {
        int i, k, bv, bi;
        float r0, angle;

        r0 = InnerRadius;

        const int VertexCount = 4;  // vertex per teeth
        Vector3[] V = new Vector3[VertexCount * TeethCount];
        Vector3[] N = new Vector3[VertexCount * TeethCount];

        int[] I = new int[3 * VertexCount * TeethCount];

        const int FaceCount = 2;
        int[,] F = new int[FaceCount, 3] { { 0, 1, 2 }, { 0, 2, 3 } };

        for (i = 0; i < TeethCount; i++)
        {
            bv = i * VertexCount;
            bi = i * FaceCount * 3;

            // build vertex and normal buffer for tooth face
            angle = i * 2.0f * Mathf.PI / TeethCount;
            V[bv + 0] = GetVector(r0, angle, -Width * 0.5f);
            V[bv + 1] = GetVector(r0, angle,  Width * 0.5f);
            N[bv + 0] = new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle), 0f);
            N[bv + 1] = N[bv + 0];

            angle = (i + 1) * 2.0f * Mathf.PI / TeethCount;
            
            V[bv + 2] = GetVector(r0, angle,  Width * 0.5f);
            V[bv + 3] = GetVector(r0, angle, -Width * 0.5f);
            N[bv + 2] = new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle), 0f);
            N[bv + 3] = N[bv + 2];

            // build indice buffer for tooth face
            k = 0;
            for (int f = 0; f < FaceCount; f++)
            {
                I[bi + k++] = bv + F[f, 0];
                I[bi + k++] = bv + F[f, 1];
                I[bi + k++] = bv + F[f, 2];
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
    */
}
