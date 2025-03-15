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
    private readonly Inventory inventory = new();
    private bool invOpen = false;

    public GameObject InventoryParent;
    public GameObject HotbarParent;
    public Canvas Canvas;
    public GameObject InventoryBackPrefab;
    public GameObject SlotPrefab;

    void Start()
    {
        GenerateSlotPanel(InventoryParent, new(Canvas.transform.position.x, Canvas.transform.position.y + invVertOffset),
            inventory.Width, inventory.Height, OnInvSlotClick);
        GenerateSlotPanel(HotbarParent, new(Canvas.transform.position.x, hotbarBottomOffset),
            inventory.HotbarWidth, 1, OnHotarSlotClick);

        InventoryParent.SetActive(invOpen);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            invOpen = !invOpen;
            InventoryParent.SetActive(invOpen);
        }
    }

    private void GenerateSlotPanel(GameObject parent, Vector2 position, int width, int height, Action<Vector2Int> callback)
    {
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
                Button button = slot.GetComponent<Button>();
                Vector2Int pos = new(x, y);
                button.onClick.AddListener(() => callback(pos));
            }
        }
    }

    private void OnInvSlotClick(Vector2Int pos)
    {
        print("inventory: " + pos);
    }

    private void OnHotarSlotClick(Vector2Int pos)
    {
        print("hotbar" + pos);
    }
}
