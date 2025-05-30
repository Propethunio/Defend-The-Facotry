using UnityEngine;

[CreateAssetMenu(menuName = "Quests/UI Interaction/Change Recipe")]
public class ChangeRecipeStepSO : QuestStepSO {
    
    private ConstructorPopup popup;

    public override void Execute() {
        popup = FindFirstObjectByType<ConstructorPopup>(FindObjectsInactive.Include);
        popup.OnRecipeChanged += RecipeChanged;
    }

    protected override void CompleteStep() {
        popup.OnRecipeChanged -= RecipeChanged;
        base.CompleteStep();
    }

    private void RecipeChanged(int index) {
        if (index != 1) return;
        
        CompleteStep();
    }
}