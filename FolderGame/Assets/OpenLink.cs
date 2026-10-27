using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string url;

    public void Open()
    {
        Application.OpenURL(url);
    }
}
