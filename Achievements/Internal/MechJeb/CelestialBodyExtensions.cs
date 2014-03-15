using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CelestialBodyExtensions
{
    public static double TerrainAltitude(this CelestialBody body, double latitude, double longitude)
    {
        if (body.pqsController == null) return 0;

        Vector3d pqsRadialVector = QuaternionD.AngleAxis(longitude, Vector3d.down) * QuaternionD.AngleAxis(latitude, Vector3d.forward) * Vector3d.right;
        double ret = body.pqsController.GetSurfaceHeight(pqsRadialVector) - body.pqsController.radius;
        if (ret < 0) ret = 0;
        return ret;
    }
}
