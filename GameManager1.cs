using UnityEngine;
class CandyPrefabs
{
    public GameObject rowCandy;
    public GameObject colorCandy;
    public GameObject adjacentCandy;
    public GameObject normalCandy;
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;    

    private GameObject rowMarkerP;
    private GameObject columnMarkerP;
    private GameObject rainbowMarkerP;
    private GameObject adjacentMarkerP;
    public bool useMouse;

    public static GameManager Instance { get => instance; }
    public GameObject RowMarkerP { get => rowMarkerP; }
    public GameObject ColumnMarkerP { get => columnMarkerP; }
    public GameObject RainbowMarkerP { get => rainbowMarkerP; }
    public GameObject AdjacentMarkerP { get => adjacentMarkerP; }

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;

        rowMarkerP = Resources.Load<GameObject>("rowMarkerP");
        columnMarkerP = Resources.Load<GameObject>("columnMarkerP");
        rainbowMarkerP = Resources.Load<GameObject>("rainbowCandyP");
        adjacentMarkerP = Resources.Load<GameObject>("adjacentMarkerP");
    }
    
    public GameObject GetRowCandy()
    {
        
        return null;
    }


}