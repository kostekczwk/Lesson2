using System;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            return;
        }

        // Set parent before destroy to handle animation?
        KitchenObject kitchenObject = player.GetKitchenObject();
        kitchenObject.OnReparentingCompleted += KitchenObject_OnReparentingCompleted;
        kitchenObject.SetKitchenObjectParent(this);

        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }

    private void KitchenObject_OnReparentingCompleted(object sender, System.EventArgs e)
    {
        // Destroy after change parent to TrashCounter
        KitchenObject kitchenObject = this.GetKitchenObject();
        kitchenObject.DestroySelf();
    }

    public override void InteractAlternate(Player player)
    {
        return;
    }
}
