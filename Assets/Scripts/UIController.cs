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
            slotSize * Inventory.invWidth + spaceBetweenSlots * (Inventory.invWidth + 1),
            slotSize * Inventory.invHeight + spaceBetweenSlots * (Inventory.invHeight + 1));
    }
}
