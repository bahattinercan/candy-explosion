using System.Collections;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameInfoPanel;

    public void OK()
    {
        if (panelAnim != null && gameInfoPanel != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoPanel.SetBool("Out", true);
            StartCoroutine(GameStartCo());
        }
    }

    public void GameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);
    }

    private IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        Board.Instance.currentState = EGameState.move;
    }
}