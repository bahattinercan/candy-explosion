using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Informations")]
    public string levelToLoad;

    public int level;
    private MyGameData gameData;
    private int starsActive;
    private int highScore;

    [Header("UI Stuffs")]
    public Image[] stars;

    public Text highScoreText;
    public Text starText;

    // Start is called before the first frame update
    private void OnEnable()
    {
        gameData = FindObjectOfType<MyGameData>();
        LoadData();
        ActivateStars();
        SetText();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void LoadData()
    {
        if (gameData != null)
        {
            starsActive = gameData.saveData.stars[level - 1];
            highScore = gameData.saveData.highScores[level - 1];
        }
    }

    private void SetText()
    {
        highScoreText.text = "" + highScore;
        starText.text = starsActive + "/3";
    }

    private void ActivateStars()
    {
        for (int i = 0; i < 3; i++)
        {
            stars[i].enabled = false;
        }
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("currentLevel", level - 1);
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(levelToLoad);
    }
}