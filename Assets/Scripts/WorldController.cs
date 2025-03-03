using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    private const int worldGenRadius = 100;
    private readonly Dictionary<Vector2Int, Tile> map = new();
    private Vector2Int prevPLayerPos;

    public GameObject player;
    public GameObject[] Grass1;
    public GameObject tileParent;

    void Start()
    {
        prevPLayerPos = new(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        GenInitialWorld();
    }

    void Update()
    {
        ExpandMap();
    }

    private void ExpandMap()
    {
        Vector2Int newPlayerPos = new(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        Vector2Int offset = newPlayerPos - prevPLayerPos;
        if (offset.x == 0 && offset.y == 0)
        {
            return;
        }

        if (offset.x != 0 && offset.y != 0)
        {
            Vector2Int tempOffset = new(offset.x, 0);
            MakeLine(newPlayerPos, tempOffset);
            tempOffset = new(0, offset.y);
            MakeLine(newPlayerPos, tempOffset);
        }
        else
        {
            MakeLine(newPlayerPos, offset);
        }

        prevPLayerPos = newPlayerPos;
    }

    private void MakeLine(Vector2Int newPlayerPos, Vector2Int offset)
    {
        Vector2Int pos = newPlayerPos + offset * worldGenRadius;
        offset = new(offset.y, offset.x);
        pos -= offset * worldGenRadius;
        for (int i = 0; i < worldGenRadius * 2 + 1; i++)
        {
            GenTile(pos);
            pos += offset;
        }
    }

    private void GenTile(Vector2Int pos)
    {
        if (map.ContainsKey(pos))
        {
            return;
        }

        int idx = Random.Range(0, Grass1.Length);
        Tile tile = new(pos, Grass1[idx], true, tileParent);
        map.Add(pos, tile);
    }

    private void GenInitialWorld()
    {
        for (int x = -worldGenRadius; x <= worldGenRadius; x++)
        {
            for (int y = -worldGenRadius; y <= worldGenRadius; y++)
            {
                GenTile(new(x, y));
            }
        }
    }

    public class Tile
    {
        public readonly GameObject Instance;
        public readonly bool Passable;
        public int BuildingID { get; set; }

        public Tile(Vector2Int pos, GameObject prefab, bool passable, GameObject tileParent)
        {
            Instance = Instantiate(prefab, new(pos.x, pos.y), Quaternion.identity, tileParent.transform);
            Passable = passable;
        }

        public void Remove()
        {
            Destroy(Instance);
        }
    }
}
