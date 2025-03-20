using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class UIClicks : MonoBehaviour
    {
        public GraphicRaycaster Raycaster;
        public Camera Camera;

        private List<RaycastResult> results;
        private PointerEventData clickData;

        void Start()
        {
            results = new();
            clickData = new(EventSystem.current);
        }

        void Update()
        {
            OnClick();
        }

        private void OnClick()
        {
            clickData.position = Input.mousePosition;
            results.Clear();
            Raycaster.Raycast(clickData, results);

            if (results.Count != 0)
            {
                print(results.Count);
            }
        }
    }
}
