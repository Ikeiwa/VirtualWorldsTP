using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVect3 {
    public int v1;
    public int v2;
    public int v3;

    public IntVect3(int v1, int v2, int v3) {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
}

public class SphereBuilder
{
    /// <summary>Generates and returns a new sphere mesh. Resolution multiplies the amount of triangles by n^4, don't go above 5. Using a resolution of 0 will return a regular isocaedron.</summary>
    public static Mesh make1t1Sphere(int resolution) {
        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        float radius = 1f;

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;
        vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);
        vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);
        // Hardcoded 20 isocaedron faces to refine later
        List<IntVect3> faces = new List<IntVect3>();
        faces.Add(new IntVect3(0, 11, 5));
        faces.Add(new IntVect3(0, 5, 1));
        faces.Add(new IntVect3(0, 1, 7));
        faces.Add(new IntVect3(0, 7, 10));
        faces.Add(new IntVect3(0, 10, 11));
        faces.Add(new IntVect3(1, 5, 9));
        faces.Add(new IntVect3(5, 11, 4));
        faces.Add(new IntVect3(11, 10, 2));
        faces.Add(new IntVect3(10, 7, 6));
        faces.Add(new IntVect3(7, 1, 8));
        faces.Add(new IntVect3(3, 9, 4));
        faces.Add(new IntVect3(3, 4, 2));
        faces.Add(new IntVect3(3, 2, 6));
        faces.Add(new IntVect3(3, 6, 8));
        faces.Add(new IntVect3(3, 8, 9));
        faces.Add(new IntVect3(4, 9, 5));
        faces.Add(new IntVect3(2, 4, 11));
        faces.Add(new IntVect3(6, 2, 10));
        faces.Add(new IntVect3(8, 6, 7));
        faces.Add(new IntVect3(9, 8, 1));


        // refine triangles recursively. This replaces 1 tri by a triforce like pattern and pushes the middle triangle slightly out.
        for (int i = 0; i < resolution; i++) {
            List<IntVect3> faces2 = new List<IntVect3>();
            foreach (var tri in faces) {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                faces2.Add(new IntVect3(tri.v1, a, c));
                faces2.Add(new IntVect3(tri.v2, b, a));
                faces2.Add(new IntVect3(tri.v3, c, b));
                faces2.Add(new IntVect3(a, b, c));
            }
            faces = faces2;
        }

        Mesh msh = new Mesh();
        msh.vertices = vertList.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++) {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        msh.triangles = triList.ToArray();
        return msh;
    }

    // return index of point in the middle of p1 and p2, given a radius to push out the point.
    // Found on stackoverflow, I'm not that smart.
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius) {
        long smallerIndex = p1 < p2 ? p1 : p2, greaterIndex = p1 < p2 ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;
        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3(
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f);
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);
        cache.Add(key, i);
        return i;
    }
}
