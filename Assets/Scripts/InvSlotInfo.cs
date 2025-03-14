using System;
using UnityEngine;
using UnityEngine.UI;

public class InvSlotInfo : MonoBehaviour
{
    public Vector2Int SlotPos { get; set; }

    public event EventHandler<SlotClickedEventArgs> SlotClicked;

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnSlotClick);
    }

    public void OnSlotClick()
    {
        SlotClicked?.Invoke(this, new(SlotPos));
    }

    public class SlotClickedEventArgs : EventArgs
    {
        public readonly Vector2Int SlotPos;

        public SlotClickedEventArgs(Vector2Int slotPos)
        {
            SlotPos = slotPos;
        }
    }
}
