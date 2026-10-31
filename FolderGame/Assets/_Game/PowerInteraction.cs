using System.Collections;
using UnityEngine;

public class PowerInteraction : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private bool isUsed;
    
    [Header("Block Visual")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    
    [SerializeField] private SpriteRenderer renderer;
    private Material currentMaterial;
    
    private IEnumerator Start()
    {
        currentMaterial = new Material(hitMaterial);
        renderer = GetComponent<SpriteRenderer>();
        
        if (renderer != null) 
            renderer.material = currentMaterial;

        yield return null;
    }

    private void OnEnable()
    {
        StartCoroutine(FlashCoroutine());
    }
    
    private IEnumerator FlashCoroutine()
    {
        float currentFlashAmount = 1;
        float elapsedTime = 0;
        
        yield return null;
        
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(0, flashCurve.Evaluate(elapsedTime), elapsedTime / flashTime);
            PlayFlash(currentFlashAmount);
            yield return null;
        }
    }
    
    private void PlayFlash(float amount)
    {
        currentMaterial.SetFloat("_HitEffectBlend", amount);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();
            if (controller != null)   controller.DeactivatePlayer();
            
            InventoryManager.Instance.SaveInventory();
            CameraManager.Instance.ShakeCameraIncreasing(0.5f, 0.5f);
            LoadManager.Instance.OpenScene(nextSceneName);
            isUsed = true;
        }
    }
}
