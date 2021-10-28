using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainTile : MonoBehaviour
{
    public Transform buildingRoot;
    public GameObject buildingPrefab;
    private BuildingComposer composer;

    void Start()
    {
        composer = new BuildingComposer();
        SpawnBuilding(Vector3.zero, 50,50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnBuilding(Vector3 position, float sizeX, float sizeY)
    {
        GameObject building = Instantiate(buildingPrefab, buildingRoot);
        building.transform.localPosition =  position - new Vector3(sizeX/2, 0, sizeY/2);

        Mesh buildingMesh = composer.ComposeNew(MetaBuildingType.Debug, sizeX, sizeY);

        building.GetComponent<MeshFilter>().sharedMesh = buildingMesh;
        building.GetComponent<MeshCollider>().sharedMesh = buildingMesh;
    }
}
