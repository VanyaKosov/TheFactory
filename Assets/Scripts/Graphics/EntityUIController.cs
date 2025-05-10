using Dev.Kosov.Factory.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class EntityUIController : MonoBehaviour
    {
        private const int recipesPerRow = 6;
        private const float spaceBetweenSlots = 8f;
        private const float slotSize = 45f;
        private readonly Vector2 recipeChoiceScreenOffset = new(-26.5f, -157.5f + 45 / 2); // new(-26.5f, -157.5f);
        private SlotRenderer[] inputSlotRenderers;
        private SlotRenderer[] outputSlotRenderers;
        private World world;
        private Crafter crafter;
        private GameObject recipeChoicePanel;
        private RectTransform crafterUIRectTransform;
        private bool choicePanelOpen = false;

        public Catalogs Catalogs;
        public GameObject RecipeUIBackPrefab;
        public GameObject RecipeUISlotPrefab;
        public UIController UIController;
        public WorldController WorldController;
        public GameObject CrafterUI;
        public Slider ProgressBar;
        public GameObject[] InputSlots;
        public GameObject[] OutputSlots;
        public Button ChangeRecipeButton;

        void Start()
        {
            crafterUIRectTransform = CrafterUI.GetComponent<RectTransform>();
            world = WorldController.World;
            inputSlotRenderers = new SlotRenderer[6];
            outputSlotRenderers = new SlotRenderer[6];

            ChangeRecipeButton.onClick.AddListener(OnChangeRecipeClick);
            for (int i = 0; i < InputSlots.Length; i++)
            {
                inputSlotRenderers[i] = InputSlots[i].GetComponent<SlotRenderer>();
                Vector2Int pos = new(i, 0);
                inputSlotRenderers[i].Init(Catalogs, () => OnInputSlotLeftClick(pos), null);
            }
            for (int i = 0; i < OutputSlots.Length; i++)
            {
                outputSlotRenderers[i] = OutputSlots[i].GetComponent<SlotRenderer>();
                Vector2Int pos = new(i, 0);
                outputSlotRenderers[i].Init(Catalogs, () => OnOutputSlotLeftClick(pos), null);
            }

            world.EntityOpened += OpenEntity;
        }

        void Update()
        {
            if (!UIController.InvOpen)
            {
                choicePanelOpen = false;
                Destroy(recipeChoicePanel);
                gameObject.SetActive(false);
            }

            UpdateState();
        }

        private void UpdateState()
        {
            if (crafter == null) return;

            UpdateStorageState(crafter.InputStorage, inputSlotRenderers, crafter.GetExpectedInputItem);
            UpdateStorageState(crafter.OutputStorage, outputSlotRenderers, crafter.GetExpectedOutputItem);

            ProgressBar.value = crafter.GetPercentComplete();
        }

        private void UpdateStorageState(Storage storage, SlotRenderer[] slotRenderers, Func<Vector2Int, InvSlot> getExpected)
        {
            for (int i = 0; i < slotRenderers.Length; i++)
            {
                InvSlot item = storage.GetItem(new(i, 0));
                InvSlot expected = getExpected(new(i, 0));
                if (expected.Type == ItemType.None)
                {
                    slotRenderers[i].gameObject.SetActive(false);
                }
                else
                {
                    slotRenderers[i].gameObject.SetActive(true);
                }

                if (item.Amount == 0)
                {
                    slotRenderers[i].SetItem(expected.Type, expected.Amount, true);
                }
                else
                {
                    slotRenderers[i].SetItem(item.Type, item.Amount, false);
                }
            }
        }

        private void GenerateRecipeChoicePanel()
        {
            int numRecipes = crafter.AvailableRecipes.Count;
            int width = recipesPerRow;
            int height = 1;
            if (numRecipes > recipesPerRow)
            {
                height = numRecipes % recipesPerRow == 0 ? numRecipes / recipesPerRow : numRecipes / recipesPerRow + 1;
            }

            GameObject back = Instantiate(RecipeUIBackPrefab, new(0, 0), Quaternion.identity, gameObject.transform);
            recipeChoicePanel = back;
            RectTransform rectTransform = back.GetComponent<RectTransform>();

            float xOffset = (width * slotSize + (width - 1) * spaceBetweenSlots) / 2;
            float yOffset = (height * slotSize + (height - 1) * spaceBetweenSlots) / 2;

            rectTransform.anchoredPosition =
                crafterUIRectTransform.anchoredPosition +
                recipeChoiceScreenOffset -
                new Vector2(0, yOffset);

            rectTransform.sizeDelta = new(
                slotSize * width + spaceBetweenSlots * (width + 1),
                slotSize * height + spaceBetweenSlots * (height + 1));

            xOffset -= slotSize / 2;
            yOffset -= slotSize / 2;

            int slotsCreated = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    if (slotsCreated == numRecipes) return;

                    Vector3 worldPos = new(x * (slotSize + spaceBetweenSlots) - xOffset,
                        y * (slotSize + spaceBetweenSlots) - yOffset);
                    GameObject slot = Instantiate(RecipeUISlotPrefab, worldPos, Quaternion.identity, back.transform);

                    RectTransform slotRectTrans = slot.GetComponent<RectTransform>();
                    slotRectTrans.sizeDelta = new(slotSize, slotSize);
                    slotRectTrans.anchoredPosition = worldPos;

                    SlotRenderer slotRenderer = slot.GetComponent<SlotRenderer>();
                    RecipeType recipeType = crafter.AvailableRecipes[slotsCreated];
                    slotRenderer.Init(Catalogs, () => ChosenRecipe(recipeType), null);

                    var recipe = CraftingRecipes.Get(crafter.AvailableRecipes[slotsCreated]);
                    slotRenderer.SetItem(recipe.outputs[0].Type, recipe.outputs[0].Amount, false);

                    slotsCreated++;
                }
            }
        }

        private void ChosenRecipe(RecipeType recipe)
        {
            choicePanelOpen = false;
            Destroy(recipeChoicePanel);
            crafter.ChangeRecipe(recipe);
        }

        private void OnChangeRecipeClick()
        {
            choicePanelOpen = !choicePanelOpen;

            if (choicePanelOpen)
            {
                GenerateRecipeChoicePanel();
            }
            else
            {
                Destroy(recipeChoicePanel);
            }
        }

        private void OnInputSlotLeftClick(Vector2Int pos)
        {
            world.TryPutOrTakeFromCrafterInput(crafter, pos);
        }

        private void OnOutputSlotLeftClick(Vector2Int pos)
        {
            world.TryTakeFromCrafterOutput(crafter, pos);
        }

        private void OpenEntity(object sender, World.EntityOpenedEventArgs args)
        {
            choicePanelOpen = false;
            Destroy(recipeChoicePanel);

            crafter = args.Crafter;
            UIController.InvOpen = true;
            gameObject.SetActive(true);
        }
    }
}
