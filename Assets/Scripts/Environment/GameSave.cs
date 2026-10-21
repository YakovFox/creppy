using UnityEngine;

public class GameSave : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float saveInterval = 5f;

    private float timer;

    private const string HasSaveKey = "HasGameSave";
    private const string PosXKey = "PlayerPosX";
    private const string PosYKey = "PlayerPosY";
    private const string PosZKey = "PlayerPosZ";
    private const string HealthKey = "PlayerHealth";

    private void Start()
    {
        if (PlayerPrefs.GetInt(HasSaveKey, 0) == 1)
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat(PosXKey),
                PlayerPrefs.GetFloat(PosYKey),
                PlayerPrefs.GetFloat(PosZKey)
            );

            if (playerHealth != null)
                playerHealth.SetHealth(PlayerPrefs.GetFloat(HealthKey, 100f));
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= saveInterval)
        {
            Save();
            timer = 0f;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt(HasSaveKey, 1);

        PlayerPrefs.SetFloat(PosXKey, transform.position.x);
        PlayerPrefs.SetFloat(PosYKey, transform.position.y);
        PlayerPrefs.SetFloat(PosZKey, transform.position.z);

        if (playerHealth != null)
            PlayerPrefs.SetFloat(HealthKey, playerHealth.CurrentHealth);

        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(HasSaveKey);
        PlayerPrefs.DeleteKey(PosXKey);
        PlayerPrefs.DeleteKey(PosYKey);
        PlayerPrefs.DeleteKey(PosZKey);
        PlayerPrefs.DeleteKey(HealthKey);
        PlayerPrefs.Save();
    }
}