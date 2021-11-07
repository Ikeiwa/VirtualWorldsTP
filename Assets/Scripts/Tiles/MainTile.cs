using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MainTile : MonoBehaviour
{
    public Transform buildingRoot;
    public GameObject buildingPrefab;
    public Vector2Int tilePos;
    private BuildingComposer composer;

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

        PopulateTile();
    }

    [ContextMenu("Populate")]
    public void TestPopulate()
    {
        TileManager.tiles = new Dictionary<Vector2Int, MainTile>();
        tilePos = TileManager.GetTilePosition(transform.position);
        composer = new BuildingComposer();

        for (int i = buildingRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(buildingRoot.GetChild(i).gameObject);

        PopulateTile();
    }

    
    private void PopulateTile()
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
        SpawnBuilding(boundNW);
        SpawnBuilding(boundSE);
        SpawnBuilding(boundSW);
    }

    private void SpawnBuilding(BuildingBound bounds)
    {
        GameObject building = Instantiate(buildingPrefab, buildingRoot);
        building.transform.localPosition = new Vector3(bounds.center.x,0,bounds.center.y)  - new Vector3(bounds.extend.x, 0, bounds.extend.y);

        Mesh buildingMesh = composer.ComposeNew((MetaBuildingType)UnityEngine.Random.Range(1, 5), bounds.size.x, bounds.size.y);

        building.GetComponent<MeshFilter>().sharedMesh = buildingMesh;
        building.GetComponent<MeshCollider>().sharedMesh = buildingMesh;
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