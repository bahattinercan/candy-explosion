using UnityEngine;

public class LevelSelectManager : MonoBehaviour, IDataPersistence
{
    public GameObject[] panels;
    public GameObject currentPanel;
    public int page;
    private GameData gameData;
    public int currentLevel = 0;

    public void LoadData(GameData data)
    {
        gameData = data;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        if (gameData != null)
        {
            for (int i = 0; i < gameData.GetIsActives().Count; i++)
            {
                if (gameData.GetIsActive(i))
                {
                    currentLevel = i;
                }
            }
        }
        page = (int)Mathf.Floor(currentLevel / 9);
        currentPanel = panels[page];
        panels[page].SetActive(true);
    }

    public void SaveData(GameData data)
    {
        
    }

    public void PageRight()
    {
        if (page < panels.Length - 1)
        {
            currentPanel.SetActive(false);
            page++;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
        }
    }

    public void PageLeft()
    {
        if (page > 0)
        {
            currentPanel.SetActive(false);
            page--;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
        }
    }
}