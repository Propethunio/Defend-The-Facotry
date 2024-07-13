using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class AssemblerUI : MonoBehaviour {

    public static AssemblerUI Instance { get; private set; }



    [SerializeField] private List<ItemRecipeSO> itemRecipeScriptableObjectList;

    private Dictionary<ItemRecipeSO, Transform> recipeButtonDic;
    private Assembler assembler;
    private Image craftingProgressBar;

    private void Awake() {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Hide();
        };

        craftingProgressBar = transform.Find("CraftingProgressBar").Find("Bar").GetComponent<Image>();
        craftingProgressBar.fillAmount = 0f;

        SetupRecipes();

        Hide();
    }

    private void Update() {
        UpdateCraftingProgress();
    }

    private void UpdateCraftingProgress() {
        if (assembler != null) {
            craftingProgressBar.fillAmount = assembler.GetCraftingProgressNormalized();
        } else {
            craftingProgressBar.fillAmount = 0f;
        }
    }

    private void SetupRecipes() {
        Transform recipeContainer = transform.Find("RecipeContainer");
        Transform recipeTemplate = recipeContainer.Find("Template");
        recipeTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in recipeContainer) {
            if (transform != recipeTemplate) {
                Destroy(transform.gameObject);
            }
        }

        recipeButtonDic = new Dictionary<ItemRecipeSO, Transform>();

        // Build transforms
        for (int i = 0; i < itemRecipeScriptableObjectList.Count; i++) {
            ItemRecipeSO itemRecipeScriptableObject = itemRecipeScriptableObjectList[i];
            Transform recipeTransform = Instantiate(recipeTemplate, recipeContainer);
            recipeTransform.gameObject.SetActive(true);

            recipeButtonDic[itemRecipeScriptableObject] = recipeTransform;

            recipeTransform.Find("Icon").GetComponent<Image>().sprite = itemRecipeScriptableObject.outputItemList[0].item.sprite;
            //recipeTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemRecipeScriptableObject.name;

            recipeTransform.GetComponent<Button_UI>().ClickFunc = () => {
                if (assembler != null) {
                    assembler.SetItemRecipeScriptableObject(itemRecipeScriptableObject);
                    UpdateSelectedRecipe();
                }
            };
        }

        UpdateSelectedRecipe();
    }

    private void UpdateSelectedRecipe() {
        foreach (ItemRecipeSO itemRecipeScriptableObject in recipeButtonDic.Keys) {
            if (assembler != null && assembler.GetItemRecipeSO() == itemRecipeScriptableObject) {
                // This one is selected
                recipeButtonDic[itemRecipeScriptableObject].Find("Selected").gameObject.SetActive(true);
            } else {
                // Not selected
                recipeButtonDic[itemRecipeScriptableObject].Find("Selected").gameObject.SetActive(false);
            }
        }

        UpdateInputs();
        UpdateOutputs();
    }

    private void UpdateInputs() {
        Transform inputsContainer = transform.Find("InputsContainer");
        Transform inputsTemplate = inputsContainer.Find("Template");
        inputsTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in inputsContainer) {
            if (transform != inputsTemplate) {
                Destroy(transform.gameObject);
            }
        }

        if (assembler != null && assembler.HasItemRecipe()) {
            ItemRecipeSO itemRecipeScriptableObject = assembler.GetItemRecipeSO();

            foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeScriptableObject.inputItemList) {
                Transform inputTransform = Instantiate(inputsTemplate, inputsContainer);
                inputTransform.gameObject.SetActive(true);

                inputTransform.Find("Icon").GetComponent<Image>().sprite = recipeItem.item.sprite;
                inputTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = assembler.GetItemStoredCount(recipeItem.item).ToString();
            }
        }
    }

    private void UpdateOutputs() {
        Transform outputsContainer = transform.Find("OutputsContainer");
        Transform outputsTemplate = outputsContainer.Find("Template");
        outputsTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in outputsContainer) {
            if (transform != outputsTemplate) {
                Destroy(transform.gameObject);
            }
        }

        if (assembler != null && assembler.HasItemRecipe()) {
            ItemRecipeSO itemRecipeScriptableObject = assembler.GetItemRecipeSO();

            foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeScriptableObject.outputItemList) {
                Transform outputTransform = Instantiate(outputsTemplate, outputsContainer);
                outputTransform.gameObject.SetActive(true);

                outputTransform.Find("Icon").GetComponent<Image>().sprite = recipeItem.item.sprite;
                outputTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = assembler.GetItemStoredCount(recipeItem.item).ToString();
            }
        }
    }

    private void Assembler_OnItemStorageCountChanged(object sender, System.EventArgs e) {
        UpdateInputs();
        UpdateOutputs();
    }

    public void Show(Assembler assembler) {
        gameObject.SetActive(true);

        if (this.assembler != null) {
            // Unsub from previous Assembler
            this.assembler.OnItemStorageCountChanged -= Assembler_OnItemStorageCountChanged;
        }

        this.assembler = assembler;

        if (assembler != null) {
            // Sub for item changes
            assembler.OnItemStorageCountChanged += Assembler_OnItemStorageCountChanged;
        }

        UpdateSelectedRecipe();
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

}
