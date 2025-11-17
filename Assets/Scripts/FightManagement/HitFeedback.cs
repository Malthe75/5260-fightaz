using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFeedback : MonoBehaviour
{
    public Camera mainCam;
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.2f;
    public float hitStopTime = 0.1f;
    public static HitFeedback Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void TriggerHitEffect()
    {
        StartCoroutine(HitStopWithShake(hitStopTime, shakeDuration, shakeMagnitude));
    }

    private IEnumerator HitStopWithShake(float stopTime, float shakeDur, float magnitude)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.05f;

        StartCoroutine(ScreenShake(shakeDur, magnitude));

        yield return new WaitForSecondsRealtime(stopTime);

        // Smoothly return timescale to normal
        float restoreDuration = 0.05f;
        float t = 0;
        while (t < restoreDuration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0.05f, originalTimeScale, t / restoreDuration);
            yield return null;
        }

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


