using Dev.Kosov.Factory.Core;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class InserterController : MonoBehaviour, IEntityInitializer
    {
        private Inserter inserter;
        private SpriteRenderer itemRenderer;
        private Catalogs catalogs;

        public GameObject ArmBase;
        public GameObject ItemIcon;

        void Start()
        {
            itemRenderer = ItemIcon.GetComponent<SpriteRenderer>();
            ItemIcon.SetActive(false);
        }

        void Update()
        {
            ArmBase.transform.eulerAngles = new(0, 0, inserter.GetTotalDegrees());
            ItemIcon.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            if (inserter.Item.Type == ItemType.None)
            {
                ItemIcon.SetActive(false);
                return;
            }

            itemRenderer.sprite = catalogs.GetIconSprite(inserter.Item.Type);
            ItemIcon.SetActive(true);
        }

        public void Init(Entity entity, Catalogs catalogs)
        {
            inserter = (Inserter)entity;
            this.catalogs = catalogs;
        }
    }
}
