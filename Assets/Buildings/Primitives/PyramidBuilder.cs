using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidBuilder {
    public static Mesh buildPyramidt1() {
        Mesh toreturn = new Mesh();

        toreturn.vertices = new Vector3[] {
            new Vector3 (0,0,0),
            new Vector3 (1,0,0),
            new Vector3 (1,0,1),
            new Vector3 (0,0,1),
            new Vector3 (0.5f,1,0.5f),
        };

        toreturn.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            0, 1, 4,
            1, 2, 4,
            2, 3, 4,
            3, 0, 4
        };

        return toreturn;
    }
}
