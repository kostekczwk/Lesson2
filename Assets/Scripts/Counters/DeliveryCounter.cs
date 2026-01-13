using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // Only accept plates
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                //plateKitchenObject.DestroySelf();
                player.GetKitchenObject().DestroySelf();
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        return;
    }
}
