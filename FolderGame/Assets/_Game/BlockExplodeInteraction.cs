using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockExplodeInteraction : BlockPlayInteraction
{
    [Header("Block Interaction")]
    [SerializeField] private TMP_Text explodeText;
    [SerializeField] private int explodeTime = 5;

    [Header("Warning Settings")]
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private float blinkInterval = 0.25f;

    private float timer;
    private bool isCounting;

    private readonly string[] trollMessages =
    {
        "Mmm, actually, try again!",
        "Probably missed...",
        "Close enough. Not really...",
        "Skill issue detected...",
        "Nice try, genius...",
        "Error 404: Brain not found...",
        "Press harder. I'm sure it'll help...",
        "Almost! Just kidding...",
        "Free phone? Sure, buddy...",
        "Congratulations! You played yourself...",
        "Certified genius moment...",
        "I admire your confidence...",
        "Wow. That was embarrassing...",
        "Keep clicking. Something might happen...",
        "FAILED......",
        "The definition of insanity is trying this again...",
        "Achievement unlocked: Professional Button Presser...",
        "Surprise! Nothing happened..."
    };

    private Coroutine explodeCoroutine;

    public override IEnumerator Start()
    {
        yield return null;
        
        Sprite randomSprite = image.sprite;
        
        if (isRandom)
        {
            if (adSprites.Length > 0) 
                randomSprite = adSprites[Random.Range(0, adSprites.Length)];
        }
      
        image.sprite = randomSprite;
        
        AutoImageAspectRatio aspectRatio = image.GetComponent<AutoImageAspectRatio>();
        if (aspectRatio != null) aspectRatio.Init();
        
        isCounting = false;

        float warningTimer = 0f;
        bool isRed = false;

        while (warningTimer < warningDuration)
        {
            if (explodeText != null)
            {
                explodeText.text = "!!!!! INCOMING! BE READY !!!!!";
                explodeText.color = isRed ? Color.red : Color.black;
            }

            isRed = !isRed;

            yield return new WaitForSeconds(blinkInterval);
            warningTimer += blinkInterval;
        }

        timer = explodeTime;

        if (explodeText != null)
            explodeText.color = Color.white;

        isCounting = true;
        explodeCoroutine = null;
    }

    public override void Release()
    {
        if (!isCounting) return;
        
        explodeText.color = Color.red;
        timer = explodeTime;
        
        string randomMessage = trollMessages[Random.Range(0, trollMessages.Length)];
        explodeText.text = randomMessage;
        
        isCounting = false;

        if (explodeCoroutine != null) StopCoroutine(explodeCoroutine);
        explodeCoroutine = StartCoroutine(DelayedRelease());
    }

    private IEnumerator DelayedRelease()
    {
        yield return new WaitForSeconds(warningDuration);
        isCounting = true;
        
        explodeCoroutine = null;
    }

    public void Explode()
    {
        IsUsed = true;
        CameraManager.Instance.ShakeCamera(0.2f, 0.2f);
        parentBlock.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isCounting) return;

        timer -= Time.deltaTime;

        UpdateTimerText();

        if (timer <= 0f)
        {
            isCounting = false;
            Explode();
        }
    }
    
    private void UpdateTimerText()
    {
        if (explodeText != null)
        {
            explodeText.color = Color.black;
            explodeText.text = $"!!!!!Get a free phone! Offer ends in <color=red>{Mathf.CeilToInt(Mathf.Max(0f, timer))}</color> seconds!!!!!!";
        }
    }
}
