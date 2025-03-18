using Dev.Kosov.Factory.Core;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityPlacer : MonoBehaviour
    {
        public GameObject TreeParent;
        public GameObject[] TreePrefabs;
        public GameObject WoodChestPrefab;

        private World world;
        private Inventory inventory;

        void OnEnable()
        {
            world = GameObject.Find("WorldController").GetComponent<WorldController>().World;
            inventory = world.Inventory;

            world.EntityCreated += SpawnEntity;
        }

        void Start()
        {

        }

        void Update()
        {

        }

        private void SpawnEntity(object sender, World.EntityCreatedEventArgs args)
        {
            if (args.Type == EntityType.Tree)
            {
                int idx = Random.Range(0, TreePrefabs.Length);
                Instantiate(TreePrefabs[idx], new Vector3(args.Pos.x, args.Pos.y), Quaternion.identity, TreeParent.transform);
            }
        }
    }
}
