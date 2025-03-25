using Dev.Kosov.Factory.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityPlacer : MonoBehaviour
    {
        private List<RaycastResult> UIObjectsUnderMouse;
        private PointerEventData clickData;
        private WorldController worldController;
        private World world;
        private Inventory inventory;
        private ItemType hologramItemType;
        private Dictionary<int, GameObject> entities = new();

        public Catalogs Catalogs;
        public SpriteRenderer hologramRenderer;
        public Camera Camera;
        public GraphicRaycaster Raycaster;
        public GameObject BuildingHologram;
        public GameObject EntityParent;

        void OnEnable()
        {
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
            TryPlaceBuilding(mouseWorldPos);
            DisplayBuildingHologram(mouseWorldPos);
        }

        private void DisplayBuildingHologram(Vector2 centerPos)
        {
            if (!ItemInfo.Get(hologramItemType).Placable)
            {
                BuildingHologram.SetActive(false);
                return;
            }

            Vector2Int size = EntityInfo.Get(ItemInfo.Get(hologramItemType).EntityType).Size;
            Vector2 pos = CenterEntityPos(DecenterEntityPos(centerPos, size), size);

            BuildingHologram.transform.position = pos;

            hologramRenderer.sprite = Catalogs.GetEntitySprite(ItemInfo.Get(hologramItemType).EntityType);
            BuildingHologram.SetActive(true);
        }

        private void TryPlaceBuilding(Vector2 centerPos)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            UpdateRaycaster();
            if (UIObjectsUnderMouse.Count != 0) return;
            if (!ItemInfo.Get(hologramItemType).Placable) return;

            world.PlaceEntity(DecenterEntityPos(centerPos, EntityInfo.Get(ItemInfo.Get(hologramItemType).EntityType).Size));
        }

        private Vector2 CenterEntityPos(Vector2Int bottomLeftPos, Vector2Int size)
        {
            return bottomLeftPos + new Vector2((size.x - 1) / 2.0f, (size.y - 1) / 2.0f);
        }

        private Vector2Int DecenterEntityPos(Vector2 centerPos, Vector2Int size)
        {
            Func<float, int, int> decenter = (pos, length) =>
                Mathf.RoundToInt(length % 2 == 0 ? pos + 0.5f - length / 2 : pos - length / 2);

            return new(decenter(centerPos.x, size.x), decenter(centerPos.y, size.y));
        }

        private void UpdateRaycaster()
        {
            clickData.position = Input.mousePosition;
            UIObjectsUnderMouse.Clear();
            Raycaster.Raycast(clickData, UIObjectsUnderMouse);
        }

        private void SpawnEntity(object sender, World.EntityCreatedEventArgs args)
        {
            GameObject prefab = Catalogs.EntityTypeToPrefab(args.Type);
            Vector2 pos = CenterEntityPos(args.Pos, args.Size);
            GameObject instance = Instantiate(prefab, pos, Quaternion.identity, EntityParent.transform);
            entities.Add(args.EntityID, instance);
        }

        private void SetHologramItem(object sender, Inventory.SetCursorEventArgs args)
        {
            hologramItemType = args.Type;
        }
    }
}
