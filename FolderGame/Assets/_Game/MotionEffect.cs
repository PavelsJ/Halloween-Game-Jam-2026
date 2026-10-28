using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionEffect : MonoBehaviour
{
    public bool playOnAwake = true;
    
    [Header("Movement")]
    [SerializeField] private float amplitude = 0.1f;
    [SerializeField] private float cycleDuration = 4f;
    [SerializeField] private float rotateSpeed = 4f;

    private float startY;
    private float timer;
    
    public bool isMoving = true;
    public bool isRotating = false;
    
    private IEnumerator Start()
    {
        if (!playOnAwake) yield break;
        
        yield return null;

        StartMotion();
    }

    public void StartMotion()
    {
        startY = transform.position.y;
        timer = Random.Range(0f, cycleDuration);
        
        StartCoroutine(MoveRoutine());
        StartCoroutine(RotateRoutine());
    }
    
    public void StopMotion()
    {
        startY = transform.position.y;
        timer = Random.Range(0f, cycleDuration);
        
        StopAllCoroutines();
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (isMoving)
            {
                timer += Time.deltaTime;

                float yOffset = Mathf.Sin(
                    (timer / cycleDuration) * 2f * Mathf.PI
                ) * amplitude;

                Vector3 pos = transform.position;
                pos.y = startY + yOffset;
                transform.position = pos;
            }

            yield return null;
        }
    }

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            if (isRotating)
            {
                transform.Rotate(
                    Vector3.forward,
                    rotateSpeed * Time.deltaTime
                );
            }

            yield return null;
        }
    }
}
