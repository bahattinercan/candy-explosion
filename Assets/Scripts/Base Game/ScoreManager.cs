using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;

    private Board board;
    public Text scoreText;
    public int score;
    public Image scoreSliderImage;

    public static ScoreManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        board = Board.Instance;
        IncreaseScore(0);
    }

    public void IncreaseScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
        scoreSliderImage.fillAmount = ((float)score / board.scoreGoals[2]);

        int highScore = DataPersistenceManager.instance.gameData.GetHighScore(board.level);
        if (score > highScore)
        {
            DataPersistenceManager.instance.gameData.SetHighScore(score, board.level);
            DataPersistenceManager.instance.gameData
                .SetStar(GameManager.Instance.GetScoreStar(board.scoreGoals, score), board.level);
        }
    }
}