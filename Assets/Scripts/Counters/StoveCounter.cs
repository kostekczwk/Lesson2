using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State State { get; set; }
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    private State currentState;

    private float fryingTimerProgress;
    private FryingRecipeSO fryingRecipeSO;
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    private float burningTimerProgress;
    private BurningRecipeSO burningRecipeSO;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private void Start()
    {
        currentState = State.Idle;
    }

    private void Update()
    {
        HandleFrying();
    }

    private void HandleFrying()
    {
        if (HasKitchenObject())
        {
            switch (currentState)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    HandleFryingState();
                    break;
                case State.Fried:
                    HandleFriedState();
                    break;
                case State.Burned:
                    break;
                default:
                    break;
            }
        }
    }

    private void HandleFryingState()
    {
        fryingTimerProgress += Time.deltaTime;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs 
        { 
            progressNormalized = fryingTimerProgress / fryingRecipeSO.fryingTimerMax
        });

        if (fryingTimerProgress >= fryingRecipeSO.fryingTimerMax)
        {
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

            currentState = State.Fried;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = currentState });

            burningTimerProgress = 0;
            burningRecipeSO = GetBurningRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
        }
    }

    private void HandleFriedState()
    {
        burningTimerProgress += Time.deltaTime;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs 
        { 
            progressNormalized = burningTimerProgress / burningRecipeSO.burningTimerMax
        });

        if (burningTimerProgress >= burningRecipeSO.burningTimerMax)
        {
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

            currentState = State.Burned;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = currentState });
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
                KitchenObjectSO kitchenObjectSO = player.GetKitchenObject().GetKitchenObjectSO();
                if (HasFryingRecipeWithInput(kitchenObjectSO))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    ResetFryingTimerProgress();

                    fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObjectSO);

                    currentState = State.Frying;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = currentState });
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
            // Player carrying something
            if (player.HasKitchenObject())
            {
                // Counter has KitchenObject and it is plate
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(this.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        this.GetKitchenObject().DestroySelf();

                        ResetFryingTimerProgress();
                        ResetBurningTimerProgress();

                        currentState = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = currentState });
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                ResetFryingTimerProgress();
                ResetBurningTimerProgress();

                currentState = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = currentState });
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        // Nothing to do
    }

    private bool HasFryingRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
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

        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = 0
        });
    }

    private BurningRecipeSO GetBurningRecipeSOFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        if (inputKitchenObjectSO != null)
        {
            foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
            {
                if (burningRecipeSO.input == inputKitchenObjectSO)
                {
                    return burningRecipeSO;
                }
            }
        }

        return null;
    }

    private void ResetBurningTimerProgress()
    {
        burningTimerProgress = 0;

        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = 0
        });
    }
}
