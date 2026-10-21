using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
 //   [SerializeField] private string gameSceneName = "";
    [SerializeField] private GameObject settingsPanel;

    public void Play()
    {
//        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}