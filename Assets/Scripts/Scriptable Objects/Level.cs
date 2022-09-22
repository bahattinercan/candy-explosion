using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int height;

    public int width;

    [Header("Starting Tiles")]
    public TileType[] boardLayout;

    [Header("Available Dots")]
    public GameObject[] dots;

    [Header("End Game Requirements")]
    public EndGameRequirement requirements;

    [Header("Level Goals")]
    public Goal[] goals;

    private int basePieceValue=20;

    public float maxGoalMultiplier = 5f;
    public int BasePieceValue { get => basePieceValue;private set => basePieceValue = value; }

    public int[] GetScoreGoals()
    {
        int[] scoreGoals = new int[3];
        // calculate score goals
        for (int i = 0; i < goals.Length; i++)
        {
            int goal = goals[i].numberNeeded;
            scoreGoals[2] += (int)(goal * BasePieceValue * maxGoalMultiplier);
        }
        scoreGoals[1] = (int)(scoreGoals[0] * 60 / 100f);
        scoreGoals[0] = (int)(scoreGoals[0] * 40 / 100f);
        return scoreGoals;
    }
}