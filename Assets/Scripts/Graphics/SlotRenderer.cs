using Dev.Kosov.Factory.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class SlotRenderer : MonoBehaviour, IPointerClickHandler
    {
        private readonly Color hologramColor = new(86 / 256f, 173 / 256f, 215 / 256f, 237 / 256f);
        private readonly Color itemColor = new(1f, 1f, 1f, 1f);
        private Catalogs itemSpriteCatalog;
        private Action leftClickCallback;
        private Action rightClickCallback;

        public Image ItemRenderer;
        public TMP_Text TextRenderer;

        public void Init(Catalogs itemSpriteCatalog, Action leftClickCallback, Action rightClickCallback)
        {
            this.itemSpriteCatalog = itemSpriteCatalog;
            this.leftClickCallback = leftClickCallback;
            this.rightClickCallback = rightClickCallback;
        }

        public void SetItem(ItemType type, int amount, bool isHologram)
        {
            if (type == ItemType.None)
            {
                ItemRenderer.gameObject.SetActive(false);
                TextRenderer.gameObject.SetActive(false);
                return;
            }

            if (isHologram)
            {
                ItemRenderer.color = hologramColor;
            }
            else
            {
                ItemRenderer.color = itemColor;
            }

            ItemRenderer.sprite = itemSpriteCatalog.GetIconSprite(type);
            TextRenderer.text = amount.ToString();
            ItemRenderer.gameObject.SetActive(true);
            TextRenderer.gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Left)
            {
                if (leftClickCallback == null) return;

                leftClickCallback();
            }
            else if (data.button == PointerEventData.InputButton.Right)
            {
                if (rightClickCallback == null) return;

                rightClickCallback();
            }
        }
    }
}
