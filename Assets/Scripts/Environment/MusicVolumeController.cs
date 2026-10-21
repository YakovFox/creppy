using UnityEngine;

public class MusicVolumeController : MonoBehaviour
{
    private static MusicVolumeController activeController;
    private const string VolumeKey = "MusicVolume";

    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        activeController = this;
        ApplySavedVolume();
    }

    private void OnDestroy()
    {
        if (activeController == this)
            activeController = null;
    }

    private void ApplySavedVolume()
    {
        if (musicSource == null) return;

        musicSource.volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
    }

    public static void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();

        if (activeController != null && activeController.musicSource != null)
            activeController.musicSource.volume = volume;
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(VolumeKey, 1f);
    }
}
