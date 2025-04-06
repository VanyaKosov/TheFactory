using Dev.Kosov.Factory.Core;
using System.Collections;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class WorldController : MonoBehaviour
    {
        private const float playerLayer = -10;

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

            StartCoroutine(StartWorld());
        }

        void Update()
        {
            World.UpdatePlayerPos(Player.transform.position);
            World.UpdateState();
        }

        public Vector2Int WorldToMapPos(Vector2 worldPos)
        {
            return new(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        }

        public Vector2 MapToWorldPos(Vector2Int mapPos)
        {
            return new(mapPos.x, mapPos.y);
        }

        private IEnumerator StartWorld()
        {
            World.Run();
            yield return null;
        }
    }
}