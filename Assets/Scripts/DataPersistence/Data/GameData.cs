using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<Serializable_Level> levelList;
    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        levelList = new List<Serializable_Level>();
    }

    public void SetHighScore(int score, int index)
    {
        levelList[index].score = score;
    }

    public void SetStar(int star, int index)
    {
        levelList[index].star = star;
    }

    public void SetIsActive(bool isActive, int index)
    {
        levelList[index].canPlay = isActive;
    }

    public int GetHighScore(int index)
    {
        return levelList[index].score;
    }

    public int GetStar(int index)
    {
        return levelList[index].star;
    }

    public List<int> GetStars()
    {
        List<int> items = new List<int>();
        for (int i = 0; i < levelList.Count; i++)
        {
            items.Add(levelList[i].star);
        }
        return items;
    }

    public List<int> GetScores()
    {
        List<int> items = new List<int>();
        for (int i = 0; i < levelList.Count; i++)
        {
            items.Add(levelList[i].score);
        }
        return items;
    }



    public List<bool> GetIsActives()
    {
        List<bool> bools = new List<bool>();
        for (int i = 0; i < levelList.Count; i++)
        {
            bools.Add(levelList[i].canPlay);
        }
        return bools;
    }

    public bool GetIsActive(int index)
    {
        return levelList[index].canPlay;
    }
}