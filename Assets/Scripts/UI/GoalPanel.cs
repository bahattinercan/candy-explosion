using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public Image image;
    public Text text;
    public Sprite mySprite;
    public string myString;

    // Start is called before the first frame update
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        image.sprite = mySprite;
        image.preserveAspect = true;
        text.text = myString;
    }
}