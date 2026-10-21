using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Dropdown difficultyDropdown;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.SetValueWithoutNotify(CombatMusicManager.GetMusicVolume());
            volumeSlider.onValueChanged.AddListener(CombatMusicManager.SetMusicVolume);
        }

        if (difficultyDropdown != null)
        {
            int difficulty = PlayerPrefs.GetInt("Difficulty", 1);
            difficultyDropdown.SetValueWithoutNotify(difficulty);
            difficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void SetDifficulty(int difficulty)
    {
        if (DifficultySettings.Instance != null)
            DifficultySettings.Instance.SetDifficulty(difficulty);
        else
        {
            PlayerPrefs.SetInt("Difficulty", difficulty);
            PlayerPrefs.Save();
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
