using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMusicManager : MonoBehaviour
{
    private static CombatMusicManager instance;
    private static readonly HashSet<EnemyAI> alertEnemies = new();

    [SerializeField] private AudioSource firstSource;
    [SerializeField] private AudioSource secondSource;
    [SerializeField] private AudioClip calmTrack;
    [SerializeField] private AudioClip alertTrack;
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource activeSource;
    private Coroutine fadeCoroutine;
    private bool alertPlaying;
    private const string VolumeKey = "MusicVolume";

    private void Awake()
    {
        instance = this;

        float volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        firstSource.volume = volume;
        secondSource.volume = volume;

        activeSource = firstSource;
        activeSource.clip = calmTrack;
        activeSource.loop = true;
        activeSource.Play();
        alertPlaying = false;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();

        if (instance != null)
        {
            instance.firstSource.volume = volume;
            instance.secondSource.volume = volume;
        }
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(VolumeKey, 1f);
    }

    public static void SetEnemyAlert(EnemyAI enemy, bool isAlert)
    {
        if (enemy == null) return;

        if (isAlert)
            alertEnemies.Add(enemy);
        else
            alertEnemies.Remove(enemy);

        if (instance != null)
            instance.UpdateTrack(alertEnemies.Count > 0);
    }

    private void UpdateTrack(bool playAlert)
    {
        if (alertPlaying == playAlert) return;

        alertPlaying = playAlert;
        AudioClip nextClip = playAlert ? alertTrack : calmTrack;

        if (nextClip == null || activeSource.clip == nextClip) return;

        AudioSource nextSource = activeSource == firstSource
            ? secondSource
            : firstSource;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Crossfade(nextSource, nextClip));
    }

    private IEnumerator Crossfade(AudioSource nextSource, AudioClip nextClip)
    {
        float volume = GetMusicVolume();
        nextSource.clip = nextClip;
        nextSource.loop = true;
        nextSource.volume = 0f;
        nextSource.Play();

        float elapsed = 0f;
        AudioSource previousSource = activeSource;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            nextSource.volume = volume * t;
            previousSource.volume = volume * (1f - t);

            yield return null;
        }

        previousSource.Stop();
        previousSource.volume = volume;
        nextSource.volume = volume;
        activeSource = nextSource;
        fadeCoroutine = null;
    }
}