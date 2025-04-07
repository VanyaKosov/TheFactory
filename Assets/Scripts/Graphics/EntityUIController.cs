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
        private readonly Vector2 recipeChoiceScreenOffset = new(-26.5f, -157.5f);
        private SlotRenderer[] inputSlotRenderers;
        private SlotRenderer[] outputSlotRenderers;
        private World world;
        private Crafter crafter;
        private GameObject recipeChoiceScreen;
        private RectTransform crafterUIRectTransform;

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

        private void GenerateRecipeChoicePanel()
        {
            int numRecipes = crafter.AvailableRecipes.Count;
            int width = Math.Max(numRecipes, recipesPerRow);
            int height = 1;
            if (numRecipes > recipesPerRow)
            {
                height = numRecipes % recipesPerRow == 0 ? numRecipes / recipesPerRow : numRecipes / recipesPerRow + 1;
            }

            GameObject back = Instantiate(RecipeUIBackPrefab, new(0, 0), Quaternion.identity, gameObject.transform);
            recipeChoiceScreen = back;
            RectTransform rectTransform = back.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = crafterUIRectTransform.anchoredPosition + recipeChoiceScreenOffset;
            print(crafterUIRectTransform.anchoredPosition + " " + rectTransform.anchoredPosition);
            rectTransform.sizeDelta = new(
                slotSize * width + spaceBetweenSlots * (width + 1),
                slotSize * height + spaceBetweenSlots * (height + 1));

            float xOffset = (width * slotSize + (width - 1) * spaceBetweenSlots) / 2;
            xOffset -= slotSize / 2;

            float yOffset = (height * slotSize + (height - 1) * spaceBetweenSlots) / 2;
            yOffset -= slotSize / 2;

            int slotsCreated = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (slotsCreated == numRecipes) return;


                    Vector3 worldPos = new(x * (slotSize + spaceBetweenSlots) - xOffset,
                        y * (slotSize + spaceBetweenSlots) - yOffset);
                    GameObject slot = Instantiate(RecipeUISlotPrefab, worldPos, Quaternion.identity, back.transform);

                    RectTransform slotRectTrans = slot.GetComponent<RectTransform>();
                    slotRectTrans.sizeDelta = new(slotSize, slotSize);
                    slotRectTrans.anchoredPosition = worldPos;

                    SlotRenderer slotRenderer = slot.GetComponent<SlotRenderer>();
                    slotRenderer.ItemSpriteCatalog = Catalogs;
                    var recipe = CraftingRecipes.Get(crafter.AvailableRecipes[slotsCreated]);
                    slotRenderer.SetItem(recipe.outputs[0].Type, recipe.outputs[0].Amount);

                    Button button = slot.GetComponent<Button>();
                    button.onClick.AddListener(() => ChosenRecipe(crafter.AvailableRecipes[slotsCreated]));

                    slotsCreated++;
                }
            }
        }

        private void ChosenRecipe(RecipeType recipe)
        {
            Destroy(recipeChoiceScreen);
            crafter.ChangeRecipe(recipe);
        }

        private void OnChangeRecipeClick()
        {
            print("Change recipe");
            GenerateRecipeChoicePanel();
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
