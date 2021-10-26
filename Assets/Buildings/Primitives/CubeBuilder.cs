using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBuilder {

    public static Mesh buildCube1t1() {
        Mesh toreturn = new Mesh();

        toreturn.vertices = new Vector3[] {
            new Vector3 (-0.5f,-0.5f,-0.5f),
            new Vector3 (0.5f,-0.5f,-0.5f),
            new Vector3 (0.5f,0.5f,-0.5f),
            new Vector3 (-0.5f,0.5f,-0.5f),
            new Vector3 (-0.5f,0.5f,0.5f),
            new Vector3 (0.5f,0.5f,0.5f),
            new Vector3 (0.5f,-0.5f,0.5f),
            new Vector3 (-0.5f,-0.5f,0.5f)
        };

        toreturn.triangles = new int[] {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6
        };

        return toreturn;
    }
}
