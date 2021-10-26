using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofBuilder
{
    public static Mesh BuildRoof(bool isdouble) {
        Mesh toreturn = new Mesh();

        toreturn.vertices = new Vector3[] {
            new Vector3 (-0.5f,-0.5f,-0.5f),
            new Vector3 (0.5f,-0.5f,-0.5f),
            new Vector3 (0.5f,-0.5f,0.5f),
            new Vector3 (-0.5f,-0.5f,0.5f),
            new Vector3 (isdouble?0:-0.5f,0.5f,-0.5f),
            new Vector3 (isdouble?0:-0.5f,0.5f,0.5f)
        };

        toreturn.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            1, 0, 4,
            2, 1, 4,
            3, 2, 5,
            0, 3, 5,
            5, 4, 0,
            4, 5, 2
        };

        return toreturn;
    }
}
