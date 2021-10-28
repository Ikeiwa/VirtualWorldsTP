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

        StartCoroutine(PopulateTile());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PopulateTile()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                SpawnBuilding(new Vector3(x*10,0,y*10),5,5);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void SpawnBuilding(Vector3 position, float sizeX, float sizeY)
    {
        GameObject building = Instantiate(buildingPrefab, buildingRoot);
        building.transform.localPosition =  position - new Vector3(sizeX/2, 0, sizeY/2);

        Mesh buildingMesh = composer.ComposeNew(MetaBuildingType.BrutalTower, sizeX, sizeY);

        building.GetComponent<MeshFilter>().sharedMesh = buildingMesh;
        building.GetComponent<MeshCollider>().sharedMesh = buildingMesh;
    }
}
