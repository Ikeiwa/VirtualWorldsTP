using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveTesterMono : MonoBehaviour
{
    [SerializeField]
    public Material mat;

    private bool meshchanged = true;
    [SerializeField]
    private PrimitiveType type = PrimitiveType.Cube;
    private PrimitiveType typelast;

    void Start() {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        typelast = type;
        gameObject.GetComponent<MeshFilter>().mesh = PrimitiveFactory.GetMesh(type);
    }

    void Update(){
        gameObject.GetComponent<MeshRenderer>().material = mat;
        if (typelast != type) {
            typelast = type;
            meshchanged = true;
        }
        if (meshchanged) {
            gameObject.GetComponent<MeshFilter>().mesh = PrimitiveFactory.GetMesh(type);
        }
    }
}
