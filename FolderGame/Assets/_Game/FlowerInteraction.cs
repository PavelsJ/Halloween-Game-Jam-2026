using System.Collections;
using UnityEngine;

public class FlowerInteraction : MonoBehaviour
{
    [SerializeField] private ItemData flowerData;
    [SerializeField] private GameObject hitParticle;
    private bool isUsed;
    
    [Header("Block Visual")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    
    private SpriteRenderer renderer;
    private Material currentMaterial;
    
    private IEnumerator Start()
    {
        currentMaterial = new Material(hitMaterial);
        renderer = GetComponent<SpriteRenderer>();
        
        if (renderer != null) 
            renderer.material = currentMaterial;
        
        yield return new WaitForSeconds(0.1f);
        
        if (flowerData == null || InventoryManager.Instance == null) yield break;

        foreach (ItemData item in InventoryManager.Instance.Items)
        {
            if (item != null && item.ItemID == flowerData.ItemID)
            {
                isUsed = true;
                gameObject.SetActive(false);
                yield break;
            }
        }
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
            InventoryManager.Instance.AddItem(flowerData);
            isUsed = true;

            StartCoroutine(MoveToTarget(other.transform));
        }
    }
    private IEnumerator MoveToTarget(Transform target)
    {
        Vector3 startPos = transform.position;
        Vector3 jumpPeak = startPos + new Vector3(0f, 0.2f, 0f);

        float jumpDuration = 0.3f;
        float fallDuration = 0.4f;
        float totalDuration = jumpDuration + fallDuration;

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / jumpDuration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            transform.position = Vector3.Lerp(startPos, jumpPeak, easedT);
            PlayAlpha(1f - Mathf.Clamp01(elapsed / totalDuration));
            PlayFlash(1f - Mathf.Clamp01(elapsed / totalDuration));

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fallDuration);
            float easedT = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

            transform.position = Vector3.Lerp(jumpPeak, target.position, easedT);
            PlayAlpha(1f - Mathf.Clamp01((jumpDuration + elapsed) / totalDuration));
            PlayFlash(1f - Mathf.Clamp01((jumpDuration + elapsed) / totalDuration));
            
            yield return null;
        }
        
        GameObject particle = Instantiate(hitParticle, transform.position, Quaternion.identity, null);
        particle.GetComponent<ParticleSystem>().Play();
        Destroy(particle, 2f);

        PlayAlpha(0f);
        PlayFlash(0f);
        
        gameObject.SetActive(false);
    }
    
    private void PlayAlpha(float amount)
    {
        currentMaterial.SetFloat("_AlphaEffectBlend", amount);
    }
}
