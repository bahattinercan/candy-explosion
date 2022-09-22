using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CandySO")]
public class CandySO : ScriptableObject
{    
    public GameObject normalP;
    public GameObject rowP;
    public GameObject columnP;
    public GameObject adjacentP;

    [Space]
    public Sprite[] normalSprite;
    public Sprite[] rowSprite;
    public Sprite[] columnSprite;
    public Sprite[] adjacentSprite;
}
