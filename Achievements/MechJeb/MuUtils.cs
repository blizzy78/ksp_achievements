using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

public static class MuUtils
{
    //keeps angles in the range 0 to 360
    public static double ClampDegrees360(double angle)
    {
        angle = angle % 360.0;
        if (angle < 0) return angle + 360.0;
        else return angle;
    }

    //keeps angles in the range -180 to 180
    public static double ClampDegrees180(double angle)
    {
        angle = ClampDegrees360(angle);
        if (angle > 180) angle -= 360;
        return angle;
    }
}
