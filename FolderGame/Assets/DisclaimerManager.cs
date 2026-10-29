using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisclaimerManager : MonoBehaviour
{
    [Header("Preload Transition")]
    [SerializeField] private float stayDuration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Image fadeImage;
    
    private bool isLoading = false;
    
    private const string DisclaimerSaveKey = "Disclaimer";

    private void Awake()
    {
        bool hasHey = PlayerPrefs.HasKey(DisclaimerSaveKey);
        
        if (fadeImage != null)
            fadeImage.gameObject.SetActive(!hasHey);
    }
    
    private void Start()
    {
        if (fadeImage != null && fadeImage.gameObject.activeSelf)
            StartCoroutine(AutoFadeOut());
    }
    
    private IEnumerator AutoFadeOut()
    {
        yield return new WaitForSeconds(stayDuration);

        if (!isLoading && fadeImage != null && fadeImage.gameObject.activeSelf)
        {
            StartCoroutine(OnFadeOut());
            PlayerPrefs.SetInt(DisclaimerSaveKey, 1);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown && !isLoading)
        {
            StartCoroutine(OnFadeOut());
            PlayerPrefs.SetInt(DisclaimerSaveKey, 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator OnFadeOut()
    {
        if (fadeImage == null)
        {
            isLoading = false;
            yield break;
        }
        
        isLoading = true;
        
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
    }
}
