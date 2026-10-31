using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MilfController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] blocks;
   
    
    [Header("Boss Movement")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private float movementRadius = 5f;
    [SerializeField] private float minMoveDelay = 0.5f;
    [SerializeField] private float maxMoveDelay = 1.5f;
    
    private Vector3 movementTarget;
    
    [Header("Fireball Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float minAttackInterval = 1.5f;
    [SerializeField] private float maxAttackInterval = 3f;
    [SerializeField] private float fireballSpeed = 7f;
    [SerializeField] private float fireballLifetime = 5f;
    
    [Header("Platform Attack")]
    [SerializeField] private GameObject[] platforms;
    [SerializeField] private GameObject[] platformHiddenSpots;
    
    [SerializeField] private float minPlatformSwapDelay = 5f;
    [SerializeField] private float maxPlatformSwapDelay = 8f;
    
    [SerializeField] private float platformShakeDuration = 2f;
    [SerializeField] private float platformShakeMagnitude = 0.3f;

    private Coroutine platformCoroutine;
    
    [Header("Death")]
    [SerializeField] private Transform deathMovePos;

    [Header("Power Block")]
    [SerializeField] private GameObject powerBlock;
    [SerializeField] private Transform powerBlockMovePos;
    
    private int currentBossHealth;
    
    private Coroutine attackCoroutine;
    private Coroutine movementCoroutine;
    
    private int metersFromPlayer = 900;
    private int flowersCollected = 0;
    private const int maxFlowers = 8;

    private bool isActive = false;
    private MotionEffect motionEffect;
    
    private readonly Color[] randomColors =
    {
        Color.yellow,
        Color.green,
        Color.cyan
    };

    private void Start()
    {
        descriptionText.text = $"[{metersFromPlayer} meters from you]";
        currentBossHealth = metersFromPlayer;
        motionEffect = GetComponent<MotionEffect>();
    }

    public void ReceiveFlover()
    {
        if (isActive) return;
        if (flowersCollected >= maxFlowers) return;
        
        CameraManager.Instance.ShakeCamera(0.2f,0.2f);
        MusicManager.instance.PlayInstaSound(MusicManager.SoundType.ItemIncrease, Random.Range(0.8f, 1.2f));
        
        flowersCollected++;

        metersFromPlayer = Mathf.Max(0, metersFromPlayer - 100);
        currentBossHealth = metersFromPlayer;
        
        if (motionEffect != null)
            motionEffect.Shake(0.1f, 0.5f);
        
        descriptionText.text = $"[{metersFromPlayer} meters from you]";
        descriptionText.color = randomColors[Random.Range(0, randomColors.Length)];

        if (InventoryManager.Instance.IsInventoryEmpty())
        {
            StartBoss();
        }
    }

    public void StartBoss()
    {
        isActive = true;
        Debug.Log("Starting boss");
        
        motionEffect.MoveTowards(targetPos.position);
        
        movementCoroutine = StartCoroutine(MoveAround());
        
        StartCoroutine(DestroyObstacles());
        MusicManager.instance.StartBossMusic();
        
        animator.SetBool("Fight", true);
    }
    
    private IEnumerator MoveAround()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(Random.Range(minMoveDelay, maxMoveDelay));
            
            Vector3 randomOffset = Random.insideUnitCircle * movementRadius;
            movementTarget = targetPos.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            motionEffect.MoveTowards(movementTarget);
        }
    }

    private IEnumerator DestroyObstacles()
    {
        foreach (GameObject block in blocks)
        {
            if(!block.activeSelf) continue;
            block.GetComponent<MotionEffect>().Shake(5, 0.3f);
            yield return  new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(3);

        foreach (GameObject block in blocks)
        {
            block.SetActive(false);
            block.GetComponent<MotionEffect>().StopMotion();
            MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Explosion, Random.Range(0.8f, 1.2f));
            yield return  new WaitForSeconds(0.1f);
        }
        
        attackCoroutine = StartCoroutine(FireballAttack());
        platformCoroutine = StartCoroutine(SwapPlatforms());
    }

    public void LoverBossHealth()
    {
        if (!isActive) return;
        
        currentBossHealth -= 5;
        
        descriptionText.text = $"[{currentBossHealth} meters from you]";
        descriptionText.color = randomColors[Random.Range(0, randomColors.Length)];
        
        if (motionEffect != null)
            motionEffect.Shake(0.1f, 0.2f);
        
        MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Attack, Random.Range(0.8f, 1.2f));

        if (currentBossHealth <= 0)
        {
            EndBoss();
        }
    }
    
    private void EndBoss()
    {
        animator.SetBool("Fight", false);
        isActive = false;
        
        StopCoroutine(movementCoroutine);
        movementCoroutine = null;
        
        StopCoroutine(attackCoroutine);
        attackCoroutine =  null;
        
        StopCoroutine(platformCoroutine);
        platformCoroutine = null;
        
        StopAllCoroutines();
        
        CameraManager.Instance.ShakeCameraIncreasing(2.5f, 0.5f);
        motionEffect.Shake(5, 2);
        
        MusicManager.instance.PlayRandomSound(MusicManager.SoundType.Death, Random.Range(0.8f, 1.2f));
        MusicManager.instance.SwapMusic(MusicManager.instance.gameMusic[0]);

        StartCoroutine(DeathMove());
    }

    private IEnumerator DeathMove()
    {
        yield return new WaitForSeconds(1f);
        
        motionEffect.MoveTowards(deathMovePos.position);
        
        yield return new WaitForSeconds(1f);
        
        MusicManager.instance.PlayRandomSound(MusicManager.SoundType.Spawn, Random.Range(0.8f, 1.2f));
        powerBlock.SetActive(true);
      
        MotionEffect powerMotion = powerBlock.GetComponent<MotionEffect>();
        if (powerMotion != null)
             powerMotion.StartMotion();
        
        yield return new WaitForSeconds(0.5f);
        
        MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Jump);
        powerMotion.MoveTowards(powerBlockMovePos.position);
    }
    
    private IEnumerator SwapPlatforms()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(Random.Range(minPlatformSwapDelay, maxPlatformSwapDelay));
            if (!isActive) yield break;
            
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            
            int activeIndex = -1;

            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] != null && platforms[i].activeSelf)
                {
                    activeIndex = i;
                    break;
                }
            }
            
            if (activeIndex == -1) continue;
            int nextIndex = (activeIndex + 1) % 2;

            GameObject currentPlatform = platforms[activeIndex];
            GameObject nextPlatform = platforms[nextIndex];

            GameObject hiddenPlatform = platformHiddenSpots[activeIndex];
            GameObject showPlatform = platformHiddenSpots[nextIndex];
            
            MotionEffect motion = currentPlatform.GetComponent<MotionEffect>();
            
            if (motion != null)
            {
                motion.StartMotion();
                motion.Shake(platformShakeDuration, platformShakeMagnitude);
            }

            yield return new WaitForSeconds(platformShakeDuration);

            if (!isActive) yield break;
            MusicManager.instance.PlayInstaSound(MusicManager.SoundType.Explosion, Random.Range(0.8f, 1.2f));
            
            currentPlatform.SetActive(false);
            nextPlatform.SetActive(true);
            
            hiddenPlatform.SetActive(false);
            showPlatform.SetActive(true);
            
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(FireballAttack());
        }
    }
    
    private IEnumerator FireballAttack()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(Random.Range(minAttackInterval, maxAttackInterval));

            if (!isActive || playerTarget == null) continue;

            SpawnFireball();
        }
    }

    private void SpawnFireball()
    {
        Vector3 spawnPosition = fireballSpawnPoint != null ? fireballSpawnPoint.position : transform.position;
        Vector2 direction = (playerTarget.position - spawnPosition).normalized;

        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

        fireball.GetComponent<FireballInteraction>()?.Initialize(direction);
    }
}
