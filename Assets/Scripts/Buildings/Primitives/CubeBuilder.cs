using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBuilder
{

    public static readonly Vector3[] vertices =
    {
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f)
    };

    public static readonly Vector3[] normals =
    {
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f)
    };

    public static readonly int[] triangles =
    {
        0,2,3,
        0,3,1,
        8,4,5,
        8,5,9,
        10,6,7,
        10,7,11,
        12,13,14,
        12,14,15,
        16,17,18,
        16,18,19,
        20,21,22,
        20,22,23
    };

    public static Mesh buildCube1t1()
    {
        Mesh toreturn = new Mesh();

        toreturn.vertices = vertices;
        toreturn.normals = normals;
        toreturn.triangles = triangles;

        return toreturn;
    }
}
