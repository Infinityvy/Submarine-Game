using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_Tester : MonoBehaviour
{
    public WorldLoader worldLoader;
    public WorldGenerator worldGenerator;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            worldLoader.regenerateChunks();
            Debug.Log("Regenerated chunks.");
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            worldGenerator.testNoise();
        }
    }
}
