using Dev.Kosov.Factory.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityPlacer : MonoBehaviour
    {
        private const float backGroundLayer = 10;
        private const float oreLayer = 1;
        private const float entityRemovalDelay = 0.5f;
        private readonly Dictionary<int, GameObject> entities = new();
        private readonly Dictionary<Vector2Int, GameObject> ores = new();
        private readonly List<RaycastResult> UIObjectsUnderMouse = new();
        private PointerEventData clickData;
        private WorldController worldController;
        private World world;
        private Inventory inventory;
        private ItemType hologramItemType;
        private Rotation currentRotation = Rotation.Up;

        public UserInput UserInput;
        public ActionType ActionType { get; private set; }
        public Catalogs Catalogs;
        public SpriteRenderer hologramRenderer;
        public Camera Camera;
        public GraphicRaycaster Raycaster;
        public GameObject BuildingHologram;
        public GameObject EntityParent;
        public GameObject TilePrefab;
        public GameObject OreParent;
        public GameObject TileParent;

        void OnEnable()
        {
            worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
            world = worldController.World;
            inventory = world.Inventory;

            world.EntityCreated += SpawnEntity;
            world.EntityRemoved += RemoveEntity;
            world.OreMined += UpdateOre;
            world.TileGenerated += SpawnTile;
            world.OreSpawned += SpawnOre;
            inventory.SetCursorItem += SetHologramItem;
            UserInput.SecondaryInput += OnSecondaryInput;
            UserInput.RotateEntity += OnRotateEntity;
        }

        void Start()
        {
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

            hologramRenderer.sprite = Catalogs.GetEntitySprite(ItemInfo.Get(hologramItemType).EntityType, currentRotation);
            BuildingHologram.SetActive(true);
        }

        private void TryPlaceBuilding(Vector2 centerPos)
        {
            if (!Input.GetMouseButton(0)) return;
            if (!ItemInfo.Get(hologramItemType).Placable) return;
            UpdateRaycaster();
            if (UIObjectsUnderMouse.Count != 0) return;

            world.PlaceEntity(DecenterEntityPos(centerPos, EntityInfo.Get(ItemInfo.Get(hologramItemType).EntityType).Size), currentRotation);
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
            try
            {
                int entityID = -1;
                float timeStarted = Time.time;
                Vector2Int pos;
                while (Input.GetMouseButton(1))
                {
                    while (Time.time - timeStarted < entityRemovalDelay)
                    {
                        pos = worldController.WorldToMapPos(Camera.ScreenToWorldPoint(Input.mousePosition));
                        ActionType = world.GetActionType(pos);

                        if (entityID != world.GetTileInfo(pos).EntityID)
                        {
                            entityID = world.GetTileInfo(pos).EntityID;
                            timeStarted = Time.time;

                            continue;
                        }

                        if (!Input.GetMouseButton(1)) yield break;
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

                    world.Remove(pos);
                    timeStarted = Time.time;
                    yield return null;
                }
            }
            finally
            {
                ActionType = ActionType.None;
            }
        }

        private void RemoveEntity(object sender, World.EntityRemovedEventArgs args)
        {
            GameObject instance = entities[args.EntityID];
            Destroy(instance);
        }

        private void SpawnEntity(object sender, World.EntityCreatedEventArgs args)
        {
            GameObject prefab = Catalogs.EntityTypeToPrefab(args.Type, args.Rotation);
            Vector2 pos = CenterEntityPos(args.Pos, args.Size);
            GameObject instance = Instantiate(prefab, pos, Quaternion.identity, EntityParent.transform);
            entities.Add(args.EntityID, instance);
        }

        private void UpdateOre(object sender, World.OreMinedEventArgs args)
        {
            if (args.Type == OreType.None)
            {
                Destroy(ores[args.Pos]);
                ores.Remove(args.Pos);

                return;
            }

            Sprite newSprite = Catalogs.GetNewOreSpriteIfNeeded(args.Type, args.PrevRichnessPercent, args.NewRichnessPercent);
            if (newSprite == null) return;

            ores[args.Pos].GetComponent<SpriteRenderer>().sprite = newSprite;
        }

        private void SpawnOre(object sender, World.OreSpawnedEventArgs args)
        {
            var created = Instantiate(TilePrefab, new Vector3(args.Pos.x, args.Pos.y, oreLayer), Quaternion.identity, OreParent.transform);
            var renderer = created.GetComponent<SpriteRenderer>();
            renderer.sprite = Catalogs.GetRandomOreSprite(args.Type, args.RichnessPercent);

            ores.Add(args.Pos, created);
        }

        private void SpawnTile(object sender, World.TileGeneratedEventArgs args)
        {
            var created = Instantiate(TilePrefab, new Vector3(args.Pos.x, args.Pos.y, backGroundLayer), Quaternion.identity, TileParent.transform);
            var renderer = created.GetComponent<SpriteRenderer>();
            renderer.sprite = Catalogs.GetRandomBackSprite(args.Background);
            renderer.sortingOrder = -10;
        }

        private void SetHologramItem(object sender, Inventory.SetCursorEventArgs args)
        {
            hologramItemType = args.Type;
        }

        private void OnSecondaryInput(object sender, EventArgs args)
        {
            StartCoroutine(TryRemoveEntity());
        }

        private void OnRotateEntity(object sender, EventArgs args)
        {
            switch (currentRotation)
            {
                case Rotation.Up:
                    currentRotation = Rotation.Right;
                    return;
                case Rotation.Right:
                    currentRotation = Rotation.Down;
                    return;
                case Rotation.Down:
                    currentRotation = Rotation.Left;
                    return;
                case Rotation.Left:
                    currentRotation = Rotation.Up;
                    return;
                default:
                    throw new Exception("Unknown rotation");
            }
        }
    }
}
