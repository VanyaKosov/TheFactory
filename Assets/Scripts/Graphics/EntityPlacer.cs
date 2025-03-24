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
        private List<RaycastResult> UIObjectsUnderMouse;
        private PointerEventData clickData;
        private WorldController worldController;
        private World world;
        private Inventory inventory;
        private ItemType hologramItemType;

        public SpriteCatalogs SpriteCatalogs;
        public SpriteRenderer hologramRenderer;
        //public GameObject Player;
        public Camera Camera;
        public GraphicRaycaster Raycaster;
        public GameObject BuildingHologram;
        public GameObject EntityParent;
        public GameObject[] TreePrefabs;
        public GameObject Assembler1Prefab;
        public GameObject WoodChestPrefab;

        void OnEnable()
        {
            //hologramRenderer = BuildingHologram.GetComponent<SpriteRenderer>();

            worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
            world = worldController.World;
            inventory = world.Inventory;

            world.EntityCreated += SpawnEntity;
            inventory.SetCursorItem += SetHologramItem;
        }

        void Start()
        {
            UIObjectsUnderMouse = new();
            clickData = new(EventSystem.current);
        }

        void Update()
        {
            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int pos = worldController.WorldToMapPos(mouseWorldPos);
            TryPlaceBuilding(pos);
            DisplayBuildingHologram(pos);
        }

        private void DisplayBuildingHologram(Vector2Int centerPos)
        {
            if (!ItemInfo.Get(hologramItemType).Placable)
            {
                BuildingHologram.SetActive(false);
                return;
            }

            BuildingHologram.transform.position = new(centerPos.x, centerPos.y);

            hologramRenderer.sprite = SpriteCatalogs.GetEntitySprite(itemToEntityType[hologramItemType]);
            BuildingHologram.SetActive(true);
        }

        private void TryPlaceBuilding(Vector2Int centerPos)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            UpdateRaycaster();
            if (UIObjectsUnderMouse.Count != 0) return;

            world.PlaceEntity(DecenterEntityPos(centerPos, EntityInfo.Get(ItemInfo.Get(hologramItemType).EntityType).Size));
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

        private Vector2Int CenterEntityPos(Vector2Int bottomLeftPos, Vector2Int size)
        {
            if (size.x % 2 != 0) size.x--;
            if (size.y % 2 != 0) size.y--;

            return new(bottomLeftPos.x + size.x / 2, bottomLeftPos.y + size.y / 2);
        }

        private Vector2Int DecenterEntityPos(Vector2Int centerPos, Vector2Int size)
        {
            if (size.x % 2 != 0) size.x--;
            if (size.y % 2 != 0) size.y--;

            return new(centerPos.x - size.x / 2, centerPos.y - size.y / 2);
        }

        private void UpdateRaycaster()
        {
            clickData.position = Input.mousePosition;
            UIObjectsUnderMouse.Clear();
            Raycaster.Raycast(clickData, UIObjectsUnderMouse);
        }

        private void SpawnEntity(object sender, World.EntityCreatedEventArgs args)
        {
            GameObject prefab = EntityTypeToPrefab(args.Type);
            Vector2 pos = CenterEntityPos(args.Pos, args.Size);
            Instantiate(prefab, pos, Quaternion.identity, EntityParent.transform);
        }

        private void SetHologramItem(object sender, Inventory.SetCursorEventArgs args)
        {
            hologramItemType = args.Type;
        }
    }
}
