#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GameobjectFollower))]
public class GameobjectFollowerEditor : Editor
{ 

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameobjectFollower script = (GameobjectFollower)target;

        string buttonText = script.isFollowing ? "Stop Following" : "Start Following";
        if (GUILayout.Button(buttonText))
        {
            script.isFollowing = !script.isFollowing;
        }
    }
}
#endif
