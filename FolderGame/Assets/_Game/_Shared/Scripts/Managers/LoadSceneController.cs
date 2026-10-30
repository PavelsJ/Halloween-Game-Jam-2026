using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Random = UnityEngine.Random;

public class LoadSceneController : MonoBehaviour
{
    [Header("Preload Transition")]
    [SerializeField] private bool fadeOnAwake = false;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] public Image fadeImage;
    
    [Header("Update Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image progressBar;
    
    [Header("Loading Settings")]
    [SerializeField] private float minimumLoadingTime = 2f;
    [SerializeField] private float progressSpeed = 30f;

    private AsyncOperation preloadOperation;
    private bool isLoading = false;
    
    private readonly string[] updateMessages =
    {
        "Preparing system updates.",
        "Installing critical updates.",
        "Disabling Security.",
        "Configuring system components.",
        "Deploying Trust_Me_Folder",
        "Optimizing system performance.",
        "Downloading League of Legends.",
        "Updating system files.",
        "Preparing your PC for restart.",
        "Uploading Free Antivirus.exe",
        "Applying important changes.",
        "Restarting system services.",
        "Updating your system registry.",
        "HEHhhEheHEHeeHE",
        "Almost done."
    };

    private readonly string loadingMessage = "Please do not turn off your computer.";
    
    private void Awake()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
        
        MusicManager.instance.StartMusic();

        if (fadeOnAwake)
        {
            isLoading = true;
            StartCoroutine(OnFadeOut());
        }
    }
    
    public bool CanLoadScene(string sceneName)
    {
        return !isLoading && Application.CanStreamedLevelBeLoaded(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        if (!CanLoadScene(sceneName)) return;
        isLoading = true;
        
        // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonPress);
        StartCoroutine(PreloadAndTransition(sceneName));
    }
   
    private IEnumerator PreloadAndTransition(string scene)
    {
        if (preloadOperation != null) yield break;
        
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return OnFadeIn();
        }
        
        if (loadingScreen != null) 
            loadingScreen.SetActive(true);
        
        if (statusText != null)
        {
            string randomText = $"{updateMessages[Random.Range(0, updateMessages.Length)]}\n{loadingMessage}";
            statusText.text = randomText;
        }
        
        SetProgress(0);
        
        preloadOperation = SceneManager.LoadSceneAsync(scene);

        if (preloadOperation == null)
        {
            Debug.LogError($"Failed to load scene: {scene}");
            isLoading = false;
            yield break;
        }
        
        preloadOperation.allowSceneActivation = false;

        float elapsedTime = 0f;
        float displayedProgress = 0f;

        while (preloadOperation.progress < 0.9f || elapsedTime < minimumLoadingTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float actualProgress = Mathf.Clamp01(preloadOperation.progress / 0.9f);
            float targetProgress = actualProgress * 99f;
            
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, progressSpeed * Time.unscaledDeltaTime);

            SetProgress(Mathf.Min(displayedProgress, 99f));
            
            if (statusText != null && Random.value < 0.002f)
            {
                string randomText = $"{updateMessages[Random.Range(0, updateMessages.Length)]}\n{loadingMessage}";
                statusText.text = randomText;
            }

            yield return null;
        }
        
        SetProgress(100);

        if (statusText != null) 
            statusText.text = "Restarting your computer...";
        
        yield return new WaitForSecondsRealtime(0.5f);
        preloadOperation.allowSceneActivation = true;
    }
    
    private void SetProgress(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);

        if (progressText != null)
            progressText.text = $"{Mathf.RoundToInt(value)}% complete";

        if (progressBar != null)
            progressBar.fillAmount = value / 100f;
    }
    
    private IEnumerator OnFadeIn()
    {
        if (fadeImage == null) yield break;
        
        CanvasGroup canvasGroup = fadeImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;
        
        float alpha = canvasGroup.alpha;
        
        for (float t = 0; t <= fadeDuration; t += Time.deltaTime)
        {
            alpha = t / fadeDuration;
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator OnFadeOut()
    {
        if (fadeImage == null)
        {
            isLoading = false;
            yield break;
        }
        
        fadeImage.gameObject.SetActive(true);
        
        CanvasGroup canvasGroup = fadeImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;
        
        float alpha = canvasGroup.alpha;

        for (float t = fadeDuration; t >= 0; t -= Time.deltaTime)
        {
            alpha = t / fadeDuration;
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        
        fadeImage.gameObject.SetActive(false);
        
        isLoading = false;
    }
}