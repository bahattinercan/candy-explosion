using UnityEngine;
using UnityEngine.UI;

public enum EGameType
{
    moves,
    time
}

[System.Serializable]
public class EndGameRequirement
{
    public EGameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    private static EndGameManager instance;

    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counter;
    public int currentCounterValue;
    public EndGameRequirement requirements;
    private float timerSeconds;
    private Board board;

    public static EndGameManager Instance { get => instance; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        board = Board.Instance;
        SetGameType();
        SetupGame();
    }

    // Update is called once per frame
    private void Update()
    {
        if (requirements.gameType == EGameType.time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    private void SetGameType()
    {
        if (board.world != null)
        {
            World world = board.world;
            int levelNumber = board.level;
            if (world.levels[levelNumber] != null)
            {
                requirements = world.levels[levelNumber].requirements;
            }
        }
    }

    private void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == EGameType.moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != EGameState.pause)
        {
            currentCounterValue--;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
            counter.text = "" + currentCounterValue;
        }
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = EGameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fadePanel = GameObject.Find("AnimationController").GetComponent<FadePanelController>();
        DataPersistenceManager.instance.SaveGame();
        fadePanel.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = EGameState.lose;
        Debug.Log("you lose");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fadePanel = GameObject.Find("AnimationController").GetComponent<FadePanelController>();
        DataPersistenceManager.instance.SaveGame();
        fadePanel.GameOver();
    }
}