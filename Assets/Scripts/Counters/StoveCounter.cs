using System;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter
{
    public event EventHandler<OnProgressChangedEventArgs> OnFryingProgressChanged;

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    private float fryingTimerProgress;

    private void Update()
    {
        HandleFryingTimer();
    }

    private void HandleFryingTimer()
    {
        if (HasKitchenObject())
        {
            KitchenObject kitchenObject = GetKitchenObject();
            KitchenObjectSO kitchenObjectSO = kitchenObject.GetKitchenObjectSO();
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObjectSO);

            // No recipe found (ex. cooked meet -> burned meet)
            if (fryingRecipeSO == null)
            {
                return;
            }

            fryingTimerProgress += Time.deltaTime;

            if (fryingTimerProgress >= fryingRecipeSO.fryingTimerMax)
            {
                ResetFryingTimerProgress();

                kitchenObject.DestroySelf();
                KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
            }
        }
    }

    public override void Interact(Player player)
    {
        // Empty counter
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Prevent put slices to counter
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    ResetFryingTimerProgress();
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
            }
            else
            {
                // Player not carrying anything
            }
        }
        // Counter has KitchenObject
        else
        {

            if (player.HasKitchenObject())
            {
                // Player carrying something
            }
            else
            {
                ResetFryingTimerProgress();
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        // Nothing to do
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetFryingRecipeSOFromInput(inputKitchenObjectSO) != null;
    }

    private FryingRecipeSO GetFryingRecipeSOFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        if (inputKitchenObjectSO != null)
        {
            foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
            {
                if (fryingRecipeSO.input == inputKitchenObjectSO)
                {
                    return fryingRecipeSO;
                }
            }
        }

        return null;
    }

    private void ResetFryingTimerProgress()
    {
        fryingTimerProgress = 0;
        OnFryingProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = 0
        });
    }
}
