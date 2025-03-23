using Dev.Kosov.Factory.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class WorldController : MonoBehaviour
    {
        private const float playerLayer = -10;
        private const float backGroundLayer = 10;
        private const float oreLayer = 1;

        private static readonly Dictionary<Back, Sprite[]> backToSprite = new();
        private static readonly Dictionary<Ore, Sprite[]> oreToSprite = new();

        public readonly World World = new();
        public EntityPlacer EntityPlacer;
        public UIController UIController;
        public GameObject Player;
        public GameObject TilePrefab;
        public GameObject OreParent;
        public GameObject TileParent;

        void Start()
        {
            InitializeBackToSprite();
            InitializeOreToSprite();

            Player.transform.position = new(World.PlayerPos.x, World.PlayerPos.y, playerLayer);

            World.TileGenerated += SpawnTile;
            World.OreSpawned += SpawnOre;

            StartCoroutine(StartWorld());
        }

        void Update()
        {
            World.UpdatePlayerPos(Player.transform.position);
        }

        void OnDestroy()
        {
            World.TileGenerated -= SpawnTile;
            World.OreSpawned -= SpawnOre;
        }

        public Vector2Int WorldToMapPos(Vector2 worldPos)
        {
            int x = (int)(worldPos.x + (worldPos.x < 0 ? -0.5f : 0.5f));
            int y = (int)(worldPos.y + (worldPos.y < 0 ? -0.5f : 0.5f));
            return new(x, y);
        }

        public Vector2 MapToWorldPos(Vector2Int mapPos)
        {
            return new(mapPos.x, mapPos.y);
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
            oreToSprite.Add(Ore.Copper, Resources.LoadAll<Sprite>("Copper"));
            oreToSprite.Add(Ore.Iron, Resources.LoadAll<Sprite>("Iron"));
        }

        private void SpawnTile(object sender, World.TileGeneratedEventArgs args)
        {
            Sprite[] sprites = backToSprite[args.Background];

            int idx = UnityEngine.Random.Range(0, sprites.Length);
            var created = Instantiate(TilePrefab, new Vector3(args.Pos.x, args.Pos.y, backGroundLayer), Quaternion.identity, TileParent.transform);
            var renderer = created.GetComponent<SpriteRenderer>();
            renderer.sprite = sprites[idx];
            renderer.sortingOrder = -10;
        }

        private void SpawnOre(object sender, World.OreSpawnedEventArgs args)
        {
            Sprite[] sprites = oreToSprite[args.Type];

            int idx = sprites.Length - 1 - (int)(sprites.Length / 100f * args.RichnessPercent);
            idx -= idx % 8;
            idx += UnityEngine.Random.Range(0, 8);
            var created = Instantiate(TilePrefab, new Vector3(args.Pos.x, args.Pos.y, oreLayer), Quaternion.identity, OreParent.transform);
            var renderer = created.GetComponent<SpriteRenderer>();
            renderer.sprite = sprites[idx];
        }

        private IEnumerator StartWorld()
        {
            World.Run();
            yield return null;
        }
    }
}