using System;
using System.Collections;
using UnityEngine;

public class SmoothReparenting : MonoBehaviour
{
    public event EventHandler OnReparentingCompleted;

    [SerializeField] float duration = 0.1f;

    public void InstantReparenting(Transform newParentTransform)
    {
        transform.SetParent(newParentTransform, true);
        transform.localPosition = Vector3.zero;
    }

    public void StartSmoothReparenting(Transform newParentTransform)
    {
        Coroutine coroutine = StartCoroutine(SmoothReparentingRoutine(newParentTransform));
    }

    private IEnumerator SmoothReparentingRoutine(Transform newParentTransform)
    {
        float timeElapsed = 0f;

        Vector3 transformPosition = transform.position;
        Vector3 targetTransformPosition = newParentTransform.position;

        // Loop until the transition duration is reached
        while (timeElapsed < duration)
        {
            // Calculate the interpolation factor (0 at start, 1 at end)
            float t = timeElapsed / duration;

            transform.position = Vector3.Lerp(transformPosition, targetTransformPosition, t);

            // Increment time and wait for the next frame
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        InstantReparenting(newParentTransform);

        OnReparentingCompleted?.Invoke(this, EventArgs.Empty);
    }
}
