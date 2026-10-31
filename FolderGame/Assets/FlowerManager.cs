using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FlowerManager : MonoBehaviour
{
   [SerializeField] private GameObject flowerPanel;
   [SerializeField] private TMP_Text flowerText;
   
   private IEnumerator Start()
   {
      yield return new WaitForSeconds(1f);

      int flowerCount = InventoryManager.Instance.Items.Count;
      flowerPanel.SetActive(true);

      switch (flowerCount)
      {
         case 0:
            flowerText.text = "No flowers collected...";
            break;

         case 1:
            flowerText.text = $"Only {flowerCount} flower...";
            break;

         case 2:
            flowerText.text = $"{flowerCount} flowers collected...";
            break;
         
         case 7:
            flowerText.text = $"{flowerCount} flowers collected! Massive progress!";
            break;

         case 8:
            flowerText.text = $"{flowerCount} flowers collected! You are so ready!!!!!";
            break;

         default: 
            flowerText.text = $"{flowerCount} flowers collected! Not bad at all. ;(";
            break;
      }
   }
}
