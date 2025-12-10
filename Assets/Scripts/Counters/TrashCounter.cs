using UnityEngine;

public class TrashCounter : BaseCounter
{
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
