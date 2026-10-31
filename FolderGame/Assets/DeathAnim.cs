using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlitchMaskAnimation : MonoBehaviour
{
   [Header("Animation Settings")]
   [SerializeField] private float animationDuration = 1.5f;
   [SerializeField] private float startDelay = 0f;
   
   private Material currentMaterial;
   
   private static readonly int CircleRadius = Shader.PropertyToID("_CircleRadius");

   private void Start()
   {
      Image image = GetComponent<Image>();

      currentMaterial = new Material(image.material);
      image.material = currentMaterial;

      currentMaterial.SetFloat(CircleRadius, 0f);

      StartCoroutine(Animate(animationDuration));
      StartCoroutine(TextDelay(1));
   }

   private IEnumerator TextDelay(float duration)
   {
      yield return new WaitForSeconds(duration);
      
      TMP_Text text = GetComponentInChildren<TMP_Text>();
      if (text != null) text.color = Color.white;
   }
   
   private IEnumerator Animate(float duration)
   {
      if (startDelay > 0f) 
         yield return new WaitForSeconds(startDelay);

      float elapsed = 0f;

      while (elapsed < duration)
      {
         elapsed += Time.deltaTime;
         float progress = Mathf.Clamp01(elapsed / duration);
         currentMaterial.SetFloat(CircleRadius, Mathf.Lerp(0f, 1f, progress));

         yield return null;
      }
      currentMaterial.SetFloat(CircleRadius, 1f);
   }

   private void OnDestroy()
   {
      if (currentMaterial != null)
         Destroy(currentMaterial);
   }
}
