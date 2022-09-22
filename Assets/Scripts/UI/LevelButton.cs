using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour, IDataPersistence
{
    [Header("Active Stuff")]
    public bool isActive;

    public Sprite lockedSprite;
    public Image buttonImage;
    public Button myButton;
    private int starsActive;

    [Header("Level UI")]
    public Image[] stars;

    public Text levelText;
    public int level;
    public GameObject confirmPanel;
    private void Start()
    {
        buttonImage = transform.Find("buttonImage").GetComponent<Image>();
        myButton = GetComponent<Button>();
    }

    public void LoadData(GameData data)
    {
        Debug.Log(data.levelList.Count);
        
        LocalLoadData(data);
        ActivateStars();
        ShowLevel();
        DecideSprite();
        levelText.text = level.ToString();
    }

    public void SaveData(GameData data)
    {
        //throw new System.NotImplementedException();
    }

    private void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    private void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.gameObject.SetActive(false);
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.gameObject.SetActive(true);
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    private void ShowLevel()
    {
        levelText.text = "" + level;
    }

    public void ConfirmPanel()
    {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }

    private void LocalLoadData(GameData gameData)
    {        
        if (gameData.GetIsActive(level-1)==true)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }
        // get active star number
        starsActive = gameData.GetStar(level-1);
    }
}