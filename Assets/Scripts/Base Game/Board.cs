using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enums

public enum EGameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum ETile
{
    breakable,
    blank,
    locked,
    concrete,
    slime,
    normal
}

#endregion Enums

#region classes

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public EDotType dotType;
    public EDotColor dotColor;
}

#endregion classes

public class Board : MonoBehaviour
{

    #region Variables
    private static Board ınstance;
    
    [Header("Scriptable Object Stuff")]
    public World world;

    public int level;

    public EGameState currentState = EGameState.move;

    [Header("Board Dimensions")]
    public int width;

    public int height;
    public int offSet;

    [Header("Layout")]
    public TileType[] boardLayout;

    private bool[,] blankSpaces;
    public BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] concreteTiles;
    public BackgroundTile[,] slimeTiles;
    public Transform[,] startDots;
    public Dot[,] allDots;
    public GameObject[] dots;
    public Dot currentDot;
    public MatchType matchType;
    private bool makeSlime = true;

    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;

    [Header("Score Stuff")]
    public int basePieceValue = 20;

    public int[] scoreGoals;
    private int streakValue = 1;

    [Header("Time For Refill Delay")]
    public float refillDelay = .5f;

    public static Board Instance { get => ınstance; private set => ınstance = value; }
    #endregion

    private void Awake()
    {
        Instance = this;
        if (PlayerPrefs.HasKey("currentLevel"))
        {

            level = PlayerPrefs.GetInt("currentLevel");
            Debug.Log(level);
        }
        if (world != null && world.levels[level] != null)
        {
            Level level = world.levels[this.level];
            width = level.width;
            height = level.height;
            dots = level.dots;
            boardLayout = level.boardLayout;
            basePieceValue = level.BasePieceValue;
            scoreGoals = level.GetScoreGoals();

            CandiesSO candiesSO = Resources.Load<CandiesSO>(typeof(CandiesSO).Name);
            // dot tipine ve rengine göre sprite'ları level sprite'a yükle
            for (int i = 0; i < level.goals.Length; i++)
            {
                Goal currentGoal = level.goals[i];
                level.goals[i].Sprite = candiesSO.GetCandySprite(currentGoal.dotColor, currentGoal.dotType,true);
            }
        }
        else
        {
            Debug.Log("world or world level is null");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        soundManager = SoundManager.Instance;
        scoreManager = ScoreManager.Instance;
        goalManager = GoalManager.Instance;
        findMatches = FindMatches.Instance;

        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        startDots = new Transform[width, height];
        blankSpaces = new bool[width, height];
        allDots = new Dot[width, height];
        Setup();
        currentState = EGameState.pause;
    }


    #region Generate Functions

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].dotType == EDotType.blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].dotType == EDotType.breakable)
            {
                int x = boardLayout[i].x;
                int y = boardLayout[i].y;
                Vector2 tempPos = new Vector2(x, y);
                GameObject tile = Instantiate(GameManager.Instance.breakableP, tempPos, Quaternion.identity);
                tile.transform.parent = transform;
                lockTiles[x, y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateLockTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].dotType == EDotType.locked)
            {
                int x = boardLayout[i].x;
                int y = boardLayout[i].y;
                Vector2 tempPos = new Vector2(x, y);
                GameObject tile = Instantiate(GameManager.Instance.lockTileP, tempPos, Quaternion.identity);
                breakableTiles[x, y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateConcreteTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].dotType == EDotType.concrete)
            {
                int x = boardLayout[i].x;
                int y = boardLayout[i].y;
                Vector2 tempPos = new Vector2(x, y);
                GameObject tile = Instantiate(GameManager.Instance.concreteTileP, tempPos, Quaternion.identity);
                tile.transform.SetParent(transform);
                concreteTiles[x, y] = tile.GetComponent<BackgroundTile>();

                GameObject bgTile = Instantiate(GameManager.Instance.backgroundTileP, tempPos, Quaternion.identity);
                bgTile.transform.SetParent(transform);
            }
        }
    }

    private void GenerateSlimeTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].dotType == EDotType.slime)
            {
                int x = boardLayout[i].x;
                int y = boardLayout[i].y;
                Vector2 tempPos = new Vector2(x, y);
                GameObject tile = Instantiate(GameManager.Instance.slimeP, tempPos, Quaternion.identity);
                tile.transform.SetParent(transform);
                slimeTiles[x, y] = tile.GetComponent<BackgroundTile>();

                GameObject bgTile = Instantiate(GameManager.Instance.backgroundTileP, tempPos, Quaternion.identity);
                bgTile.transform.SetParent(transform);
            }
        }
    }

    public void GenerateCandies_Start()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].dotType == EDotType.normal)
            {
                TileType tileType = boardLayout[i];
                Vector2 tempPosition = new Vector2(tileType.x, tileType.y + offSet);
                Dot dot = Instantiate(
                    GameManager.Instance.candiesSO.GetCandyPrefab(tileType.dotColor,tileType.dotType),
                    tempPosition,
                    Quaternion.identity).GetComponent<Dot>();
                dot.Setup(tileType.x, tileType.y, transform);
                
                startDots[tileType.x, tileType.y] = dot.transform;                
                allDots[tileType.x, tileType.y] = dot;
            }
        }
    }

    #endregion Generate Functions

    private void Setup()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        GenerateCandies_Start();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePos = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(GameManager.Instance.backgroundTileP, tilePos, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    int doToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[doToUse]) && maxIterations < 100)
                    {
                        doToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }

                    if (startDots[i, j]==null)
                    {
                        Dot dot = Instantiate(dots[doToUse], tempPosition, Quaternion.identity).GetComponent<Dot>();
                        dot.Setup(i, j, transform);
                        allDots[i, j] = dot;
                    }                    
                }
            }
        }
    }

    #region Check Functions

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //Make sure that one and two to the right are in the
                    //board
                    if (i < width - 2)
                    {
                        //Check if the dots to the right and two to the right exist
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].CompareTag(allDots[i, j].tag)
                               && allDots[i + 2, j].CompareTag(allDots[i, j].tag))
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        //Check if the dots above exist
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].CompareTag(allDots[i, j].tag)
                               && allDots[i, j + 2].CompareTag(allDots[i, j].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private void CheckToMakeBombs()
    {
        // how many objects are in findMatches currentMatches?
        if (findMatches.currentMatches.Count > 3)
        {
            MatchType matchType = IsColumnOrRow();

            if (currentDot != null)
            {
                // make a color bomb
                if (matchType.type == 1)
                {
                    if (currentDot.isMatched && currentDot.CompareTag(matchType.color))
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeColorBomb();
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot;
                            if (otherDot.isMatched && otherDot.CompareTag(matchType.color))
                            {
                                otherDot.isMatched = false;
                                otherDot.MakeColorBomb();
                            }
                        }
                    }
                }
                // make a adjacent bomb
                else if (matchType.type == 2)
                {
                    if (currentDot.isMatched && currentDot.CompareTag(matchType.color))
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeAdjacentBomb();
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot;
                            if (otherDot.isMatched && otherDot.CompareTag(matchType.color))
                            {
                                otherDot.isMatched = false;
                                otherDot.MakeAdjacentBomb();
                            }
                        }
                    }
                }
                // make a row or column bomb
                else if (matchType.type == 3)
                {
                    findMatches.CheckBombs(matchType);
                }
            }
        }
    }

    private void CheckToMakeSlime()
    {
        //Check the slime tiles array
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (slimeTiles[i, j] != null && makeSlime)
                {
                    //Call another method to make a new slime
                    MakeNewSlime();
                    return;
                }
            }
        }
    }

    #endregion Check Functions

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// checking for what type of matches is there (column,row,color,adjacent)
    /// </summary>
    /// <returns></returns>
    private MatchType IsColumnOrRow()
    {
        matchType.type = 0;
        matchType.color = "";
        // make a copy of current matches
        List<GameObject> matchCopy = findMatches.currentMatches;

        // cycle through all of match Copy and decide if a bomb needs to be made
        for (int i = 0; i < matchCopy.Count; i++)
        {
            // store current dot
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            string color = thisDot.tag;
            int columnMatch = 0;
            int rowMatch = 0;
            // cycle through rest of the pieces and compare
            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (nextDot == thisDot)
                    continue;
                if (nextDot.column == thisDot.column && nextDot.CompareTag(thisDot.tag))
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.CompareTag(thisDot.tag))
                {
                    rowMatch++;
                }
            }

            // return 3 if column or row match
            // return 2 if adjacent
            // return 1 if it's a color bomb
            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    public void BombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            if (concreteTiles[i, row] != null)
            {
                BackgroundTile concreteTile = concreteTiles[i, row];
                concreteTile.TakeDamage(1);
                if (concreteTile.hitPoints <= 0)
                {
                    concreteTiles[i, row] = null;
                    concreteTile.Die();
                }
            }
        }
    }

    public void BombColumn(int column)
    {
        for (int j = 0; j < height; j++)
        {
            if (concreteTiles[column, j] != null)
            {
                BackgroundTile concreteTile = concreteTiles[column, j];
                concreteTile.TakeDamage(1);
                if (concreteTile.hitPoints <= 0)
                {
                    concreteTiles[column, j] = null;
                    concreteTile.Die();
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        Dot dot = allDots[column, row];
        if (dot.isMatched)
        {
            // Does a tile need to break for breakable tiles
            if (breakableTiles[column, row] != null)
            {
                BackgroundTile bgTile = breakableTiles[column, row];
                bgTile.TakeDamage(1);
                if (bgTile.hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                    bgTile.Die();
                }
            }
            // Does a tile need to break for lock tiles
            if (lockTiles[column, row] != null)
            {
                BackgroundTile bgTile = lockTiles[column, row];
                bgTile.TakeDamage(1);
                if (bgTile.hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                    bgTile.Die();
                }
            }
            DamageConcrete(column, row);
            DamageSlime(column, row);
            if (goalManager != null)
            {
                goalManager.CompareGoal(dot.dotColor,dot.dotType);
                goalManager.UpdateGoals();
            }

            if (soundManager != null)
                soundManager.PlayRandomDestroyNoise();
            GameObject particle = Instantiate(GameManager.Instance.destroyEffect, dot.transform.position, Quaternion.identity);
            Destroy(particle, .3f);
            dot.PopAnimation();
            Destroy(dot.gameObject, .3f);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        // How many elements are in the matched pieces list from findMatches?
        if (findMatches.currentMatches.Count >= 4)
            CheckToMakeBombs();
        findMatches.currentMatches.Clear();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRowCo2());
    }

    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {
            if (concreteTiles[column - 1, row] != null)
            {
                BackgroundTile concreteTile = concreteTiles[column - 1, row];
                concreteTile.TakeDamage(1);
                if (concreteTile.hitPoints <= 0)
                {
                    concreteTiles[column - 1, row] = null;
                    concreteTile.Die();
                }
            }
        }
        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row] != null)
            {
                BackgroundTile concreteTile = concreteTiles[column + 1, row];
                concreteTile.TakeDamage(1);
                if (concreteTile.hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                    concreteTile.Die();
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row - 1] != null)
            {
                BackgroundTile concreteTile = concreteTiles[column, row - 1];
                concreteTile.TakeDamage(1);
                if (concreteTile.hitPoints <= 0)
                {
                    concreteTiles[column, row - 1] = null;
                    concreteTile.Die();
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1] != null)
            {
                BackgroundTile concreteTile = concreteTiles[column, row + 1];
                concreteTile.TakeDamage(1);
                if (concreteTile.hitPoints <= 0)
                {
                    concreteTiles[column, row + 1] = null;
                    concreteTile.Die();
                }
            }
        }
    }

    private void DamageSlime(int column, int row)
    {
        if (column > 0)
        {
            if (slimeTiles[column - 1, row] != null)
            {
                BackgroundTile tile = slimeTiles[column - 1, row];
                tile.TakeDamage(1);
                if (tile.hitPoints <= 0)
                {
                    slimeTiles[column - 1, row] = null;
                    tile.Die();
                }
                makeSlime = false;
            }
        }
        if (column < width - 1)
        {
            if (slimeTiles[column + 1, row] != null)
            {
                BackgroundTile tile = slimeTiles[column + 1, row];
                tile.TakeDamage(1);
                if (tile.hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                    tile.Die();
                }
                makeSlime = false;
            }
        }
        if (row > 0)
        {
            if (slimeTiles[column, row - 1] != null)
            {
                BackgroundTile tile = slimeTiles[column, row - 1];
                tile.TakeDamage(1);
                if (tile.hitPoints <= 0)
                {
                    slimeTiles[column, row - 1] = null;
                    tile.Die();
                }
                makeSlime = false;
            }
        }
        if (row < height - 1)
        {
            if (slimeTiles[column, row + 1] != null)
            {
                BackgroundTile tile = slimeTiles[column, row + 1];
                tile.TakeDamage(1);
                if (tile.hitPoints <= 0)
                {
                    slimeTiles[column, row + 1] = null;
                    tile.Die();
                }
                makeSlime = false;
            }
        }
    }

    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // if current spot isn't blank and is empty
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    // loop from the space above to the top of the column
                    for (int k = j + 1; k < height; k++)
                    {
                        // if a dot is found. . .
                        if (allDots[i, k] != null)
                        {
                            // move that dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            // set that spot to be null
                            allDots[i, k] = null;
                            // break out of the top
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    Dot dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity).GetComponent<Dot>();
                    dot.Setup(i,j, transform);
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].isMatched)
                    return true;
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return null;
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }
        currentDot = null;
        CheckToMakeSlime();
        if (IsDeadLocked())
        {
            StartCoroutine(ShuffleBoard());
        }
        yield return new WaitForSeconds(refillDelay);
        System.GC.Collect();
        if (currentState != EGameState.pause)
            currentState = EGameState.move;
        makeSlime = true;
        streakValue = 1;
    }

    private Vector2 CheckForAdjacent(int column, int row)
    {
        if (column < width - 1 && allDots[column + 1, row])
        {
            return Vector2.right;
        }
        if (column > 0 && allDots[column - 1, row])
        {
            return Vector2.left;
        }
        if (row < height - 1 && allDots[column, row + 1])
        {
            return Vector2.up;
        }
        if (row > 0 && allDots[column, row - 1])
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    private void MakeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        while (!slime && loops < 200)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (slimeTiles[x, y] != null)
            {
                Vector2 adjacent = CheckForAdjacent(x, y);
                if (adjacent != Vector2.zero)
                {
                    int newX = x + (int)adjacent.x;
                    int newY = y + (int)adjacent.y;
                    allDots[newX, newY].Die();
                    Vector2 tempPos = new Vector2(newX, newY);
                    GameObject tile = Instantiate(GameManager.Instance.slimeP, tempPos, Quaternion.identity);
                    slimeTiles[newX, newY] = tile.GetComponent<BackgroundTile>();
                    slime = true;
                }
            }
            loops++;
        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            Dot secondPiece = allDots[column + (int)direction.x, row + (int)direction.y].GetComponent<Dot>();
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = secondPiece;
        }
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        //Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        //Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j].gameObject);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        //for every spot on the board. . .
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot shouldn't be blank
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && slimeTiles[i, j])
                {
                    //Pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    //Assign the column to the piece
                    int maxIterations = 0;

                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    //Make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();

                    piece.column = i;
                    //Assign the row to the piece
                    piece.row = j;
                    //Fill in the dots array with this new piece
                    allDots[i, j] = newBoard[pieceToUse].GetComponent<Dot>();
                    //Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        //Check if it's still deadlocked
        if (IsDeadLocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }

}