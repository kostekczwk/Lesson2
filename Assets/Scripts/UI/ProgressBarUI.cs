using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;

    private IHasProgress hasProgress;

    public void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress is null )
        {
            MyDebug.Error($"{hasProgressGameObject} not implement IHasProgress");
        }

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

        this.barImage.fillAmount = 0;
        Hide();
    }

    private void HasProgress_OnProgressChanged(object sender, OnProgressChangedEventArgs e)
    {
        this.barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized >= 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
    }
}
