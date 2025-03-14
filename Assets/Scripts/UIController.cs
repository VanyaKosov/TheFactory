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
            slotSize * Inventory.Width + spaceBetweenSlots * (Inventory.Width + 1),
            slotSize * Inventory.Height + spaceBetweenSlots * (Inventory.Height + 1));

        float x = 0;
        if (Inventory.Width % 2 == 0)
        {
            x -= slotSize * (Inventory.Width / 2) + spaceBetweenSlots * (Inventory.Width / 2 + 1);
        }
        else
        {
            x -= slotSize * (Inventory.Width / 2) + slotSize / 2 + spaceBetweenSlots * (Inventory.Width / 2 + 1);
        }

        float y = 0;
        if (Inventory.Height % 2 == 0)
        {
            y -= slotSize * (Inventory.Height / 2) + spaceBetweenSlots * (Inventory.Height / 2 + 1);
        }
        else
        {
            y -= slotSize * (Inventory.Height / 2) + slotSize / 2 + spaceBetweenSlots * (Inventory.Height / 2 + 1);
        }

        for (float xPos = x; xPos < Inventory.Width; xPos += slotSize + spaceBetweenSlots)
        {
            for (float yPos = y; yPos < Inventory.Height; yPos += slotSize + spaceBetweenSlots)
            {
                Instantiate(SlotPrefab, new Vector3(xPos, yPos), Quaternion.identity, back.transform);
            }
        }
    }
}
