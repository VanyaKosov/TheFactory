using Dev.Kosov.Factory.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class UIController : MonoBehaviour
    {
        private const float chestHorOffsetPercent = 0.20f;
        private const float chestVertOffsetPercent = 0.50f;
        private const float invHorOffset = 0f;
        private const float spaceBetweenSlots = 8f;
        private const float slotSize = 45f;
        private const float invVertOffset = 40f;
        private const float hotbarBottomHalfOffsetPercent = 0.05f;

        public const float fadeDuration = 2f;

        private SlotRenderer[,] invSlotRenderers;
        private SlotRenderer[,] hotbarSlotRenderers;
        private CursorSlotRenderer cursorSlotRenderer;
        private World world;
        private Inventory inventory;
        private RectTransform canvasRectTransform;
        private RectTransform cursorRectTransform;
        private Chest openChest = null;
        private GameObject chestInstance;
        private SlotRenderer[,] chestSlotRenderers;

        public UserInput UserInput;
        public bool InvOpen = false;
        public GraphicRaycaster Raycaster;
        public Catalogs SpriteCatalogs;
        public GameObject InventoryParent;
        public GameObject HotbarParent;
        public GameObject CursorParent;
        public GameObject ChestParent;
        public Camera Camera;
        public Canvas Canvas;
        public GameObject InventoryBackPrefab;
        public GameObject SlotPrefab;
        public GameObject CursorSlotPrefab;
        public Image FadePanel;
        public bool testFadeIn = false;
        public bool testFadeOut = false;

        void OnEnable()
        {
            world = GameObject.Find("WorldController").GetComponent<WorldController>().World;
            inventory = world.Inventory;
        }

        void Start()
        {
            canvasRectTransform = Canvas.GetComponent<RectTransform>();
            cursorSlotRenderer = GenerateCursorSlot();
            cursorRectTransform = cursorSlotRenderer.GetComponent<RectTransform>();

            invSlotRenderers = GenerateSlotPanel(InventoryParent,
                new(Canvas.transform.position.x + invHorOffset, Canvas.transform.position.y + invVertOffset),
                inventory.Width, inventory.Height, OnInvSlotLeftClick, OnInvSlotRightClick).renderers;
            hotbarSlotRenderers = GenerateSlotPanel(HotbarParent,
                new(Canvas.transform.position.x, Canvas.transform.position.y * 2 * hotbarBottomHalfOffsetPercent),
                inventory.HotbarWidth, inventory.HotbarHeight, OnHotbarSlotLeftClick, OnHotbarSlotRightClick).renderers;

            world.ChestOpened += OpenChest;

            inventory.SetInvItem += SetInvItem;
            inventory.SetHotbarItem += SetHotbarItem;
            inventory.SetCursorItem += SetCursorItem;

            InventoryParent.SetActive(InvOpen);
        }

        void Update()
        {
            UpdateCursorSlotPos();
            UpdateChestState();

            InventoryParent.SetActive(InvOpen);

            if (testFadeIn == true)
            {
                FadeIn();
                testFadeIn = false;
            }

            if (testFadeOut == true)
            {
                FadeOut();
                testFadeOut = false;
            }
        }

        public void FadeIn()
        {
            StartCoroutine(Fade(0, 1));
        }

        public void FadeOut()
        {
            StartCoroutine(Fade(1, -1));
        }

        private void UpdateChestState()
        {
            if (openChest == null) return;

            if (!InvOpen)
            {
                openChest = null;
                Destroy(chestInstance);
                chestSlotRenderers = null;

                return;
            }

            for (int x = 0; x < openChest.InvWidth; x++)
            {
                for (int y = 0; y < openChest.InvHeight; y++)
                {
                    InvSlot item = openChest.Storage.GetItem(new(x, y));
                    chestSlotRenderers[x, y].SetItem(item.Type, item.Amount, false);
                }
            }
        }

        private (SlotRenderer[,] renderers, GameObject instance) GenerateSlotPanel(GameObject parent, Vector2 position, int width, int height, Action<Vector2Int> leftCallback, Action<Vector2Int> rightCallback)
        {
            SlotRenderer[,] slotRenderers = new SlotRenderer[width, height];

            GameObject back = Instantiate(InventoryBackPrefab, position, Quaternion.identity, parent.transform);
            RectTransform rectTransform = back.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new(
                slotSize * width + spaceBetweenSlots * (width + 1),
                slotSize * height + spaceBetweenSlots * (height + 1));

            float xOffset = (width * slotSize + (width - 1) * spaceBetweenSlots) / 2;
            xOffset -= slotSize / 2;

            float yOffset = (height * slotSize + (height - 1) * spaceBetweenSlots) / 2;
            yOffset -= slotSize / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPos = new(x * (slotSize + spaceBetweenSlots) - xOffset,
                        y * (slotSize + spaceBetweenSlots) - yOffset);
                    GameObject slot = Instantiate(SlotPrefab, worldPos, Quaternion.identity, back.transform);

                    RectTransform slotRectTrans = slot.GetComponent<RectTransform>();
                    slotRectTrans.sizeDelta = new(slotSize, slotSize);
                    slotRectTrans.anchoredPosition = worldPos;

                    SlotRenderer slotRenderer = slot.GetComponent<SlotRenderer>();
                    Vector2Int pos = new(x, y);
                    slotRenderer.Init(SpriteCatalogs, () => leftCallback(pos), () => rightCallback(pos));

                    slotRenderers[x, y] = slotRenderer;
                }
            }

            return (slotRenderers, back);
        }

        private void UpdateCursorSlotPos()
        {
            Vector2 canvasSize = canvasRectTransform.rect.size;
            Vector3 mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseOffset = mousePos - Camera.gameObject.transform.position;
            Vector2 scale = canvasSize / new Vector2(Camera.orthographicSize * 2 * Camera.aspect, Camera.orthographicSize * 2);
            Vector3 scaledMouseOffset = mouseOffset * scale;

            cursorRectTransform.anchoredPosition = scaledMouseOffset;
        }

        private CursorSlotRenderer GenerateCursorSlot()
        {
            GameObject slot = Instantiate(CursorSlotPrefab, new(), Quaternion.identity, CursorParent.transform);
            slot.GetComponent<RectTransform>().sizeDelta = new(slotSize, slotSize);
            SlotRenderer slotRenderer = slot.GetComponent<SlotRenderer>();
            slotRenderer.Init(SpriteCatalogs, null, null);
            CursorSlotRenderer cursorSlotRenderer = slot.GetComponent<CursorSlotRenderer>();
            cursorSlotRenderer.Raycaster = Raycaster;

            return cursorSlotRenderer;
        }

        private void OnInvSlotLeftClick(Vector2Int pos)
        {
            inventory.TryPutToInventory(pos);
        }

        private void OnInvSlotRightClick(Vector2Int pos)
        {
            inventory.TryTakeHalfFromInventory(pos);
        }

        private void OnHotbarSlotLeftClick(Vector2Int pos)
        {
            inventory.TryPutToHotbar(pos);
        }

        private void OnHotbarSlotRightClick(Vector2Int pos)
        {
            inventory.TryTakeHalfFromHotbar(pos);
        }

        private void OnChestLeftClick(Vector2Int pos)
        {
            inventory.TryPutToStorage(pos, openChest.Storage);
        }

        private void OnChestRightClick(Vector2Int pos)
        {
            inventory.TryTakeHalfFromStorage(pos, openChest.Storage);
        }

        private void SetInvItem(object sender, Inventory.SetItemEventArgs args)
        {
            invSlotRenderers[args.Pos.x, args.Pos.y].SetItem(args.Type, args.Amount, false);
        }

        private void SetHotbarItem(object sender, Inventory.SetItemEventArgs args)
        {
            hotbarSlotRenderers[args.Pos.x, args.Pos.y].SetItem(args.Type, args.Amount, false);
        }

        private void SetCursorItem(object sender, Inventory.SetCursorEventArgs args)
        {
            cursorSlotRenderer.SetItem(args.Type, args.Amount);
        }

        private void OpenChest(object sender, World.ChestOpenedEventArgs args)
        {
            Destroy(chestInstance);
            chestSlotRenderers = null;

            InvOpen = true;
            openChest = args.Chest;

            (SlotRenderer[,] renderers, GameObject instance) = GenerateSlotPanel(ChestParent,
                new(Canvas.transform.position.x * 2 * chestHorOffsetPercent, Canvas.transform.position.y * 2 * chestVertOffsetPercent),
                openChest.InvWidth, openChest.InvHeight, OnChestLeftClick, OnChestRightClick);

            chestSlotRenderers = renderers;
            chestInstance = instance;
        }

        private IEnumerator Fade(int startingValue, int direction) // direction should be 1 or -1
        {
            float timeStarted = Time.time;

            float time = Time.time;
            Color color;
            while (time - timeStarted <= fadeDuration)
            {
                color = FadePanel.color;
                FadePanel.color = new(color.r, color.g, color.b, startingValue + direction * (time - timeStarted) / fadeDuration);

                if (color.a < 0f)
                {
                    color.a = 0f;
                    yield break;
                }

                if (color.a > 1f)
                {
                    color.a = 1f;
                    yield break;
                }

                yield return null;

                time = Time.time;
            }

            color = FadePanel.color;
            if (direction == 1)
            {
                FadePanel.color = new(color.r, color.g, color.b, 1f);
            }
            else
            {
                FadePanel.color = new(color.r, color.g, color.b, 0f);
            }
        }
    }
}