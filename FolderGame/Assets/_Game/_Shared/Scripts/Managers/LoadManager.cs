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
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearAllSaves();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OpenPreviousScene();
        }

        // 2 — следующая сцена
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OpenNextScene();
        }
        
        totalTime += Time.deltaTime;
    }

    public void OpenScene(string scene)
    {
        SaveTime();
        
        loadController.LoadScene(scene);
    }

    public void OpenPreviousScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (sceneCount <= 1) return;

        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousSceneIndex < 0) previousSceneIndex = sceneCount - 1;

        SaveTime();

        string previousScene = SceneUtility.GetScenePathByBuildIndex(previousSceneIndex);
        loadController.LoadScene(previousScene);
    }

    public void OpenNextScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (sceneCount <= 1) return;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= sceneCount) nextSceneIndex = 0;

        SaveTime();

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




