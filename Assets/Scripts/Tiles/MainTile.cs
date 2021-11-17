using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MainTile : MonoBehaviour
{
    public Transform buildingRoot;
    public GameObject buildingPrefab;
    public Vector2Int tilePos;
    private BuildingComposer composer;
    public Material InteriorMapping;

    public Dictionary<Vector2Int, bool> connections = new Dictionary<Vector2Int, bool>()
    {
        {Vector2Int.up, true},
        {Vector2Int.right, true},
        {Vector2Int.down, true},
        {Vector2Int.left, true}
    };

    void Start()
    {
        composer = new BuildingComposer();

        StartCoroutine(PopulateTile());
    }

    [ContextMenu("Populate")]
    public void TestPopulate()
    {
        TileManager.tiles = new Dictionary<Vector2Int, MainTile>();
        tilePos = TileManager.GetTilePosition(transform.position);
        composer = new BuildingComposer();

        for (int i = buildingRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(buildingRoot.GetChild(i).gameObject);

        StartCoroutine(PopulateTile());
    }

    
    private IEnumerator PopulateTile()
    {
        BuildingBound boundNE = new BuildingBound(new Vector2(2,2),new Vector2(15,15));
        BuildingBound boundNW = new BuildingBound(new Vector2(-2,2),new Vector2(-15,15));
        BuildingBound boundSE = new BuildingBound(new Vector2(2,-2),new Vector2(15,-15));
        BuildingBound boundSW = new BuildingBound(new Vector2(-2,-2),new Vector2(-15,-15)); 

        int closedConnections = 0;

        //connection up
        if (Random.value < 0.25f)
        {
            boundNE.SnapTo(boundNW);
            closedConnections++;
            connections[Vector2Int.up] = false;
        }

        //connection left
        if (Random.value < 0.25f)
        {
            boundNW.SnapTo(boundSW);
            closedConnections++;
            connections[Vector2Int.left] = false;
        }

        //connection down
        if (Random.value< 0.25f)
        {
            boundSW.SnapTo(boundSE);
            closedConnections++;
            connections[Vector2Int.down] = false;
        }

        //connection right
        if (Random.value < 0.25f && closedConnections < 3)
        {
            boundSE.SnapTo(boundNE);
            connections[Vector2Int.right] = false;
        }
        
        SpawnBuilding(boundNE);
        yield return new WaitForEndOfFrame();
        SpawnBuilding(boundNW);
        yield return new WaitForEndOfFrame();
        SpawnBuilding(boundSE);
        yield return new WaitForEndOfFrame();
        SpawnBuilding(boundSW);
    }

    private static readonly int WindowsAlbedo = Shader.PropertyToID("_WindowAlbedo");

    private void SpawnBuilding(BuildingBound bounds)
    {
        GameObject building = Instantiate(buildingPrefab, buildingRoot);
        building.transform.localPosition = new Vector3(bounds.center.x,0,bounds.center.y)  - new Vector3(bounds.extend.x, 0, bounds.extend.y);
    
        Mesh buildingMesh = composer.ComposeNew((MetaBuildingType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(MetaBuildingType)).Length), bounds.size.x, bounds.size.y);

        Material procMat = new Material(InteriorMapping);
        procMat.SetTextureScale(WindowsAlbedo,new Vector2(8,16));
        procMat.SetFloat("_RandomSeed", Random.Range(0,1000));
        procMat.SetFloat("_RoomDepth",Random.Range(2,4));
        procMat.SetFloat("_RoomWidth", Random.Range(4,8));
        if(Random.value > 0.75f)
            procMat.DisableKeyword("_CORRIDOR_ON");
        if (Random.value > 0.9f)
        {
            procMat.SetFloat("_RoomMaxAmbient",0.1f);
            procMat.SetFloat("_RoomMinAmbient", 0.1f);
            procMat.SetFloat("_CorridorMaxAmbient", 0.1f);
            procMat.SetFloat("_CorridorMinAmbient", 0.1f);
        }else if (Random.value > 0.9f)
        {
            procMat.SetFloat("_RoomMaxAmbient", 1f);
            procMat.SetFloat("_RoomMinAmbient", 1f);
            procMat.SetFloat("_CorridorMaxAmbient", 1f);
            procMat.SetFloat("_CorridorMinAmbient", 1f);
        }

        if (Random.value > 0.99f)
        {
            procMat.EnableKeyword("_SPHERE_ON");
        }


        building.GetComponent<MeshFilter>().mesh = buildingMesh;
        building.GetComponent<MeshCollider>().sharedMesh = buildingMesh;
        building.GetComponent<MeshRenderer>().material = procMat;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(MainTile))]
public class MainTileDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 

        if (GUILayout.Button("Populate"))
        {
            ((MainTile)target).TestPopulate();
        }
    }
}
#endif