using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticScript
{
    private static float _time;
    public static float Time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
        }
    }
}
