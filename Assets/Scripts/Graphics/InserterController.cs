using Dev.Kosov.Factory.Core;
using System.Collections;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class InserterController : MonoBehaviour, IEntityInitializer
    {
        private const float maxDegrees = 360f;
        private const float minDegrees = 180f;
        private const float degreesPerSecond = 180f;
        private float degrees = 180f;
        private int direction = -1;
        private bool waiting = false;
        private Inserter inserter;
        private SpriteRenderer itemRenderer;
        private Catalogs catalogs;

        public GameObject ArmBase;
        public GameObject ItemIcon;

        void Start()
        {
            if (inserter != null)
            {
                itemRenderer = ItemIcon.GetComponent<SpriteRenderer>();
                ItemIcon.SetActive(false);
            }
        }

        void Update()
        {
            if (inserter == null)
            {
                SimplifiedUpdate();
            }
            else
            {
                ProperUpdate();
            }
        }

        public void Init(Entity entity, Catalogs catalogs)
        {
            inserter = (Inserter)entity;
            this.catalogs = catalogs;
        }

        private void SimplifiedUpdate()
        {
            if (waiting) return;

            degrees += degreesPerSecond * Time.deltaTime * direction;
            if (degrees < minDegrees)
            {
                direction = 1;
                degrees = minDegrees;
                StartCoroutine(WaitForRandomTime());
            }
            if (degrees > maxDegrees)
            {
                direction = -1;
                degrees = maxDegrees;
                StartCoroutine(WaitForRandomTime());
            }

            ArmBase.transform.eulerAngles = new(0, 0, degrees);
        }

        private void ProperUpdate()
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

        private IEnumerator WaitForRandomTime()
        {
            float timeToWait = Random.value;
            waiting = true;
            yield return new WaitForSeconds(timeToWait);
            waiting = false;
        }
    }
}
