using UnityEngine;

public class CursorInteraction : MonoBehaviour
{
    [SerializeField] private Transform playerCursor;
    private PlayerPreview playerPreview;

    private void Start()
    {
        playerPreview = GetComponent<PlayerPreview>();
    }
    
    public void MakeCursorBigger(float size)
    {
        playerCursor.localScale += Vector3.one * size;
    }

    public void MakeCursorSmaller(float size)
    {
        playerCursor.localScale -= Vector3.one * size;
        
        playerCursor.localScale = new Vector3(
            Mathf.Max(0.1f, playerCursor.localScale.x),
            Mathf.Max(0.1f, playerCursor.localScale.y),
            Mathf.Max(0.1f, playerCursor.localScale.z)
        );
    }
    
    public void ChangeCursor(float size)
    {
        playerCursor.localScale = Vector3.one * size;
    } 
}
