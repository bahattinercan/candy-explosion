using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Goal
{
    public int numberNeeded;
    private int numberCollected;
    private Sprite sprite;
    public EDotColor dotColor;
    public EDotType dotType;

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public int NumberCollected { get => numberCollected; set => numberCollected = value; }
}

public class GoalManager : MonoBehaviour
{
    private static GoalManager instance;
    public Goal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    private Board board;
    private EndGameManager endGame;

    public static GoalManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        board = Board.Instance;
        endGame = EndGameManager.Instance;
        GetGoals();
        SetupGoals();
    }

    private void GetGoals()
    {
        if (board.world != null && board.world.levels[board.level] != null)
        {
            levelGoals = board.world.levels[board.level].goals;
            for (int i = 0; i < levelGoals.Length; i++)
            {
                levelGoals[i].NumberCollected = 0;
            }
        }
    }

    private void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            goal.transform.localScale = new Vector3(1, 1, 1);
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.mySprite = levelGoals[i].Sprite;
            panel.myString = "0/" + levelGoals[i].numberNeeded;

            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            gameGoal.transform.localScale = new Vector3(1, 1, 1);
            GoalPanel gamePanel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(gamePanel);
            gamePanel.mySprite = levelGoals[i].Sprite;
            gamePanel.myString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            Goal goal = levelGoals[i];

            currentGoals[i].text.text = "" + goal.NumberCollected + "/" + goal.numberNeeded;
            if (goal.NumberCollected >= goal.numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].text.text = "" + goal.numberNeeded + "/" + goal.numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            if (endGame != null && board.currentState!=EGameState.win)
            {
                endGame.WinGame();
                //GameData.Instance.Save();
            }            
        }
    }

    public void CompareGoal(EDotType dotType)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            Goal levelGoal = levelGoals[i];
            if (dotType == levelGoal.dotType)
            {
                levelGoals[i].NumberCollected++;
            }
        }
    }

    public void CompareGoal(EDotColor dotColor, EDotType dotType)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            Goal levelGoal = levelGoals[i];
            if (dotType == levelGoal.dotType && dotColor == levelGoal.dotColor)
            {
                levelGoals[i].NumberCollected++;
            }
        }
    }
}