using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipiesDeliveredText;

    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManeger_OnStateChange;

        Hide();
    }

    private void GameManeger_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOverActive())
        {
            recipiesDeliveredText.text = DeliveryManager.Instance.GetSuccessRecipiesAmount().ToString();

            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
