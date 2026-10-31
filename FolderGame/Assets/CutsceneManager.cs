using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flowerPrefab;
    [SerializeField] private MilfController milfController;
    
    [Header("Player Movement")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private float playerMoveSpeed = 5f;
    
    [Header("Player Flash")]
    [SerializeField] private float cutsceneFlashAmount = 1f;
    [SerializeField] private float cutsceneFlashDuration = 1.5f;
    
    private Coroutine flashTransition;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 1.5f;
    [SerializeField] private float orbitDuration = 2f;
    [SerializeField] private float orbitExpandSpeed = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float spawnDelay = 0.2f;

    [Header("Attack Settings")]
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float flyAcceleration = 8f;
    [SerializeField] private float arrivalDistance = 0.15f;
    [SerializeField] private float attackDelay = 0.1f;
    
    private PlayerController playerController;
    private Transform playerTransform;

    private readonly List<GameObject> spawnedFlowers = new();
    private Dictionary<GameObject, float> flowerStartDistances = new();
    private readonly List<float> flowerAngles = new();

    private float currentRotation;
    private bool cutsceneStarted;
    private bool lockPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (cutsceneStarted || !other.CompareTag("Player")) return;

      playerController = other.GetComponent<PlayerController>();
      if (playerController == null) return;
      
      playerController.DeactivatePlayer();
      playerTransform = other.transform;

      cutsceneStarted = true;
      lockPlayer =  true;

      StartCoroutine(PlayCutscene());
    }

    private void Update()
    {
        if (!lockPlayer) return;
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos.position, playerMoveSpeed * Time.deltaTime);
    }
   
    private IEnumerator PlayCutscene()
    {
        int flowerCount = InventoryManager.Instance.Items.Count;

        if (flowerCount <= 0)
        {
            milfController.StartBoss();
            
            if (playerController != null)
            {
                playerController.ActivatePlayer();
            }
            
            yield break;
        }
        
        playerController.Visual.SetCutsceneFlash(true);
        CameraManager.Instance.ShakeCameraIncreasing(2.5f, 0.5f);
        MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Tension, Random.Range(0.9f, 1.1f));
        MusicManager.instance.StopMusic();

        currentRotation = 0f;

        // Spawn flowers
        for (int i = 0; i < flowerCount; i++)
        {
            GameObject flower = Instantiate(flowerPrefab, playerTransform.position, Quaternion.identity);
            MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Jump);

            spawnedFlowers.Add(flower);

            float angle = (360f / flowerCount) * i;
            flowerAngles.Add(angle);

            yield return new WaitForSeconds(spawnDelay);
        }

        // Expand orbit
        float expandProgress = 0f;

        while (expandProgress < 1f)
        {
            float deltaTime = Time.deltaTime;

            expandProgress += orbitExpandSpeed * deltaTime / orbitRadius;
            currentRotation += rotationSpeed * deltaTime;

            for (int i = 0; i < spawnedFlowers.Count; i++)
            {
                if (spawnedFlowers[i] == null) continue;

                float angle = flowerAngles[i] + currentRotation;
                float radians = angle * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * (orbitRadius * Mathf.Clamp01(expandProgress));

                spawnedFlowers[i].transform.position = playerTransform.position + offset;
            }

            yield return null;
        }

        // Orbit
        float elapsed = 0f;

        while (elapsed < orbitDuration)
        {
            float deltaTime = Time.deltaTime;

            elapsed += deltaTime;
            currentRotation += rotationSpeed * deltaTime;

            UpdateFlowerOrbit();

            yield return null;
        }

        if (milfController != null)
        {
            List<GameObject> flyingFlowers = new();
            List<float> flowerSpeeds = new();

            lockPlayer = false;

            if (playerController != null)
            {
                playerController.Visual.SetCutsceneFlash(false);
                playerController.ActivatePlayer();
            }
            
            // Start sending flowers
            for (int i = 0; i < spawnedFlowers.Count; i++)
            {
                GameObject flower = spawnedFlowers[i];

                if (flower == null) continue;

                spawnedFlowers[i] = null;

                flyingFlowers.Add(flower);
                flowerSpeeds.Add(0f);

                flowerStartDistances[flower] = Vector2.Distance(flower.transform.position, milfController.transform.position);

                CameraManager.Instance.ShakeCamera(0.1f, 0.2f);
                MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Attack);

                float delayElapsed = 0f;

                while (delayElapsed < attackDelay)
                {
                    float deltaTime = Time.deltaTime;

                    delayElapsed += deltaTime;
                    currentRotation += rotationSpeed * deltaTime;

                    UpdateFlowerOrbit();

                    // Update ALL flying flowers
                    for (int j = flyingFlowers.Count - 1; j >= 0; j--)
                    {
                        GameObject flyingFlower = flyingFlowers[j];

                        if (flyingFlower == null)
                        {
                            flyingFlowers.RemoveAt(j);
                            flowerSpeeds.RemoveAt(j);
                            continue;
                        }

                        flowerSpeeds[j] = Mathf.MoveTowards(flowerSpeeds[j], flySpeed, flyAcceleration * deltaTime);
                        flyingFlower.transform.position = Vector3.MoveTowards(flyingFlower.transform.position, milfController.transform.position, flowerSpeeds[j] * deltaTime);
                        float distance = Vector2.Distance(flyingFlower.transform.position, milfController.transform.position);

                        // Update alpha independently for each flower
                        if (flowerStartDistances.TryGetValue(flyingFlower, out float startDistance))
                        {
                            float alpha = Mathf.Clamp01(distance / startDistance);

                            flyingFlower.GetComponent<HitEffect>()?.PlayAlpha(alpha);
                        }

                        if (distance <= arrivalDistance)
                        {
                            flyingFlower.GetComponent<HitEffect>()?.PlayParticles();

                            InventoryManager.Instance.RemoveItem();
                            milfController.ReceiveFlover();

                            flowerStartDistances.Remove(flyingFlower);

                            Destroy(flyingFlower);

                            flyingFlowers.RemoveAt(j);
                            flowerSpeeds.RemoveAt(j);
                        }
                    }

                    yield return null;
                }
            }

            // Continue flying until every flower arrives
            while (flyingFlowers.Count > 0)
            {
                float deltaTime = Time.deltaTime;
                Vector3 target = milfController.transform.position;

                for (int i = flyingFlowers.Count - 1; i >= 0; i--)
                {
                    GameObject flower = flyingFlowers[i];

                    if (flower == null)
                    {
                        flyingFlowers.RemoveAt(i);
                        flowerSpeeds.RemoveAt(i);
                        continue;
                    }

                    flowerSpeeds[i] = Mathf.MoveTowards(flowerSpeeds[i], flySpeed, flyAcceleration * deltaTime);
                    flower.transform.position = Vector3.MoveTowards(flower.transform.position, target, flowerSpeeds[i] * deltaTime);
                    float distance = Vector2.Distance(flower.transform.position, target);

                    if (flowerStartDistances.TryGetValue(flower, out float startDistance))
                    {
                        float alpha = Mathf.Clamp01(distance / startDistance);
                        flower.GetComponent<HitEffect>()?.PlayAlpha(alpha);
                    }

                    if (distance <= arrivalDistance)
                    {
                        flower.GetComponent<HitEffect>()?.PlayParticles();

                        InventoryManager.Instance.RemoveItem();
                        milfController.ReceiveFlover();

                        flowerStartDistances.Remove(flower);

                        Destroy(flower);

                        flyingFlowers.RemoveAt(i);
                        flowerSpeeds.RemoveAt(i);
                    }
                }

                yield return null;
            }
        }

        spawnedFlowers.Clear();
        flowerAngles.Clear();
        flowerStartDistances.Clear();
    }

    private void UpdateFlowerOrbit()
    {
        if (playerTransform == null)
            return;

        for (int i = 0; i < spawnedFlowers.Count; i++)
        {
            if (spawnedFlowers[i] == null) continue;

            float angle = flowerAngles[i] + currentRotation;
            Vector3 offset = Quaternion.Euler(0f, 0f, angle) * Vector3.up * orbitRadius;
            spawnedFlowers[i].transform.position = playerTransform.position + offset;
        }
    }
}
