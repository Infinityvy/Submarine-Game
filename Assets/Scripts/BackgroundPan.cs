using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPan : MonoBehaviour
{
    public Material mat;
    public Transform playerPos;

    void Update()
    {
        mat.SetVector("_PlayerPos", playerPos.position);
    }
}
