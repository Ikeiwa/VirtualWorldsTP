using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTesterMono : MonoBehaviour
{
    [SerializeField]
    public Material mat;

    private bool buildingchanged = true;
    [SerializeField]
    private MetaBuildingType type = MetaBuildingType.Debug;
    private MetaBuildingType typelast;

    private BuildingComposer composer = new BuildingComposer();

    void Start() {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        typelast = type;
        gameObject.GetComponent<MeshFilter>().mesh = composer.ComposeNew(type,5,5);
    }

    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().material = mat;
        if (typelast != type) {
            typelast = type;
            buildingchanged = true;
        }
        if (buildingchanged) {
            gameObject.GetComponent<MeshFilter>().mesh = composer.ComposeNew(type, 5, 5);
        }
    }
}