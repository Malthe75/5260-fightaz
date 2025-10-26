using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTwo : MonoBehaviour
{
    public static AudioManagerTwo Instance;
    private AudioSource oneShotSource;
    [Range(0f, 1f)] public float defaultVolume = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        oneShotSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip, float pitchVariance = 0.05f, float volume = 0.5f, float fadeOutTime = 0.1f)
    {
        if (clip == null) return;

        // Create a temp source for independent volume control
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        src.volume = volume;
        src.PlayOneShot(clip);

        // Start fade-out coroutine
        StartCoroutine(FadeOutAndDestroy(src, fadeOutTime, clip.length));
    }

    private IEnumerator FadeOutAndDestroy(AudioSource src, float fadeTime, float clipLength)
    {
        yield return new WaitForSecondsRealtime(clipLength - fadeTime);

        float startVol = src.volume;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }

        Destroy(src);
    }
}