using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidBuilder {
    public static Mesh buildPyramidt1() {
        Mesh toreturn = new Mesh();

        toreturn.vertices = new Vector3[] {
            new Vector3 (-0.5f,-0.5f,-0.5f),
            new Vector3 (0.5f,-0.5f,-0.5f),
            new Vector3 (0.5f,-0.5f,0.5f),
            new Vector3 (-0.5f,-0.5f,0.5f),
            new Vector3 (0,1,0),
        };

        toreturn.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            1, 0, 4,
            2, 1, 4,
            3, 2, 4,
            0, 3, 4
        };

        return toreturn;
    }
}
