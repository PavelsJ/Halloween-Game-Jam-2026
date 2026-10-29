using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] bool spawnOnStart = true;
    [SerializeField] private GameObject firstPlatform;
    [SerializeField] private GameObject [] platforms;
    [SerializeField] private GameObject [] objects;

    [Header("Spawn Interactions")]
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float spawnDelay = 0.1f;
    [SerializeField] private float returnDuration = 0.5f;

    private void Awake()
    {
        firstPlatform.SetActive(false);

        foreach (GameObject platform in platforms)
        {
            platform.SetActive(false);
        }

        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    private void Start()
    {
        if (spawnOnStart)
            StartLevel();
    }

    public void StartLevel()
    {
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine()
    {
        BlockController firstController = firstPlatform.GetComponent<BlockController>();
        if (firstController != null) yield return StartCoroutine(SpawnPlatform(firstController));
        
        List<GameObject> shuffledPlatforms = new List<GameObject>(platforms);

        for (int i = 0; i < shuffledPlatforms.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPlatforms.Count);

            (shuffledPlatforms[i], shuffledPlatforms[randomIndex]) = (shuffledPlatforms[randomIndex], shuffledPlatforms[i]);
        }
        
        foreach (GameObject platform in shuffledPlatforms)
        {
            yield return new WaitForSeconds(spawnDelay);
            
            BlockController blockController = platform.GetComponent<BlockController>();
            if (blockController != null) StartCoroutine(SpawnPlatform(blockController));
        }
        
        foreach (GameObject obj in objects)
        {
            yield return new WaitForSeconds(spawnDelay);
            
            StartCoroutine(SpawnObject(obj));
        }
    }

    private IEnumerator SpawnPlatform(BlockController platform)
    {
        if (platform == null) yield break;
        
        Transform platformTransform = platform.transform;
        platform.GetGround().enabled = false;
     
        Vector3 targetPosition = platformTransform.position;
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 startPosition = targetPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

        platformTransform.position = startPosition;
        platform.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float easedT = t * t;

            platformTransform.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            yield return null;
        }
        
        platformTransform.position = targetPosition;
        platform.GetMotion().StartMotion();
        platform.GetGround().enabled = true;
        
        CameraManager.Instance.ShakeCamera(0.1f, 0.2f);
    }
    
    private IEnumerator SpawnObject(GameObject obj)
    {
        if (obj == null) yield break;
        
        Transform platformTransform = obj.transform;
     
        Vector3 targetPosition = platformTransform.position;
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 startPosition = targetPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

        platformTransform.position = startPosition;
        obj.SetActive(true);

        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float easedT = t * t;

            platformTransform.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            yield return null;
        }
        
        platformTransform.position = targetPosition;
        obj.GetComponent<MotionEffect>().StartMotion();
        
        CameraManager.Instance.ShakeCamera(0.1f, 0.2f);
    }
}
