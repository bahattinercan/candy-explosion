using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public CandiesSO candiesSO;

    [Header("Prefabs")]
    public GameObject backgroundTileP;
    public GameObject breakableP;
    public GameObject lockTileP;
    public GameObject concreteTileP;
    public GameObject slimeP;
    public GameObject destroyEffect;

    [Header("Gameplay Settings")]
    public bool useMouse;

    
    public static GameManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        instance = this;
        candiesSO = Resources.Load<CandiesSO>(typeof(CandiesSO).Name);
    }

    public int GetScoreStar(int[] goals,int score)
    {
        int star = 0;
        for (int i = 0; i < goals.Length; i++)
        {
            if (score > goals[i])
                star = i + 1;
        }
        return star;
    }
}