using Dev.Kosov.Factory.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityUIController : MonoBehaviour
    {
        private SlotRenderer[] inputSlotRenderers;
        private SlotRenderer[] outputSlotRenderers;
        private World world;
        private Crafter crafter;

        public UIController UIController;
        public WorldController WorldController;
        public GameObject CrafterUI;
        public Slider ProgressBar;
        public GameObject[] InputSlots;
        public GameObject[] OutputSlots;
        public Button ChangeRecipeButton;

        void Start()
        {
            world = WorldController.World;
            inputSlotRenderers = new SlotRenderer[6];
            outputSlotRenderers = new SlotRenderer[6];

            ChangeRecipeButton.onClick.AddListener(OnChangeRecipeClick);
            for (int i = 0; i < InputSlots.Length; i++)
            {
                inputSlotRenderers[i] = InputSlots[i].GetComponent<SlotRenderer>();
                Button button = InputSlots[i].GetComponent<Button>();
                Vector2Int pos = new(i, 0);
                button.onClick.AddListener(() => OnInputSlotClick(pos));
            }
            for (int i = 0; i < OutputSlots.Length; i++)
            {
                outputSlotRenderers[i] = OutputSlots[i].GetComponent<SlotRenderer>();
                Button button = OutputSlots[i].GetComponent<Button>();
                Vector2Int pos = new(i, 0);
                button.onClick.AddListener(() => OnOutputSlotClick(pos));
            }

            world.EntityOpened += OpenEntity;
        }

        void Update()
        {
            if (!UIController.InvOpen)
            {
                gameObject.SetActive(false);
            }

            UpdateState();
        }

        private void UpdateState()
        {
            if (crafter == null) return;

            for (int i = 0; i < inputSlotRenderers.Length; i++)
            {
                InvSlot item = crafter.InputStorage.GetItem(new(i, 0));
                if (crafter.GetExpectedInputItem(new(i, 0)) == ItemType.None)
                {
                    inputSlotRenderers[i].gameObject.SetActive(false);
                }
                else
                {
                    inputSlotRenderers[i].gameObject.SetActive(true);
                }

                inputSlotRenderers[i].SetItem(item.Type, item.Amount);
            }

            for (int i = 0; i < outputSlotRenderers.Length; i++)
            {
                InvSlot item = crafter.OutputStorage.GetItem(new(i, 0));
                if (crafter.GetExpectedOutputItem(new(i, 0)) == ItemType.None)
                {
                    outputSlotRenderers[i].gameObject.SetActive(false);
                }
                else
                {
                    outputSlotRenderers[i].gameObject.SetActive(true);
                }
                outputSlotRenderers[i].SetItem(item.Type, item.Amount);
            }

            ProgressBar.value = crafter.GetPercentComplete();
        }

        private void OnChangeRecipeClick()
        {
            print("Change recipe");
        }

        private void OnInputSlotClick(Vector2Int pos)
        {
            //print("Input " + pos.x);
            world.TryPutOrTakeFromCrafterInput(crafter, pos);
        }

        private void OnOutputSlotClick(Vector2Int pos)
        {
            //print("Output " + pos.x);
            world.TryTakeFromCrafterOutput(crafter, pos);
        }

        private void OpenEntity(object sender, World.EntityOpenedEventArgs args)
        {
            crafter = args.Crafter;
            UIController.InvOpen = true;
            gameObject.SetActive(true);
        }
    }
}
