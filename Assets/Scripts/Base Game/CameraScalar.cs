using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = .625f;
    public float padding = 2;
    public float yOffset = 1;

    // Start is called before the first frame update
    private void Start()
    {
        board = Board.Instance;
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    private void RepositionCamera(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPos;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
}