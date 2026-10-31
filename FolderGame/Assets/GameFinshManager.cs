using UnityEngine;

public class GameFinshManager : MonoBehaviour
{
    [SerializeField] private GameObject finishPanel;


    public void FinishGame()
    {
        finishPanel.SetActive(true);
    }
}
