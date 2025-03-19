using Dev.Kosov.Factory.Core;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityPlacer : MonoBehaviour
    {
        public Camera Camera;
        public GameObject EntityParent;
        public GameObject[] TreePrefabs;
        public GameObject WoodChestPrefab;

        private World world;

        void OnEnable()
        {
            world = GameObject.Find("WorldController").GetComponent<WorldController>().World;

            world.EntityCreated += SpawnEntity;
        }

        void Start()
        {

        }

        void Update()
        {
            TryPlaceBuilding();
        }

        private void TryPlaceBuilding()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            int x = (int)(mouseWorldPos.x + (mouseWorldPos.x < 0 ? -0.5f : 0.5f));
            int y = (int)(mouseWorldPos.y + (mouseWorldPos.y < 0 ? -0.5f : 0.5f));
            Vector2Int pos = new(x, y);
            world.PlaceEntity(pos);
        }

        private void SpawnEntity(object sender, World.EntityCreatedEventArgs args)
        {
            GameObject prefab = null;
            switch (args.Type)
            {
                case EntityType.Tree:
                    int idx = Random.Range(0, TreePrefabs.Length);
                    prefab = TreePrefabs[idx];
                    break;
                case EntityType.Assembler1:
                    break;
                case EntityType.WoodChest:
                    prefab = WoodChestPrefab;
                    break;
                default:
                    break;
            }

            Instantiate(prefab, new Vector3(args.Pos.x, args.Pos.y), Quaternion.identity, EntityParent.transform);
            //if (args.Type == EntityType.Tree)
            //{
            //    int idx = Random.Range(0, TreePrefabs.Length);
            //    Instantiate(TreePrefabs[idx], new Vector3(args.Pos.x, args.Pos.y), Quaternion.identity, EntityParent.transform);
            //}
        }
    }
}
