using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraHandler : MonoBehaviour
{
    public Transform playerHead;
    public Camera mainCam;
    public GameObject weaponHolder;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Put camera at player's eye position
        transform.position = playerHead.transform.position;
        weaponHolder.transform.rotation = mainCam.transform.rotation;
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, new Vector3(xOffset, yOffset, mainCam.transform.localPosition.z), 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.localPosition = Vector3.zero;
    }
}
