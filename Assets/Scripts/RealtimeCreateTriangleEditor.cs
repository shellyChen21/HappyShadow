#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RealtimeCreateTriangle))]
public class RealtimeCreateTriangleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RealtimeCreateTriangle script = (RealtimeCreateTriangle)target;
        if (GUILayout.Button("Create Triangle"))
        {
            script.isCreateTriangle = true;
        }

        if (GUILayout.Button("Clear Mesh"))
        {
            script.clearMesh = true;
        }
    }
}
#endif