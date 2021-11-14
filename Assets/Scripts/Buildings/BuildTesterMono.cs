using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTesterMono : MonoBehaviour
{
    [SerializeField]
    public Material mat;

    private bool buildingchanged = true;
    [SerializeField]
    private MetaBuildingType type = MetaBuildingType.Hive;
    private MetaBuildingType typelast;
    private float lastinvalidate;

    private BuildingComposer composer = new BuildingComposer();

    void Start() {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        typelast = type;
        Mesh m = composer.ComposeNew(type, 8, 8);
        m.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = m;
    }

    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().material = mat;
        if (typelast != type) {
            typelast = type;
            buildingchanged = true;
        }
        if (Time.time > lastinvalidate + 2) {
            lastinvalidate = Time.time;
            buildingchanged = true;
        }
        if (buildingchanged) {
            buildingchanged = false;
            Mesh m = composer.ComposeNew(type, 8, 8);
            m.RecalculateNormals();
            gameObject.GetComponent<MeshFilter>().mesh = m;
        }
    }
}
