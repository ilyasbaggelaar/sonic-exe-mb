using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoopZone))]
public class LoopZoneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LoopZone loopZone = (LoopZone)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Auto-Fill Rotations (Right Path)"))
        {
            AutoFillRotations(loopZone.loopPathRight);
        }

        if (GUILayout.Button("Auto-Fill Rotations (Left Path)"))
        {
            AutoFillRotations(loopZone.loopPathLeft);
        }

        EditorUtility.SetDirty(loopZone); // mark modified
    }

    private void AutoFillRotations(LoopPoint[] path)
    {
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector2 from = path[i].point.position;
            Vector2 to = path[i + 1].point.position;
            Vector2 direction = (to - from).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            path[i].zRotation = angle;
        }

        // Optional: set the final point’s rotation to match the previous
        if (path.Length >= 2)
        {
            path[^1].zRotation = path[^2].zRotation;
        }
    }
}
