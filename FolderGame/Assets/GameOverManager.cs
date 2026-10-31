using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    
    private readonly string[] updateMessages =
    {
        "//Lol, player died",
        "//System32 has been deleted",
        "//Deleting player.exe...",
        "//Installing skill_update.exe...",
        "//Deleting C:/Users/Player/",
        "//All your files are gone.",
        "//System failure. Blame yourself.",
        "//Uploading your browser history",
        "//Downloading more RAM.",
        "//C:/Player/NotFound",
        "//Your computer has a new owner.",
        "//MeHEHhhEheHEHeeHE",
        "//Installing Windows.exe",
        "//Error 404: Player not found.",
        "//The virus sends its regards.",
        "//Deleting player backup",
    };

    private readonly string loadingMessage = "Restart expected...";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        if(gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        MusicManager.instance.PlayRandomSound(MusicManager.SoundType.GameOver, Random.Range(0.8f, 1.2f));
        
        if (gameOverText != null)
            gameOverText.text = $"{updateMessages[Random.Range(0, updateMessages.Length)]} {loadingMessage}";
    }
}
