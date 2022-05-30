using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    public Volume volume;

    public float currentWeight = 0f;
    private float defaultWeight = 0f;
    private float targetWeight = 0f;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.weight = defaultWeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (volume.weight != targetWeight)   
            LerpWeight();  
        currentWeight = volume.weight;
    }

    public void AddWeight(float target)
    {
        targetWeight = target;
    }

    public void RestoreWeight()
    {
        targetWeight = defaultWeight;
    }

    public void LerpWeight()
    {
        volume.weight = Mathf.Clamp01(Mathf.Lerp(volume.weight, targetWeight, 0.025f));
        if (volume.weight <= 0.0001f)
            volume.weight = 0f;
    }
}
