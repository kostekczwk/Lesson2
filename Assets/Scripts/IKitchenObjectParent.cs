using UnityEngine;

public interface IKitchenObjectParent
{
    Transform GetKitchenObjectDestinationTransform();

    void SetKitchenObject(KitchenObject kitchenObject);

    KitchenObject GetKitchenObject();

    void ClearKitchenObject();

    bool HasKitchenObject();
}
