using System;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    public event EventHandler OnReparentingCompleted;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private SmoothReparenting smoothReparenting;
    private IKitchenObjectParent kitchenObjectParent;

    private void Awake()
    {
        smoothReparenting = GetComponent<SmoothReparenting>();
    }

    private void Start()
    {
        smoothReparenting.OnReparentingCompleted += SmoothReparenting_OnReparentingCompleted;
    }

    private void SmoothReparenting_OnReparentingCompleted(object sender, EventArgs e)
    {
        OnReparentingCompleted?.Invoke(this, e);
    }

    public KitchenObjectSO GetKitchenObjectSO() { return kitchenObjectSO; }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) 
    { 
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject())
        {
            MyDebug.Error("IKitchenObjectParent already has a KitchenObject!");
        }

        kitchenObjectParent.SetKitchenObject(this);

        Transform destinationTransform = kitchenObjectParent.GetKitchenObjectDestinationTransform();

        smoothReparenting.StartSmoothReparenting(destinationTransform);
        //smoothReparenting.InstantReparentin(destinationTransform);
    }

    public IKitchenObjectParent GetKitchenObjectParent() { return kitchenObjectParent; }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(this.gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform parentTransform = kitchenObjectParent.GetKitchenObjectDestinationTransform();
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, parentTransform.position, parentTransform.rotation);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
    }
}
