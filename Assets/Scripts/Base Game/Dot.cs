using System.Collections;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;
    public Dot otherDot;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public EDotColor dotColor;
    public EDotType dotType;

    #region Base Functions
    // Start is called before the first frame update
    private void Start()
    {
        board = Board.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        targetX = column;
        targetY = row;
        // X Movement
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {   // Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != gameObject)
            {
                board.allDots[column, row] = this;
                FindMatches.Instance.FindAllMatches();
            }
        }
        else
        {   // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        // Y Movement
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {   // Move Towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != gameObject)
            {
                board.allDots[column, row] = this;
                FindMatches.Instance.FindAllMatches();
            }
        }
        else
        {   // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    private void OnMouseDown()
    {
        if (HintManager.Instance != null)
            HintManager.Instance.DestroyHint();

        if (board.currentState == EGameState.move)
        {
            if (GameManager.Instance.useMouse)
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == EGameState.move)
        {
            if (GameManager.Instance.useMouse)
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            CalculateAngle();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void Setup(int x,int y,Transform parentTransform)
    {
        row = y;
        column = x;
        transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = GameManager.Instance.candiesSO.GetCandySprite(dotColor);
        transform.SetParent(parentTransform);
        name = "( " + x + ", " + y + " )";
    }
    #endregion
    private IEnumerator StartShineCo()
    {
        yield return null;
    }

    public void PopAnimation()
    {
    }

    public IEnumerator CheckMoveCo()
    {
        if (dotType==EDotType.color)
        {
            FindMatches.Instance.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if (otherDot.dotType==EDotType.color)
        {
            FindMatches.Instance.MatchPiecesOfColor(tag);
            otherDot.isMatched = true;
        }

        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.isMatched)
            {
                otherDot.row = row;
                otherDot.column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = EGameState.move;
            }
            else
            {
                if (EndGameManager.Instance != null)
                {
                    if (EndGameManager.Instance.requirements.gameType == EGameType.moves)
                    {
                        EndGameManager.Instance.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();
            }
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || 
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = EGameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        }
        else
        {
            board.currentState = EGameState.move;
        }
    }

    public string SwipeAngle()
    {
        string angle = "";
        if (swipeAngle > -45 && swipeAngle <= 45)
        {   // Right Swipe
            angle = "r";
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {   // Up Swipe
            angle = "u";
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {   // Left Swipe
            angle = "l";
        }
        else if (swipeAngle < -45 && swipeAngle >= -135)
        {   // Down Swipe
            angle = "d";
        }
        return angle;
    }

    public bool CheckSwipeAngle(string angle)
    {
        if (angle == SwipeAngle())
            return true;
        return false;
    }

    private void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherDot != null)
            {
                otherDot.column += -1 * (int)direction.x;
                otherDot.row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = EGameState.move;
            }
        }
    }

    private void MovePieces()
    {
        if (CheckSwipeAngle("r") && column < board.width - 1)
        {   // Right Swipe
            MovePiecesActual(Vector2.right);
        }
        else if (CheckSwipeAngle("u") && row < board.height - 1)
        {   // Up Swipe
            MovePiecesActual(Vector2.up);
        }
        else if (CheckSwipeAngle("l") && column > 0)
        {   // Left Swipe
            MovePiecesActual(Vector2.left);
        }
        else if (CheckSwipeAngle("d") && row > 0)
        {   // Down Swipe
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = EGameState.move;
        }
    }

    /*
    private void FindMatchess()
    {
        if (column > 0 && column < board.width - 1)
        {
            Dot leftDot = board.allDots[column - 1, row];
            Dot rightDot = board.allDots[column + 1, row];
            if (leftDot != null && rightDot != null)
                if (leftDot.CompareTag(tag) && rightDot.CompareTag(tag))
                {
                    leftDot.isMatched = true;
                    rightDot.isMatched = true;
                    isMatched = true;
                }
        }
        if (row > 0 && row < board.height - 1)
        {
            Dot upDot = board.allDots[column, row + 1];
            Dot downDot = board.allDots[column, row - 1];
            if (upDot != null && downDot != null)
                if (upDot.CompareTag(tag) && downDot.CompareTag(tag))
                {
                    upDot.isMatched = true;
                    downDot.isMatched = true;
                    isMatched = true;
                }
        }
    }
    */

    public void MakeRowBomb()
    {
        if (dotType == EDotType.normal)
        {
            dotType = EDotType.row;
            GameObject go = 
                Instantiate(GameManager.Instance.candiesSO.GetCandyPrefab(dotColor, dotType), transform.position, Quaternion.identity);
            go.transform.SetParent(transform);
            SetAdvancedDotSprite(go.transform.Find("sprite").GetComponent<SpriteRenderer>());
            HideNormalDotSprite();
        }
    }

    public void MakeColumnBomb()
    {
        if (dotType == EDotType.normal)
        {
            dotType = EDotType.column;
            GameObject go = 
                Instantiate(GameManager.Instance.candiesSO.GetCandyPrefab(dotColor, dotType), transform.position, Quaternion.identity);
            go.transform.SetParent(transform);
            SetAdvancedDotSprite(go.transform.Find("sprite").GetComponent<SpriteRenderer>());
            HideNormalDotSprite();
        }
    }

    public void MakeColorBomb()
    {
        if (dotType == EDotType.normal)
        {
            dotType = EDotType.color;
            GameObject go = 
                Instantiate(GameManager.Instance.candiesSO.GetCandyPrefab(dotColor, dotType), transform.position, Quaternion.identity);
            go.transform.SetParent(transform);
            go.tag = "ColorBomb";
            HideNormalDotSprite();
        }
    }

    public void MakeAdjacentBomb()
    {
        if (dotType == EDotType.normal)
        {
            dotType = EDotType.adjacent;
            GameObject go = 
                Instantiate(GameManager.Instance.candiesSO.GetCandyPrefab(dotColor, dotType), transform.position, Quaternion.identity);
            go.transform.SetParent(transform);
            SetAdvancedDotSprite(go.transform.Find("sprite").GetComponent<SpriteRenderer>());
            HideNormalDotSprite();
        }
    }

    public void HideNormalDotSprite()
    {
        transform.Find("sprite").GetComponent<SpriteRenderer>().enabled = false;
    }

    public void SetAdvancedDotSprite(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.sprite = GameManager.Instance.candiesSO.GetCandySprite(dotColor, dotType);
    }
}