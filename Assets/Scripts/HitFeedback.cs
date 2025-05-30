using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFeedback : MonoBehaviour
{
    public Camera mainCam;
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.2f;
    public float hitStopTime = 0.1f;

    public void TriggerHitEffect()
    {
        StartCoroutine(HitStopWithShake(hitStopTime, shakeDuration, shakeMagnitude));
    }

    private IEnumerator HitStopWithShake(float stopTime, float shakeDur, float magnitude)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.05f;

        // Start shaking in unscaled time
        StartCoroutine(ScreenShake(shakeDur, magnitude));

        yield return new WaitForSecondsRealtime(stopTime);

        Time.timeScale = originalTimeScale;
    }

    private IEnumerator ScreenShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCam.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCam.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.unscaledDeltaTime; // Use unscaled time during time freeze

            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }
}


