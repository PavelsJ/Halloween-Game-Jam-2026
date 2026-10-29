using System.Collections;
using UnityEngine;

public class ChangeParent : MonoBehaviour
{
    public Transform platformTransform;

    private IEnumerator Start()
    {
        yield return null;
        
        platformTransform.SetParent(null);
    }
}
