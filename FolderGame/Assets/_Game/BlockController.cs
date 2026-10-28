using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MotionEffect))]
public class BlockController : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Collider2D ground;
    [SerializeField] private TMP_Text blockText;
    
    [SerializeField] private List<Interactable> interaction;
    public int LayerCount => 1;

    [Header("Block Visual")]
    [SerializeField] private Image outline;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    
    private Material currentMaterial;
    private MotionEffect motionEffect;
    
    private readonly string[] trollMessages =
    {
        "Free_Money.exe",
        "Hot_Singles.html",
        "Free_Robux.exe",
        "Fake_Antivirus.exe",
        "Win_Prizes.txt",
        "Free_Crypto.exe",
        "Click_Here.html",
        "Free_Vbucks.exe",
        "Hot_Deals.html",
        "Free_Iphone.exe",
        "Virus_Remover.exe",
        "Free_Diamonds.exe",
        "Big_Discount.html",
        "Lucky_Winner.txt",
        "Free_Premium.exe",
        "Secret_Hack.exe",
        "Easy_Money.exe",
        "Fake_Update.exe",
        "Free_Pizza.txt",
        "Crazy_Offer.html",
        "Free_Cookies.txt",
        "Instant_Riches.exe",
        "Hot_Offers.html",
        "Free_Gifts.exe",
        "Mega_Sale.html",
        "Bonus_Claim.txt",
        "Free_Skins.exe",
        "Super_Cleaner.exe",
        "Quick_Cash.exe",
        "Fake_Warning.txt",
        "Free_Wifi.exe",
        "Best_Deal.html",
        "Free_Laptop.exe",
        "Easy_Dating.html",
        "Free_Gold.exe",
        "Lucky_Deal.txt",
        "Free_Games.exe",
        "Secret_Offer.html",
        "Fake_Lottery.txt",
        "Free_Coupons.html"
    };

    private void Awake()
    {
        if (blockText != null)
            blockText.text = trollMessages[Random.Range(0, trollMessages.Length)];
        
        if (outline != null)
        {
            Material newMaterial = outline.material;
            outline.material = new Material(newMaterial);
            SetOutline(false);
        }
        
        currentMaterial = new Material(hitMaterial);
        
        foreach (Interactable i in interaction)
        {
            if (i != null)
                i.Init(this, currentMaterial);
        }
        
        motionEffect = GetComponent<MotionEffect>();
    }

    private void OnEnable()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator Start()
    {
        yield return null;
        
        LayerManager.Instance.Register(this);
    }
    
    public void Reset()
    {
        if (blockText != null)
            blockText.text = trollMessages[Random.Range(0, trollMessages.Length)];
        
        StartCoroutine(FlashCoroutine());
    }

    private void OnDestroy()
    {
        if (LayerManager.Instance != null)
            LayerManager.Instance.Unregister(this);
        
        if (currentMaterial != null)
            Destroy(currentMaterial);
    }

    public void SortLayers(int index)
    {
        canvas.sortingOrder = index;
    }

    public void BringToFront()
    {
        LayerManager.Instance.BringToFront(this);
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

    public void SetOutline(bool enabled)
    {
        if (outline != null)
            outline.enabled = enabled;
    }

    public MotionEffect GetMotion()
    {
        return motionEffect;
    }
    
    public Collider2D GetGround()
    {
        return ground;
    }
}