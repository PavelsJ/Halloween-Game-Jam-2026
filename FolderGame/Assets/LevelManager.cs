using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] bool spawnOnStart = true;
    [SerializeField] private GameObject firstPlatform;
    [SerializeField] private GameObject [] platforms;

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
       
        yield return StartCoroutine(SpawnPlatform(firstPlatform.GetComponent<BlockController>()));
        
        List<GameObject> shuffledPlatforms = new List<GameObject>(platforms);

        for (int i = 0; i < shuffledPlatforms.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPlatforms.Count);

            (shuffledPlatforms[i], shuffledPlatforms[randomIndex]) = (shuffledPlatforms[randomIndex], shuffledPlatforms[i]);
        }
        
        foreach (GameObject platform in shuffledPlatforms)
        {
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(SpawnPlatform(platform.GetComponent<BlockController>()));
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
    }
}
