using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GuiUtils
{
    public static Coordinates GetMouseCoordinates(CelestialBody body)
    {
        Ray mouseRay = PlanetariumCamera.Camera.ScreenPointToRay(Input.mousePosition);
        mouseRay.origin = ScaledSpace.ScaledToLocalSpace(mouseRay.origin);
        Vector3d relOrigin = mouseRay.origin - body.position;
        Vector3d relSurfacePosition;
        if (PQS.LineSphereIntersection(relOrigin, mouseRay.direction, body.Radius, out relSurfacePosition))
        {
            Vector3d surfacePoint = body.position + relSurfacePosition;
            return new Coordinates(body.GetLatitude(surfacePoint), MuUtils.ClampDegrees180(body.GetLongitude(surfacePoint)));
        }
        else
        {
            return null;
        }
    }

}

public class Coordinates
{
    public double latitude;
    public double longitude;

    public Coordinates(double latitude, double longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public static string ToStringDecimal(double latitude, double longitude, bool newline = false, int precision = 3)
    {
        double clampedLongitude = MuUtils.ClampDegrees180(longitude);
        double latitudeAbs  = Math.Abs(latitude);
        double longitudeAbs = Math.Abs(clampedLongitude);
        return latitudeAbs.ToString("F" + precision) + "° " + (latitude > 0 ? "N" : "S") + (newline ? "\n" : ", ")
            + longitudeAbs.ToString("F" + precision) + "° " + (clampedLongitude > 0 ? "E" : "W");
    }

    public string ToStringDecimal(bool newline = false, int precision = 3)
    {
        return ToStringDecimal(latitude, longitude, newline, precision);
    }
}
