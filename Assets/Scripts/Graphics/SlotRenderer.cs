using Dev.Kosov.Factory.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class SlotRenderer : MonoBehaviour
    {
        public Image ItemRenderer;
        public TMP_Text TextRenderer;
        public Catalogs ItemSpriteCatalog;

        public void SetItem(ItemType type, int amount)
        {
            if (type == ItemType.None)
            {
                ItemRenderer.gameObject.SetActive(false);
                TextRenderer.gameObject.SetActive(false);
                return;
            }

            ItemRenderer.sprite = ItemSpriteCatalog.GetIconSprite(type);
            TextRenderer.text = amount.ToString();
            ItemRenderer.gameObject.SetActive(true);
            TextRenderer.gameObject.SetActive(true);
        }
    }
}
