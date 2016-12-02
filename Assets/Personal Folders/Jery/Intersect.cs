using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersect {

    public static Vector3 RaycastMesh(Mesh mesh, Vector3 meshPos, Ray ray)
    {
        Plane plane;
        Vector3 p0, p1, p2, n, ti, intersect = Vector3.zero;
        float rayDistance, prevRayDistance = 3000f;

        List<Vector3> candidates = new List<Vector3>();
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            // load verts
            p0 = mesh.vertices[mesh.triangles[i * 3]] + meshPos;
            p1 = mesh.vertices[mesh.triangles[i * 3 + 1]] + meshPos;
            p2 = mesh.vertices[mesh.triangles[i * 3 + 2]] + meshPos;

            // calc plane via normal
            n = Vector3.Cross(p1 - p0, p2 - p0).normalized;
            plane = new Plane(n, p0);

            // raycast and in triangle test
            if (plane.Raycast(ray, out rayDistance))
            {
                ti = ray.GetPoint(rayDistance);
                if (PointInTriangle(ti, p0, p1, p2) && rayDistance < prevRayDistance)
                {
                    intersect = ray.GetPoint(rayDistance);
                    prevRayDistance = rayDistance;
                }
            }

        }

        return intersect;
    }


    static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
    {
        return Vector3.Dot(Vector3.Cross(b-a, p1-a), Vector3.Cross(b-a, p2-a)) >= 0;
    }

    static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        return SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b);
    }

}
