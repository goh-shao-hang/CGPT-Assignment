using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    public Volume volume;
    LensDistortion lensDistortion;

    private float defaultLensDistortion = 0f;
    private float targetLensDistortIntensity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out lensDistortion); 
        lensDistortion.intensity.value = defaultLensDistortion;
    }

    // Update is called once per frame
    void Update()
    {
        if (lensDistortion.intensity.value != targetLensDistortIntensity)   
            LensDistort(targetLensDistortIntensity);
    }

    public void startLensDistort(float targetValue)
    {
        targetLensDistortIntensity = targetValue;
    }

    public void EndLensDistort()
    {
        targetLensDistortIntensity = defaultLensDistortion;
    }

    public void LensDistort(float targetValue)
    {
        lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, targetValue, 0.025f);
    }
}
