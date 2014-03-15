using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GLUtils
{
    static Material _material;
    static Material material
    {
        get
        {
            if (_material == null) _material = new Material(Shader.Find("Particles/Additive"));
            return _material;
        }
    }

    public static void DrawMapViewGroundMarker(CelestialBody body, double latitude, double longitude, double radius, Color c, double rotation = 0)
    {
        Vector3d up = body.GetSurfaceNVector(latitude, longitude);
        Vector3d center = body.position + body.Radius * up;

        if (IsOccluded(center, body)) return;

        Vector3d north = Vector3d.Exclude(up, body.transform.up).normalized;

		//double radius = body.Radius / 15;

        GLTriangleMap(new Vector3d[]{
            center,
            center + radius * (QuaternionD.AngleAxis(rotation - 10, up) * north),
            center + radius * (QuaternionD.AngleAxis(rotation + 10, up) * north)
        }, c);

        GLTriangleMap(new Vector3d[]{
            center,
            center + radius * (QuaternionD.AngleAxis(rotation + 110, up) * north),
            center + radius * (QuaternionD.AngleAxis(rotation + 130, up) * north)
        }, c);

        GLTriangleMap(new Vector3d[]{
            center,
            center + radius * (QuaternionD.AngleAxis(rotation - 110, up) * north),
            center + radius * (QuaternionD.AngleAxis(rotation - 130, up) * north)
        }, c);
    }

    public static void GLTriangleMap(Vector3d[] worldVertices, Color c)
    {
        GL.PushMatrix();
        material.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.TRIANGLES);
        GL.Color(c);
        GLVertexMap(worldVertices[0]);
        GLVertexMap(worldVertices[1]);
        GLVertexMap(worldVertices[2]);
        GL.End();
        GL.PopMatrix();
    }

    public static void GLVertexMap(Vector3d worldPosition)
    {
        Vector3 screenPoint = PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition));
        GL.Vertex3(screenPoint.x / Camera.main.pixelWidth, screenPoint.y / Camera.main.pixelHeight, 0);
    }

    //Tests if byBody occludes worldPosition, from the perspective of the planetarium camera
    public static bool IsOccluded(Vector3d worldPosition, CelestialBody byBody)
    {
        if (Vector3d.Distance(worldPosition, byBody.position) < byBody.Radius - 100) return true;

        Vector3d camPos = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position);

        if (Vector3d.Angle(camPos - worldPosition, byBody.position - worldPosition) > 90) return false;

        double bodyDistance = Vector3d.Distance(camPos, byBody.position);
        double separationAngle = Vector3d.Angle(worldPosition - camPos, byBody.position - camPos);
        double altitude = bodyDistance * Math.Sin(Math.PI / 180 * separationAngle);
        return (altitude < byBody.Radius);
    }
}
