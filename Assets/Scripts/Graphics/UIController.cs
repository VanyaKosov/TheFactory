using Dev.Kosov.Factory.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class UIController : MonoBehaviour
    {
        private const float invHorOffset = 0;
        private const float spaceBetweenSlots = 8f;
        private const float slotSize = 45f;
        private const float invVertOffset = 40f;
        private const float hotbarBottomHalfOffsetPercent = 0.10f;
        private SlotRenderer[,] invSlotRenderers;
        private SlotRenderer[,] hotbarSlotRenderers;
        private CursorSlotRenderer cursorSlotRenderer;
        private Inventory inventory;
        private RectTransform canvasRectTransform;
        private RectTransform cursorRectTransform;

        public UserInput UserInput;
        public bool InvOpen = false;
        public GraphicRaycaster Raycaster;
        public Catalogs SpriteCatalogs;
        public GameObject InventoryParent;
        public GameObject HotbarParent;
        public GameObject CursorParent;
        public Camera Camera;
        public Canvas Canvas;
        public GameObject InventoryBackPrefab;
        public GameObject SlotPrefab;
        public GameObject CursorSlotPrefab;

        void OnEnable()
        {
            inventory = GameObject.Find("WorldController").GetComponent<WorldController>().World.Inventory;
            UserInput.OpenInventory += OnPrimaryInput;
        }

        void Start()
        {
            canvasRectTransform = Canvas.GetComponent<RectTransform>();
            cursorSlotRenderer = GenerateCursorSlot();
            cursorRectTransform = cursorSlotRenderer.GetComponent<RectTransform>();

            invSlotRenderers = GenerateSlotPanel(InventoryParent,
                new(Canvas.transform.position.x + invHorOffset, Canvas.transform.position.y + invVertOffset),
                inventory.Width, inventory.Height, OnInvSlotClick);
            hotbarSlotRenderers = GenerateSlotPanel(HotbarParent,
                new(Canvas.transform.position.x, Canvas.transform.position.y * hotbarBottomHalfOffsetPercent),
                inventory.HotbarWidth, inventory.HotbarHeight, OnHotarSlotClick);

            inventory.SetInvItem += SetInvItem;
            inventory.SetHotbarItem += SetHotbarItem;
            inventory.SetCursorItem += SetCursorItem;

            InventoryParent.SetActive(InvOpen);
        }

        void Update()
        {
            UpdateCursorSlotPos();

            InventoryParent.SetActive(InvOpen);
        }

        private SlotRenderer[,] GenerateSlotPanel(GameObject parent, Vector2 position, int width, int height, Action<Vector2Int> callback)
        {
            SlotRenderer[,] slotRenderers = new SlotRenderer[width, height];

            GameObject back = Instantiate(InventoryBackPrefab, position, Quaternion.identity, parent.transform);
            RectTransform rectTransform = back.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new(
                slotSize * width + spaceBetweenSlots * (width + 1),
                slotSize * height + spaceBetweenSlots * (height + 1));

            float xOffest = (width * slotSize + (width - 1) * spaceBetweenSlots) / 2;
            xOffest -= slotSize / 2;

            float yOffest = (height * slotSize + (height - 1) * spaceBetweenSlots) / 2;
            yOffest -= slotSize / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPos = new(x * (slotSize + spaceBetweenSlots) - xOffest,
                        y * (slotSize + spaceBetweenSlots) - yOffest);
                    GameObject slot = Instantiate(SlotPrefab, worldPos, Quaternion.identity, back.transform);

                    RectTransform slotRectTrans = slot.GetComponent<RectTransform>();
                    slotRectTrans.sizeDelta = new(slotSize, slotSize);
                    slotRectTrans.anchoredPosition = worldPos;

                    SlotRenderer slotRenderer = slot.GetComponent<SlotRenderer>();
                    slotRenderer.ItemSpriteCatalog = SpriteCatalogs;
                    slotRenderers[x, y] = slotRenderer;

                    Button button = slot.GetComponent<Button>();
                    Vector2Int pos = new(x, y);
                    button.onClick.AddListener(() => callback(pos));
                }
            }

            return slotRenderers;
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
            slotRenderer.ItemSpriteCatalog = SpriteCatalogs;
            CursorSlotRenderer cursorSlotRenderer = slot.GetComponent<CursorSlotRenderer>();
            cursorSlotRenderer.Raycaster = Raycaster;

            return cursorSlotRenderer;
        }

        private void OnInvSlotClick(Vector2Int pos)
        {
            inventory.TryPutToInventory(pos);
        }

        private void OnHotarSlotClick(Vector2Int pos)
        {
            inventory.TryPutToHotbar(pos);
        }

        private void OnPrimaryInput(object sender, EventArgs args)
        {
            InvOpen = !InvOpen;
        }

        private void SetInvItem(object sender, Inventory.SetItemEventArgs args)
        {
            invSlotRenderers[args.Pos.x, args.Pos.y].SetItem(args.Type, args.Amount);
        }

        private void SetHotbarItem(object sender, Inventory.SetItemEventArgs args)
        {
            hotbarSlotRenderers[args.Pos.x, args.Pos.y].SetItem(args.Type, args.Amount);
        }

        private void SetCursorItem(object sender, Inventory.SetCursorEventArgs args)
        {
            print("Set cursor item to: " + args.Type + " " + args.Amount);
            cursorSlotRenderer.SetItem(args.Type, args.Amount);
        }
    }
}