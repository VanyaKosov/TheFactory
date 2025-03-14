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

        float xOffest = 0;
        if (inventory.Width % 2 == 0)
        {
            xOffest += slotSize * (inventory.Width / 2) + spaceBetweenSlots * ((inventory.Width - 1) / 2f) + spaceBetweenSlots;
        }
        else
        {
            xOffest += slotSize * ((inventory.Width - 1) / 2) + slotSize / 2 + spaceBetweenSlots * ((inventory.Width + 1) / 2);
        }
        xOffest -= slotSize / 2 + spaceBetweenSlots;

        float yOffest = 0;
        if (inventory.Height % 2 == 0)
        {
            yOffest += slotSize * (inventory.Height / 2) + spaceBetweenSlots * ((inventory.Height - 1) / 2f) + spaceBetweenSlots;
        }
        else
        {
            yOffest += slotSize * ((inventory.Height - 1) / 2) + slotSize / 2 + spaceBetweenSlots * ((inventory.Height + 1) / 2);
        }
        yOffest -= slotSize / 2 + spaceBetweenSlots;

        for (float xPos = rectTransform.position.x - xOffest; xPos < rectTransform.position.x + xOffest + slotSize; xPos += slotSize + spaceBetweenSlots)
        {
            for (float yPos = rectTransform.position.y - yOffest; yPos < rectTransform.position.y + yOffest + slotSize; yPos += slotSize + spaceBetweenSlots)
            {
                Instantiate(SlotPrefab, new Vector3(xPos, yPos), Quaternion.identity, back.transform);
            }
        }
    }
}
