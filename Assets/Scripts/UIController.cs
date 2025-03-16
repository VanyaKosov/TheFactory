using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private const float spaceBetweenSlots = 8f;
    private const float slotSize = 45f;
    private const float invVertOffset = 40f;
    private const float hotbarBottomOffset = 40f;
    private SlotRenderer[,] invSlotRenderers;
    private SlotRenderer[,] hotbarSlotRenderers;
    private SlotRenderer cursorSlotRenderer;
    private bool invOpen = false;

    public Inventory inventory;
    public ItemSpriteCatalog ItemSpriteCatalog;
    public GameObject InventoryParent;
    public GameObject HotbarParent;
    public Camera Camera;
    public Canvas Canvas;
    public GameObject InventoryBackPrefab;
    public GameObject SlotPrefab;
    public GameObject CursorSlotPrefab;

    void Start()
    {
        cursorSlotRenderer = GenerateCursorSlot();
        invSlotRenderers = GenerateSlotPanel(InventoryParent, new(Canvas.transform.position.x, Canvas.transform.position.y + invVertOffset),
            inventory.Width, inventory.Height, OnInvSlotClick);
        hotbarSlotRenderers = GenerateSlotPanel(HotbarParent, new(Canvas.transform.position.x, hotbarBottomOffset),
            inventory.HotbarWidth, 1, OnHotarSlotClick);

        inventory.SetInvItem += SetInvItem;
        inventory.SetHotbarItem += SetHotbarItem;
        inventory.SetCursorItem += SetCursorItem;

        InventoryParent.SetActive(invOpen);
    }

    void Update()
    {
        UpdateCursorSlotPos();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            invOpen = !invOpen;
            InventoryParent.SetActive(invOpen);
        }
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
                Vector3 worldPos = new(back.transform.position.x + x * (slotSize + spaceBetweenSlots) - xOffest,
                    back.transform.position.y + y * (slotSize + spaceBetweenSlots) - yOffest);
                GameObject slot = Instantiate(SlotPrefab, worldPos, Quaternion.identity, back.transform);

                slot.GetComponent<RectTransform>().sizeDelta = new(slotSize, slotSize);

                SlotRenderer slotRenderer = slot.GetComponent<SlotRenderer>();
                slotRenderer.ItemSpriteCatalog = ItemSpriteCatalog;
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
        cursorSlotRenderer.gameObject.transform.position = Camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private SlotRenderer GenerateCursorSlot()
    {
        GameObject slot = Instantiate(CursorSlotPrefab, new(), Quaternion.identity, InventoryParent.transform);
        slot.GetComponent<RectTransform>().sizeDelta = new(slotSize, slotSize);

        return slot.GetComponent<SlotRenderer>();
    }

    private void OnInvSlotClick(Vector2Int pos)
    {
        inventory.SwitchItemWithInventory(pos);
    }

    private void OnHotarSlotClick(Vector2Int pos)
    {
        inventory.SwitchItemWithHotbar(pos);
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
        cursorSlotRenderer.SetItem(args.Type, args.Amount);
    }
}
