using Dev.Kosov.Factory.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityPlacer : MonoBehaviour
    {
        private const float entityRemovalDelay = 0.5f;
        private readonly Dictionary<int, GameObject> entities = new();
        private readonly List<RaycastResult> UIObjectsUnderMouse = new();
        private PointerEventData clickData;
        private WorldController worldController;
        private World world;
        private Inventory inventory;
        private ItemType hologramItemType;

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
            world.EntityRemoved += RemoveEntity;
            inventory.SetCursorItem += SetHologramItem;
        }

        void Start()
        {
            clickData = new(EventSystem.current);
        }

        void Update()
        {
            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            //Vector2Int roundedPos = worldController.WorldToMapPos(mouseWorldPos);
            TryPlaceBuilding(mouseWorldPos);
            DisplayBuildingHologram(mouseWorldPos);

            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(TryRemoveEntity());
            }
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
            static int decenter(float pos, int length) =>
                Mathf.RoundToInt(length % 2 == 0 ? pos + 0.5f - length / 2 : pos - length / 2);

            return new(decenter(centerPos.x, size.x), decenter(centerPos.y, size.y));
        }

        private void UpdateRaycaster()
        {
            clickData.position = Input.mousePosition;
            UIObjectsUnderMouse.Clear();
            Raycaster.Raycast(clickData, UIObjectsUnderMouse);
        }

        private IEnumerator TryRemoveEntity()
        {
            int entityID = -1;
            float timeStarted = 0.0f;
            Vector2Int pos;
            while (Input.GetMouseButton(1))
            {
                pos = worldController.WorldToMapPos(Camera.ScreenToWorldPoint(Input.mousePosition));

                if (entityID != world.GetTileInfo(pos).EntityID)
                {
                    entityID = world.GetTileInfo(pos).EntityID;
                    timeStarted = Time.time;
                }

                while (Time.time - timeStarted < entityRemovalDelay)
                {
                    yield return null;
                }

                pos = worldController.WorldToMapPos(Camera.ScreenToWorldPoint(Input.mousePosition));
                if (entityID != world.GetTileInfo(pos).EntityID)
                {
                    yield return null;
                    continue;
                }

                UpdateRaycaster();
                while (UIObjectsUnderMouse.Count != 0) yield return null;

                world.RemoveEntity(pos);
                timeStarted = Time.time;
                yield return null;
            }
        }

        private void RemoveEntity(object sender, World.EntityRemovedEventArgs args)
        {
            GameObject instance = entities[args.EntityID];
            Destroy(instance);
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
