using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }
    
    [Header("Preload Transition")]
    [SerializeField] LoadSceneController loadController;
    
    private float totalTime;
    private const string totalTimeKey = "TotalTime";
    
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
        
        totalTime = PlayerPrefs.GetFloat(totalTimeKey, 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearAllSaves();
        }
        
        totalTime += Time.deltaTime;
    }
    
    public void StartGame()
    {
        OpenScene("Tagged");
    }

    public void OpenScene(string scene)
    {
        SaveTime();
        
        loadController.LoadScene(scene);
    }

    public void OpenNextScene()
    {
        SaveTime();
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings) return;
        
        string nextScene = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        loadController.LoadScene(nextScene);
    }

    public void RestartGame()
    {
        SaveTime();
        
        string currentScene = SceneManager.GetActiveScene().name;
        loadController.LoadScene(currentScene);
    }
    
    public void QuitGame()
    {
        SaveTime();
        
        Debug.Log("Quit button clicked! Exiting game...");
        Application.Quit();
    }
    
    public void ClearAllSaves()
    {
        PlayerPrefs.DeleteAll();
    }
    
    private void SaveTime()
    {
        PlayerPrefs.SetFloat(totalTimeKey, totalTime);
        PlayerPrefs.Save();
    }
}




