using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class AppearEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float moveDistance = 9f;

    [SerializeField] private AnimationCurve curve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 targetPosition;
    private Vector3 startPosition;

    private CanvasGroup canvasGroup;
    private Coroutine appearCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        targetPosition = transform.localPosition;
        startPosition = targetPosition - new Vector3(0, moveDistance, 0);
    }

    private void OnEnable()
    {
       Appear();
    }

    public void Appear()
    {
        transform.localPosition = startPosition;
        canvasGroup.alpha = 0f;

        StartCoroutine(PlayAppear());
    }

    private IEnumerator PlayAppear()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;;

            float t = Mathf.Clamp01(time / duration);
            float evaluated = curve.Evaluate(t);

            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, evaluated);

            canvasGroup.alpha = evaluated;

            yield return null;
        }

        transform.localPosition = targetPosition;
        canvasGroup.alpha = 1f;
    }
}