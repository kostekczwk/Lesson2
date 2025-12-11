using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        switch (e.State)
        {
            default:
            case StoveCounter.State.Idle:
                stoveOnGameObject.SetActive(false);
                particlesGameObject.SetActive(false);
                break;
            case StoveCounter.State.Frying:
            case StoveCounter.State.Fried:
                stoveOnGameObject.SetActive(true);
                particlesGameObject.SetActive(true);
                break;
            case StoveCounter.State.Burned:
                stoveOnGameObject.SetActive(true);
                particlesGameObject.SetActive(false);
                break;
        }
    }
}
