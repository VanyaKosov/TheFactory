using Dev.Kosov.Factory.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityPlacer : MonoBehaviour
    {
        private readonly Dictionary<ItemType, EntityType> itemToEntityType = new()
        {
            { ItemType.Assembler1, EntityType.Assembler1 },
            { ItemType.WoodChest, EntityType.WoodChest }
        };
        private List<RaycastResult> results;
        private PointerEventData clickData;
        private WorldController worldController;
        private World world;
        private Inventory inventory;
        private ItemType hologramItemType;
        private SpriteRenderer hologramRenderer;

        public SpriteCatalogs SpriteCatalogs;
        public Camera Camera;
        public GraphicRaycaster Raycaster;
        public GameObject BuildingHologram;
        public GameObject EntityParent;
        public GameObject[] TreePrefabs;
        public GameObject Assembler1Prefab;
        public GameObject WoodChestPrefab;

        void OnEnable()
        {
            hologramRenderer = BuildingHologram.GetComponent<SpriteRenderer>();

            worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
            world = worldController.World;
            inventory = world.Inventory;

            world.EntityCreated += SpawnEntity;
            inventory.SetCursorItem += SetHologramItem;
        }

        void Start()
        {
            results = new();
            clickData = new(EventSystem.current);
        }

        void Update()
        {
            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int pos = worldController.WorldToMapPos(mouseWorldPos);
            TryPlaceBuilding(pos);
            DisplayBuildingHologram(pos);
        }

        private void DisplayBuildingHologram(Vector2Int pos)
        {
            //BuildingHologram.transform.position = worldController.MapToWorldPos(pos);

            if (!ItemInfo.Get(hologramItemType).Placable)
            {
                BuildingHologram.SetActive(false);
                return;
            }

            Vector2 worldPos = CenterEntityPos(pos, EntityInfo.Get(ItemInfo.Get(hologramItemType).EntityType).Size);
            BuildingHologram.transform.position = worldPos;

            hologramRenderer.sprite = SpriteCatalogs.GetEntitySprite(itemToEntityType[hologramItemType]);
            BuildingHologram.SetActive(true);
        }

        private void TryPlaceBuilding(Vector2Int pos)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            UpdateRaycaster();
            if (results.Count != 0) return;

            world.PlaceEntity(pos); // Make it bottom-left? Then also change Center method

        }

        private void SpawnEntity(object sender, World.EntityCreatedEventArgs args)
        {
            GameObject prefab = EntityTypeToPrefab(args.Type);
            Vector2 pos = CenterEntityPos(args.Pos, args.Size);
            Instantiate(prefab, pos, Quaternion.identity, EntityParent.transform);
        }

        private GameObject EntityTypeToPrefab(EntityType type)
        {
            GameObject prefab = null;

            switch (type)
            {
                case EntityType.Tree:
                    int idx = Random.Range(0, TreePrefabs.Length);
                    prefab = TreePrefabs[idx];
                    break;
                case EntityType.Assembler1:
                    prefab = Assembler1Prefab;
                    break;
                case EntityType.WoodChest:
                    prefab = WoodChestPrefab;
                    break;
                default:
                    break;
            }

            return prefab;
        }

        private Vector2 CenterEntityPos(Vector2Int pos, Vector2Int size)
        {
            /*if (size.x == 1 && size.y == 1) return pos;

            float xDiff = size.x / 2.0f;
            float yDiff = size.y / 2.0f;
            //float x = pos.x + (pos.x < 0 ? xDiff : -xDiff);
            //float y = pos.y + (pos.y < 0 ? yDiff : -yDiff);
            float x = pos.x + xDiff;
            float y = pos.y + yDiff;

            return new(x, y);*/

            if (size.x % 2 != 0) size.x--;
            if (size.y % 2 != 0) size.y--;

            return new(pos.x + size.x / 2, pos.y - size.y / 2);
        }

        private void UpdateRaycaster()
        {
            clickData.position = Input.mousePosition;
            results.Clear();
            Raycaster.Raycast(clickData, results);
        }

        private void SetHologramItem(object sender, Inventory.SetCursorEventArgs args)
        {
            hologramItemType = args.Type;
        }
    }
}
