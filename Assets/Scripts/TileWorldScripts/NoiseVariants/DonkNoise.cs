using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DonkNoise
{
    public static float donk(float x, int seed, float gap, float maxOffset)
    {
        float previousX = Mathf.Floor(x / gap) * gap;

        

        float nextX = previousX + gap;

        Random.InitState(seed + 100 * (int)previousX);
        float previousY = Random.Range(-maxOffset, maxOffset);

        Random.InitState(seed + 100 * (int)nextX);
        float nextY = Random.Range(-maxOffset, maxOffset);

        float y = Mathf.Lerp(previousY, nextY, (x - previousX) / (nextX - previousX));

        //Debug.Log("For x = " + x + "; PREV_X = " + previousX + "; PREV_Y = " + previousY + "; NEXT_X = " + nextX + "; NEXT_Y = " + nextY + "; Y = " + y + ";");

        return y;
    }
}
