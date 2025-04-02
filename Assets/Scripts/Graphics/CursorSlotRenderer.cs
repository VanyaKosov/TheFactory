using Dev.Kosov.Factory.Core;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class CursorSlotRenderer : MonoBehaviour
    {
        private readonly List<RaycastResult> UIObjectsUnderMouse = new();
        private PointerEventData clickData;
        private ItemType itemType = ItemType.None;

        public SlotRenderer SlotRenderer;
        public GraphicRaycaster Raycaster;
        public Image ItemRenderer;

        void Start()
        {
            clickData = new(EventSystem.current);
        }

        void Update()
        {
            UpdateVisibility();
        }

        public void SetItem(ItemType type, int amount)
        {
            itemType = type;

            SlotRenderer.SetItem(type, amount);
        }

        private void UpdateVisibility()
        {
            if (!ItemInfo.Get(itemType).Placable)
            {
                ItemRenderer.gameObject.SetActive(false);
                return;
            }

            UpdateRaycaster();
            if (UIObjectsUnderMouse.Count != 0)
            {
                ItemRenderer.gameObject.SetActive(true);
                return;
            }

            ItemRenderer.gameObject.SetActive(false);
        }

        private void UpdateRaycaster()
        {
            clickData.position = Input.mousePosition;
            UIObjectsUnderMouse.Clear();
            Raycaster.Raycast(clickData, UIObjectsUnderMouse);
        }
    }
}