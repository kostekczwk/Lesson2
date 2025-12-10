using UnityEngine;
using UnityEngine.UI;
using static CuttingCounter;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private CuttingCounter cuttingCounter;

    public void Start()
    {
        cuttingCounter.OnProgressChanged += CuttingCounter_OnProgressChanged;
        this.barImage.fillAmount = 0;
        Hide();
    }

    private void CuttingCounter_OnProgressChanged(object sender, OnProgressChangedEventArgs e)
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
