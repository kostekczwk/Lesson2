using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

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
                    ResetCuttingProgress();
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
                ResetCuttingProgress();
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        KitchenObject kitchenObject = GetKitchenObject();
        KitchenObjectSO kitchenObjectSO = kitchenObject?.GetKitchenObjectSO();
        KitchenObjectSO outputKitchenObjectSO = GetOutputFromInput(kitchenObjectSO);

        if (kitchenObject != null 
            && kitchenObjectSO != null
            && outputKitchenObjectSO != null)
        {
            CuttingRecipeSO recipeSO = GetCuttingRecipeSOFromInput(kitchenObjectSO);

            IncrementCuttingProgress(recipeSO.cuttingProgressMax);

            if (cuttingProgress >= recipeSO.cuttingProgressMax)
            {
                ResetCuttingProgress();

                kitchenObject.DestroySelf();

                // Create cutting object
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        if (inputKitchenObjectSO != null)
        {
            foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
            {
                if (cuttingRecipeSO.input == inputKitchenObjectSO)
                {
                    return cuttingRecipeSO;
                }
            }
        }

        return null;
    }

    private KitchenObjectSO GetOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO recipeSO = GetCuttingRecipeSOFromInput(inputKitchenObjectSO);
        return recipeSO?.output;
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetCuttingRecipeSOFromInput(inputKitchenObjectSO) != null;
    }

    private void IncrementCuttingProgress(int cuttingProgressMax)
    {
        cuttingProgress++;

        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = GetCuttingProgressNormalized(cuttingProgress, cuttingProgressMax)
        });

        OnCut?.Invoke(this, EventArgs.Empty);
    }

    private void ResetCuttingProgress()
    {
        cuttingProgress = 0;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = 0
        });
    }

    private float GetCuttingProgressNormalized(int cuttingProgress, int cuttingProgressMax)
    {
        return (float)cuttingProgress / (float)cuttingProgressMax;
    }
}
