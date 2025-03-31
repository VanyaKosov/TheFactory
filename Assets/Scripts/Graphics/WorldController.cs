using Dev.Kosov.Factory.Core;
using System.Collections;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class WorldController : MonoBehaviour
    {
        private const float playerLayer = -10;
        private const float backGroundLayer = 10;
        private const float oreLayer = 1;

        public readonly World World = new();
        public Catalogs Catalogs;
        public EntityPlacer EntityPlacer;
        public UIController UIController;
        public GameObject Player;
        public GameObject TilePrefab;
        public GameObject OreParent;
        public GameObject TileParent;

        void Start()
        {
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
            return new(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        }

        public Vector2 MapToWorldPos(Vector2Int mapPos)
        {
            return new(mapPos.x, mapPos.y);
        }

        private void SpawnTile(object sender, World.TileGeneratedEventArgs args)
        {
            var created = Instantiate(TilePrefab, new Vector3(args.Pos.x, args.Pos.y, backGroundLayer), Quaternion.identity, TileParent.transform);
            var renderer = created.GetComponent<SpriteRenderer>();
            renderer.sprite = Catalogs.GetRandomBackSprite(args.Background);
            renderer.sortingOrder = -10;
        }

        private void SpawnOre(object sender, World.OreSpawnedEventArgs args)
        {
            var created = Instantiate(TilePrefab, new Vector3(args.Pos.x, args.Pos.y, oreLayer), Quaternion.identity, OreParent.transform);
            var renderer = created.GetComponent<SpriteRenderer>();
            renderer.sprite = Catalogs.GetRandomOreSprite(args.Type, args.RichnessPercent);
        }

        private IEnumerator StartWorld()
        {
            World.Run();
            yield return null;
        }
    }
}