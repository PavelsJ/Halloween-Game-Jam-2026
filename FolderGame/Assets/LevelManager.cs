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
    
    [Header("Spawn Rate")]
    [SerializeField] private float spawnRateStartDelay = 3f;
    [SerializeField] private float spawnRateIncreaseDuration = 2f;
    [SerializeField] private float minSpawnDelay = 0.01f;
    [SerializeField] private AnimationCurve spawnRateCurve;
    
    private float levelStartTime;

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
        levelStartTime = Time.time;
        
        if (firstPlatform != null)
        {
            BlockController firstController = firstPlatform.GetComponent<BlockController>();
            if (firstController != null) yield return StartCoroutine(SpawnPlatform(firstController));
        }
        
        if (platforms != null && platforms.Length > 0)
        {
            List<GameObject> shuffledPlatforms = new List<GameObject>(platforms);

            for (int i = 0; i < shuffledPlatforms.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledPlatforms.Count);

                (shuffledPlatforms[i], shuffledPlatforms[randomIndex]) = (shuffledPlatforms[randomIndex], shuffledPlatforms[i]);
            }
        
            foreach (GameObject platform in shuffledPlatforms)
            {
                yield return new WaitForSeconds(GetCurrentSpawnDelay());
                
                BlockController blockController = platform.GetComponent<BlockController>();
                if (blockController != null) StartCoroutine(SpawnPlatform(blockController));
            }
        }

        if (objects != null && objects.Length > 0)
        {
            List<GameObject> shuffledObjects = new List<GameObject>(objects);

            for (int i = 0; i <  shuffledObjects .Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledObjects.Count);

                ( shuffledObjects[i],  shuffledObjects[randomIndex]) = ( shuffledObjects[randomIndex], shuffledObjects[i]);
            }
        
            foreach (GameObject obj in shuffledObjects)
            {
                yield return new WaitForSeconds(GetCurrentSpawnDelay());
                StartCoroutine(SpawnObject(obj));
            }
        }
    }
    
    private float GetCurrentSpawnDelay()
    {
        float elapsed = Time.time - levelStartTime - spawnRateStartDelay;
        if (elapsed <= 0f) return spawnDelay;

        float progress = Mathf.Clamp01(elapsed / spawnRateIncreaseDuration);
        float curveValue = spawnRateCurve.Evaluate(progress);

        return Mathf.Lerp(spawnDelay, minSpawnDelay, curveValue);
    }

    private IEnumerator SpawnPlatform(BlockController platform)
    {
        if (platform == null) yield break;
        MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Spawn, Random.Range(0.8f, 1.2f));
        
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
        
        CameraManager.Instance.ShakeCamera(0.1f, 0.1f);
    }
    
    private IEnumerator SpawnObject(GameObject obj)
    {
        if (obj == null) yield break;
        MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Spawn, Random.Range(0.8f, 1.2f));
        
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
        MotionEffect motion = obj.GetComponent<MotionEffect>();
        if (motion != null) motion.StartMotion();
    }
}
