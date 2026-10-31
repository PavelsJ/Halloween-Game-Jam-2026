using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MilfController : MonoBehaviour
{
    private int currentBossHealth;
    
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Animator animator;
    
    private int metersFromPlayer = 900;
    private int flowersCollected = 0;
    private const int maxFlowers = 8;

    private bool isActive = false;
    
    
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
        MusicManager.instance.StartBossMusic();
        animator.SetBool("Fight", true);
        Debug.Log("Starting boss");
    }

    public void LoverBossHealth()
    {
        if (!isActive) return;
        
        currentBossHealth -= 10;
        
        descriptionText.text = $"[{currentBossHealth} meters from you]";
        descriptionText.color = randomColors[Random.Range(0, randomColors.Length)];
        
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
        
        MusicManager.instance.PlayRandomSound(MusicManager.SoundType.Death, Random.Range(0.8f, 1.2f));
        MusicManager.instance.SwapMusic(MusicManager.instance.gameMusic[0]);
    }


    private void SpawnViruses()
    {
        
    }

    private void RotateFlowers()
    {
        
    }

    private void FastAttack()
    {
        
    }
}
