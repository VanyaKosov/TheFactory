using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    private const float playerLayer = -10;
    private readonly World world = new();

    private static readonly Dictionary<Back, Sprite[]> backToSprite = new();
    private static readonly Dictionary<Ore, Sprite[]> oreToSprite = new();

    public GameObject player;
    public GameObject tilePrefab;
    public GameObject tileParent;

    void Start()
    {
        InitializeBackToSprite();
        InitializeOreToSprite();

        player.transform.position = new(world.PlayerPos.x, world.PlayerPos.y, playerLayer);

        world.TileGenerated += SpawnTile;
        world.OreSpawned += SpawnOre;

        world.Run();
    }

    void Update()
    {
        world.UpdatePLayerPos(player.transform.position);
    }

    void OnDestroy()
    {
        world.TileGenerated -= SpawnTile;
    }

    private void InitializeBackToSprite()
    {
        backToSprite.Add(Back.Empty, null);
        backToSprite.Add(Back.Grass1, Resources.LoadAll<Sprite>("Grass1"));
    }

    private void InitializeOreToSprite()
    {
        oreToSprite.Add(Ore.Empty, null);
        oreToSprite.Add(Ore.Coal, Resources.LoadAll<Sprite>("Coal"));
    }

    private void SpawnTile(object sender, World.TileGeneratedEventArgs args)
    {
        Sprite[] sprites = backToSprite[args.Background];

        int idx = UnityEngine.Random.Range(0, sprites.Length);
        var created = Instantiate(tilePrefab, new Vector3(args.Pos.x, args.Pos.y), Quaternion.identity, tileParent.transform);
        created.GetComponent<SpriteRenderer>().sprite = sprites[idx];
    }

    private void SpawnOre(object sender, World.OreSpawnedEventArgs args)
    {
        print(args.Pos);

        Sprite[] sprites = oreToSprite[args.Type];

        int idx = 0;
        var created = Instantiate(tilePrefab, new Vector3(args.Pos.x, args.Pos.y), Quaternion.identity, tileParent.transform);
        created.GetComponent<SpriteRenderer>().sprite = sprites[idx];
    }
}
