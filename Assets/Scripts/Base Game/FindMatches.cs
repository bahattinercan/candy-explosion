using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private static FindMatches instance;
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    public static FindMatches Instance { get => instance; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        board = Board.Instance;
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.dotType==EDotType.adjacent)
            currentDots.Union(GetAdjacentPieces(dot1.column, dot1.row));

        if (dot2.dotType == EDotType.adjacent)
            currentDots.Union(GetAdjacentPieces(dot2.column, dot2.row));

        if (dot3.dotType == EDotType.adjacent)
            currentDots.Union(GetAdjacentPieces(dot3.column, dot3.row));
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.dotType == EDotType.row)
        {
            currentDots.Union(GetRowPieces(dot1.row));
            board.BombRow(dot1.row);
        }

        if (dot2.dotType == EDotType.row)
        {
            currentDots.Union(GetRowPieces(dot2.row));
            board.BombRow(dot2.row);
        }

        if (dot3.dotType == EDotType.row)
        {
            currentDots.Union(GetRowPieces(dot3.row));
            board.BombRow(dot3.row);
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.dotType == EDotType.column)
        {
            currentDots.Union(GetColumnPieces(dot1.column));
            board.BombColumn(dot1.column);
        }

        if (dot2.dotType == EDotType.column)
        {
            currentDots.Union(GetColumnPieces(dot2.column));
            board.BombColumn(dot2.column);
        }

        if (dot3.dotType == EDotType.column)
        {
            currentDots.Union(GetColumnPieces(dot3.column));
            board.BombColumn(dot3.column);
        }
        return currentDots;
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return null;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                Dot currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    // Vertical Check
                    if (i > 0 && i < board.width - 1)
                    {
                        Dot leftDot = board.allDots[i - 1, j];
                        Dot rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.CompareTag(currentDot.tag) && rightDot.CompareTag(currentDot.tag))
                            {
                                currentMatches.Union(IsRowBomb(leftDot, currentDot, rightDot));
                                currentMatches.Union(IsColumnBomb(leftDot, currentDot, rightDot));
                                currentMatches.Union(IsAdjacentBomb(leftDot, currentDot, rightDot));

                                AddMatches(currentDot);
                                AddMatches(leftDot);
                                AddMatches(rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        Dot upDot = board.allDots[i, j + 1];
                        Dot downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag))
                            {
                                currentMatches.Union(IsRowBomb(downDot, currentDot, upDot));
                                currentMatches.Union(IsColumnBomb(downDot, currentDot, upDot));
                                currentMatches.Union(IsAdjacentBomb(downDot, currentDot, upDot));

                                AddMatches(currentDot);
                                AddMatches(upDot);
                                AddMatches(downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddMatches(Dot dot)
    {
        if (!currentMatches.Contains(dot.gameObject))
        {
            currentMatches.Add(dot.gameObject);
        }
        dot.isMatched = true;
    }

    private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row];
                if (dot.dotType == EDotType.column)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i, row].gameObject);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    private List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i];
                if (dot.dotType == EDotType.row)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                dots.Add(board.allDots[column, i].gameObject);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    public List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j].gameObject);
                        board.allDots[i, j].isMatched = true;
                    }
                }
            }
        }
        return dots;
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    Dot dot = board.allDots[i, j];
                    if (dot.gameObject.CompareTag(color))
                    {
                        dot.isMatched = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// is checking for column or row bomb and create it
    /// </summary>
    public void CheckBombs(MatchType matchType)
    {
        // did the player move something?
        if (board.currentDot != null)
        {
            Dot currentDot = board.currentDot;
            // is the piece they moved matched?
            if (currentDot.isMatched && currentDot.CompareTag(matchType.color))
            {
                // make it unmatched
                currentDot.isMatched = false;
                if (currentDot.CheckSwipeAngle("r") || currentDot.CheckSwipeAngle("l"))
                {
                    currentDot.MakeRowBomb();
                }
                else
                {
                    currentDot.MakeColumnBomb();
                }
            }
            // is the other piece matched?
            else if (currentDot.otherDot != null)
            {
                Dot otherDot = currentDot.otherDot;
                if (otherDot.isMatched && otherDot.CompareTag(matchType.color))
                {
                    otherDot.isMatched = false;
                    if (currentDot.CheckSwipeAngle("r") || currentDot.CheckSwipeAngle("l"))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}