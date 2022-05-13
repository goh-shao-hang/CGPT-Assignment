using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] public Transform playerHead;

    // Update is called once per frame
    void Update()
    {
        transform.position = playerHead.transform.position;
    }
}
