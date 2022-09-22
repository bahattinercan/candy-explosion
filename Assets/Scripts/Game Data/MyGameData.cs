using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool[] isActives;
    public int[] highScores;
    public int[] stars;

    public SaveData(int length)
    {
        isActives = new bool[length];
        highScores = new int[length];
        stars = new int[length];

        isActives[0] = true;
    }
}

public class MyGameData : MonoBehaviour
{
    private static MyGameData instance;
    public SaveData saveData;

    public static MyGameData Instance { get => instance; private set => instance = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        World world= Resources.Load<World>("World");
        saveData= new SaveData(world.levels.Length);
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        Load();
    }

    public void Save()
    {
        // create a binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();
        // create a rout from the program to the file
        FileStream file = File.Open(Application.persistentDataPath + "/data.dat", FileMode.Create);
        // save data in the file
        formatter.Serialize(file, saveData);
        // close the data stream
        file.Close();
        Debug.Log("saved");
    }

    public void Load()
    {
        // check if the save game file exists
        if (File.Exists(Application.persistentDataPath + "/data.dat"))
        {
            // create a binary formatter which can read binary files
            BinaryFormatter formatter = new BinaryFormatter();
            // create a rout from the program to the file
            FileStream file = File.Open(Application.persistentDataPath + "/data.dat", FileMode.Open);
            // load the data
            saveData = formatter.Deserialize(file) as SaveData;
            // close the data stream
            file.Close();
            Debug.Log("loaded");
        }
        else
        {
            Debug.Log("data is not found");
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    #region Save Data set and get methods

    public void SetHighScore(int score, int index)
    {
        saveData.highScores[index] = score;
    }

    public void SetStar(int star, int index)
    {
        saveData.stars[index] = star;
    }

    public void SetIsActive(bool isActive, int index)
    {
        saveData.isActives[index] = isActive;
    }

    public int[] GetHighScores()
    {
        return saveData.highScores;
    }

    public int GetHighScore(int index)
    {
        return saveData.highScores[index];
    }

    public int[] GetStars()
    {
        return saveData.stars;
    }

    public int GetStar(int index)
    {
        return saveData.stars[index];
    }

    public bool[] GetIsActives()
    {
        return saveData.isActives;
    }

    public bool GetIsActive(int index)
    {
        return saveData.isActives[index];
    }

    #endregion Save Data set and get methods
}