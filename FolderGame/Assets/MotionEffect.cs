using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionEffect : MonoBehaviour
{
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
        yield return null;
        
        startY = transform.position.y;
        timer = Random.Range(0f, cycleDuration);
        
        StartCoroutine(MoveRoutine());
        StartCoroutine(RotateRoutine());
    }

    public void StartMove(bool move, bool rotate)
    {
        isMoving = move;
        isRotating = rotate;
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
