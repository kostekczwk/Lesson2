using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in this.transform)
        {
            if (child == iconTemplate)
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO item in plateKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconTemplateCopyTransform = Instantiate(iconTemplate, this.transform);

            iconTemplateCopyTransform.gameObject.SetActive(true);
            iconTemplateCopyTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(item);
        }
    }
}
