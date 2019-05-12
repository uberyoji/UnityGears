using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class NormalsVisualizer : Editor
{
    public float NormalScale = 0.25f;
    public bool Show = false;

    private Mesh mesh;

    void OnEnable()
    {
        MeshFilter mf = target as MeshFilter;
        if (mf != null)
        {
            mesh = mf.sharedMesh;
        }
    }

    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();

        if (Show = EditorGUILayout.Toggle("Show Normals", Show))
        {
            NormalScale = EditorGUILayout.Slider("Normal Scale", NormalScale, 0.1f, 4f);
        }
    }

    void OnSceneGUI()
    {
        if (mesh == null || !Show )
        {
            return;
        }

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
            Handles.color = Color.yellow;
            Handles.DrawLine(
                mesh.vertices[i],
                mesh.vertices[i] + mesh.normals[i] * NormalScale);
        }
    }
}