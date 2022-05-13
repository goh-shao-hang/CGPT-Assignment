using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform playerHead;
    public Transform mainCam;
    public GameObject weaponHolder;

    // Update is called once per frame
    void Update()
    {
        //Put camera at player's eye position
        transform.position = playerHead.transform.position;
        weaponHolder.transform.rotation = mainCam.rotation; 
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCam.transform.localPosition;
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, new Vector3(xOffset, yOffset, originalPos.z), 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }
}
