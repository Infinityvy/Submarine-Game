using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MySystem
{
    public static int truths(params bool[] booleans)
    {
        return booleans.Count(b => b);
    }
}
