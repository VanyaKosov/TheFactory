using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    private const float playerLayer = -10;
    private readonly World world = new();

    public GameObject player;
    public GameObject[] Grass1;
    public Texture2DArray coal;
    public GameObject tileParent;

    void Start()
    {
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

    private void SpawnTile(object sender, World.TileGeneratedEventArgs args)
    {
        if (args.Background == Back.Grass1)
        {
            int idx = UnityEngine.Random.Range(0, Grass1.Length);
            Instantiate(Grass1[idx], new Vector3(args.Pos.x, args.Pos.y), Quaternion.identity, tileParent.transform);
        }
    }

    private void SpawnOre(object sender, World.OreSpawnedEventArgs args)
    {

    }
}
