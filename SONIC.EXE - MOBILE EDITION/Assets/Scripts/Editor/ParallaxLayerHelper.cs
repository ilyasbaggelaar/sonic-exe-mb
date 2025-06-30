using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParallaxLayer))]
public class ParallaxLayerHelper : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ParallaxLayer layer = (ParallaxLayer)target;

        if (GUILayout.Button("🔍 Auto-Calculate Sprite Width"))
        {
            if (layer.transform.childCount < 2)
            {
                Debug.LogWarning("Need at least 2 child sprites to calculate spacing.");
                return;
            }

            float totalSpacing = 0f;
            int count = 0;
            float firstX = layer.transform.GetChild(0).position.x;

            for (int i = 1; i < layer.transform.childCount; i++)
            {
                float prevX = layer.transform.GetChild(i - 1).position.x;
                float currX = layer.transform.GetChild(i).position.x;
                float spacing = Mathf.Abs(currX - prevX);

                totalSpacing += spacing;
                count++;
            }

            float avgSpacing = totalSpacing / count;
            //layer.totalWidth = avgSpacing;

            Debug.Log($"📏 Sprite Width set to: {avgSpacing:F4} (based on spacing of {count} sprites)");
        }

        if (GUILayout.Button("🧪 Check Sprite Alignment"))
        {
            if (layer.transform.childCount < 2)
            {
                Debug.LogWarning("Need at least 2 sprites to check alignment.");
                return;
            }

            float expected = -1f;
            bool allAligned = true;

            for (int i = 1; i < layer.transform.childCount; i++)
            {
                float prev = layer.transform.GetChild(i - 1).position.x;
                float curr = layer.transform.GetChild(i).position.x;
                float spacing = Mathf.Abs(curr - prev);

                if (i == 1)
                {
                    expected = spacing;
                }
                else if (Mathf.Abs(spacing - expected) > 0.01f)
                {
                    Debug.LogWarning($"⚠️ Sprite {i} spacing off: {spacing:F4} (expected ~{expected:F4})");
                    allAligned = false;
                }
            }

            if (allAligned)
                Debug.Log("✅ All sprites appear evenly spaced.");
        }
    }
}
