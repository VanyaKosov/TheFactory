using Assets.Scripts.Core;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private readonly Inventory inventory = new();
    private const float spaceBetweenSlots = 10f;
    private float slotSize;
    private bool invState = false;

    public GameObject InventoryParent;
    public Canvas Canvas;
    public GameObject InventoryBackPrefab;
    public GameObject SlotPrefab;

    void Start()
    {
        slotSize = SlotPrefab.GetComponent<RectTransform>().rect.width;

        GenerateInventory();
        InventoryParent.SetActive(invState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            invState = !invState;
            InventoryParent.SetActive(invState);
        }
    }

    private void GenerateInventory()
    {
        GameObject back = Instantiate(InventoryBackPrefab, InventoryParent.transform);
        RectTransform rectTransform = back.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new(
            slotSize * inventory.Width + spaceBetweenSlots * (inventory.Width + 1),
            slotSize * inventory.Height + spaceBetweenSlots * (inventory.Height + 1));

        float xOffest = (inventory.Width * slotSize + (inventory.Width - 1) * spaceBetweenSlots) / 2;
        xOffest -= slotSize / 2;

        float yOffest = (inventory.Height * slotSize + (inventory.Height - 1) * spaceBetweenSlots) / 2;
        yOffest -= slotSize / 2;

        for (int x = 0; x < inventory.Width; x++)
        {
            for (int y = 0; y < inventory.Height; y++)
            {
                Vector3 worldPos = new(back.transform.position.x + x * (slotSize + spaceBetweenSlots) - xOffest,
                    back.transform.position.y + y * (slotSize + spaceBetweenSlots) - yOffest);
                GameObject slot = Instantiate(SlotPrefab, worldPos, Quaternion.identity, back.transform);
                InvSlotInfo slotInfo = slot.GetComponent<InvSlotInfo>();
                slotInfo.SlotPos = new(x, y);
                slotInfo.SlotClicked += OnSlotClick;
            }
        }
    }

    private void OnSlotClick(object sender, InvSlotInfo.SlotClickedEventArgs args)
    {
        print(args.SlotPos);
    }
}
