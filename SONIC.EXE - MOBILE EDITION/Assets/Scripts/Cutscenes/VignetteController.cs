using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class VignetteHandler : MonoBehaviour
{
    public Volume globalVolume; // Drag the Global Volume here in Inspector
    private Vignette vignette;

    [Range(0f, 1f)] public float targetIntensity = 0.5f;

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogError("No Volume assigned.");
            return;
        }

        if (globalVolume.profile.TryGet(out vignette))
        {
            Debug.Log("✅ Vignette found.");
            vignette.intensity.overrideState = true;
            vignette.active = true;
        }
        else
        {
            Debug.LogError("❌ Vignette not found in volume profile.");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
    

        if (vignette != null)
        {
            vignette.intensity.value = targetIntensity;
        }
        }
        #endif
    }
}
