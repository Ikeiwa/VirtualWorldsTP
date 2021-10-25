using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindreBuilder {
    /// <summary>
    /// Fills this <see cref="Mesh"/> with vertices forming a 2D circle.
    /// Derived directly from https://github.com/GlitchEnzo/UnityProceduralPrimitives/blob/master/Assets/Procedural%20Primitives/Scripts/MeshExtensions.cs
    /// </summary>
    /// <param name="mesh">The <see cref="Mesh"/> to fill with vertices.</param>
    /// <param name="topRadius">Top radius of the cylinder. Value should be greater than or equal to 0.0f.</param>
    /// <param name="segments">The number of segments making up the circle. Value should be greater than or equal to 3.</param>
    /// <param name="startAngle">The starting angle of the circle.  Usually 0.</param>
    /// <param name="angularSize">The angular size of the circle.  2 pi is a full circle. Pi is a half circle.</param>
    public static Mesh CreateCylinder(float topRadius, float bottomRadius, float height,
        int radialSegments, int heightSegments, bool openEnded) {
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        var heightHalf = height / 2;
        int y;

        List<List<int>> verticesLists = new List<List<int>>();
        List<List<Vector2>> uvsLists = new List<List<Vector2>>();

        for (y = 0; y <= heightSegments; y++) {
            List<int> verticesRow = new List<int>();
            List<Vector2> uvsRow = new List<Vector2>();

            var v = y / (float)heightSegments;
            var radius = v * (bottomRadius - topRadius) + topRadius;

            for (int x = 0; x <= radialSegments; x++) {
                float u = (float)x / (float)radialSegments;

                var vertex = new Vector3();
                vertex.x = radius * Mathf.Sin(u * Mathf.PI * 2.0f);
                vertex.y = -v * height + heightHalf;
                vertex.z = radius * Mathf.Cos(u * Mathf.PI * 2.0f);

                vertices.Add(vertex);

                verticesRow.Add(vertices.Count - 1);
                uvsRow.Add(new Vector2(u, 1 - v));
            }

            verticesLists.Add(verticesRow);
        }

        var tanTheta = (bottomRadius - topRadius) / height;
        Vector3 na, nb;

        for (int x = 0; x < radialSegments; x++) {
            if (topRadius != 0) {
                na = vertices[verticesLists[0][x]];
                nb = vertices[verticesLists[0][x + 1]];
            } else {
                na = vertices[verticesLists[1][x]];
                nb = vertices[verticesLists[1][x + 1]];
            }

            na.y = (Mathf.Sqrt(na.x * na.x + na.z * na.z) * tanTheta);
            nb.y = (Mathf.Sqrt(nb.x * nb.x + nb.z * nb.z) * tanTheta);

            for (y = 0; y < heightSegments; y++) {
                var v1 = verticesLists[y][x];
                var v2 = verticesLists[y + 1][x];
                var v3 = verticesLists[y + 1][x + 1];
                var v4 = verticesLists[y][x + 1];

                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v4);

                triangles.Add(v2);
                triangles.Add(v3);
                triangles.Add(v4);
            }

        }

        // top cap
        if (!openEnded && topRadius > 0) {
            vertices.Add(new Vector3(0, heightHalf, 0));

            for (int x = 0; x < radialSegments; x++) {
                var v1 = verticesLists[0][x];
                var v2 = verticesLists[0][x + 1];
                var v3 = vertices.Count - 1;

                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v3);
            }
        }

        // bottom cap
        if (!openEnded && bottomRadius > 0) {
            vertices.Add(new Vector3(0, -heightHalf, 0));

            for (int x = 0; x < radialSegments; x++) {
                var v1 = verticesLists[y][x + 1];
                var v2 = verticesLists[y][x];
                var v3 = vertices.Count - 1;

                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v3);
            }
        }

        Mesh toreturn = new Mesh();
        toreturn.vertices = vertices.ToArray();
        toreturn.triangles = triangles.ToArray();
        return toreturn;
    }
}
