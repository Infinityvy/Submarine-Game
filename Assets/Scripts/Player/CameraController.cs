using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    private float distance = 20;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = target.position + Vector3.back * distance;
    }
}
