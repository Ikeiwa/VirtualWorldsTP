using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Transform player;
    public MainTile tilePrefab;
    public int viewDistance = 3;

    public const float TileSize = 30;

    public static Dictionary<Vector2Int,MainTile> tiles;
    private Vector2Int currentPlayerTile;
    private Vector2Int lastPlayerTile;

    void Start()
    {
        tiles = new Dictionary<Vector2Int,MainTile>();
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

    public static Vector2Int GetTilePosition(Vector3 position)
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

        List<Vector2Int> tagToRemove = new List<Vector2Int>();

        foreach (var tile in tiles.Keys)
        {
            Vector2Int tileDist = tile - currentPlayerTile;

            if (Mathf.CeilToInt(tileDist.magnitude) >= viewDistance)
            {
                tagToRemove.Add(tile);
            }
        }

        foreach (var tileToRemove in tagToRemove)
        {
            Destroy(tiles[tileToRemove].gameObject);
            tiles.Remove(tileToRemove);
        }
    }

    private void SpawnTile(int x, int y)
    {
        Vector2Int tile = new Vector2Int(x, y);
        if (!tiles.ContainsKey(tile))
        {
            Vector3 tilePos = new Vector3(x * TileSize, 0, y * TileSize);
            tiles.Add(tile,Instantiate(tilePrefab,tilePos,Quaternion.identity));
        }
    }

    public static MainTile GetTileAt(Vector2Int pos)
    {
        if (tiles.ContainsKey(pos))
            return tiles[pos];
        return null;
    }
}
