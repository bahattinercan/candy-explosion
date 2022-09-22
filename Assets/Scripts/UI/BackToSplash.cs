using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour
{
    public string sceneToLoad;
    private Board board;

    private void Start()
    {
        board = Board.Instance;
    }

    public void WinOk()
    {
        int index = board.level + 1;
        DataPersistenceManager.instance.gameData.SetIsActive(true, index);
        DataPersistenceManager.instance.gameData.SetStar(0, index);
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoseOk()
    {
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(sceneToLoad);
    }
}