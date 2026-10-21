using UnityEngine;

public class DifficultySettings : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public static DifficultySettings Instance { get; private set; }

    [SerializeField] private Difficulty difficulty = Difficulty.Medium;

    public float EnemyDamageMultiplier
    {
        get
        {
            switch (difficulty)
            {
                case Difficulty.Easy: return 0.5f;
                case Difficulty.Hard: return 1.5f;
                default: return 1f;
            }
        }
    }

    public int CurrentDifficulty => (int)difficulty;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        difficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty", (int)difficulty);
    }

    public void SetDifficulty(int value)
    {
        difficulty = (Difficulty)Mathf.Clamp(value, 0, 2);
        PlayerPrefs.SetInt("Difficulty", (int)difficulty);
        PlayerPrefs.Save();
    }
}