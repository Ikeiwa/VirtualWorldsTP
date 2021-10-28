using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Transform player;
    public MainTile tilePrefab;
    public int viewDistance = 3;

    public const float TileSize = 100;

    private List<MainTile> tiles;
    private Vector2Int currentPlayerTile;
    private Vector2Int lastPlayerTile;

    void Start()
    {
        tiles = new List<MainTile>();
        lastPlayerTile = GetPlayerPos();
        StartCoroutine(UpdateTiles());
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerTile = GetPlayerPos();
        if (currentPlayerTile != lastPlayerTile)
        {
            StartCoroutine(UpdateTiles());
            lastPlayerTile = currentPlayerTile;
        }
    }

    private Vector2Int GetTilePosition(Vector3 position)
    {
        int posX = Mathf.RoundToInt(position.x / TileSize);
        int posY = Mathf.RoundToInt(position.z / TileSize);

        return new Vector2Int(posX, posY);
    }

    private Vector2Int GetPlayerPos()
    {
        Vector3 playerPos = player.position;
        return GetTilePosition(playerPos);
    }

    private IEnumerator UpdateTiles()
    {
        int halfView = Mathf.FloorToInt(viewDistance / 2.0f);

        for (int x = 0; x < viewDistance; x++)
        {
            for (int y = 0; y < viewDistance; y++)
            {
                SpawnTile(x-halfView+currentPlayerTile.x, y-halfView+currentPlayerTile.y);
                yield return new WaitForEndOfFrame();
            }
        }

        for (int t = tiles.Count - 1; t >= 0; t--)
        {
            Vector2Int tilePos = GetTilePosition(tiles[t].transform.position);
            Vector2Int tileDist = tilePos - currentPlayerTile;

            if (Mathf.CeilToInt(tileDist.magnitude) >= viewDistance)
            {
                Destroy(tiles[t].gameObject);
                tiles.RemoveAt(t);
            }
                
        }
    }

    private void SpawnTile(int x, int y)
    {
        Vector3 tilePos = new Vector3(x * TileSize, 0, y * TileSize);

        if (Physics.OverlapSphereNonAlloc(tilePos, 1, new Collider[1], 1 << 7) == 0)
        {
            tiles.Add(Instantiate(tilePrefab,tilePos,Quaternion.identity));
        }
    }
}
