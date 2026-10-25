using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSceneController : MonoBehaviour
{
    [Header("Preload Transition")]
    [SerializeField] private bool fadeOnAwake = false;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] public Image fadeImage; 
    
    private AsyncOperation preloadOperation; 
    private bool isLoading = false;
    
    private void Awake()
    {
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
        
        fadeImage.gameObject.SetActive(true);
        yield return OnFadeIn();
        
        preloadOperation = SceneManager.LoadSceneAsync(scene);
        if (preloadOperation == null) yield break;
        preloadOperation.allowSceneActivation = false;

        while (preloadOperation.progress < 0.9f)
            yield return null;

        preloadOperation.allowSceneActivation = true;
    }
    
    private IEnumerator OnFadeIn()
    {
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