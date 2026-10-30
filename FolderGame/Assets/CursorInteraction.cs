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
        MusicManager.instance.PlayRandomSound(MusicManager.SoundType.PowerUp, Random.Range(0.8f, 1.2f));
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
