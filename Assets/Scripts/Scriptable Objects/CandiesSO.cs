using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CandiesSO")]
public class CandiesSO : ScriptableObject
{
    public CandySO blueCandySO;
    public CandySO purpleCandySO;
    public CandySO redCandySO;
    public CandySO orangeCandySO;
    public GameObject colorCandyP;

    public Sprite colorCandySprite;
    public Sprite breakableSprite;
    public Sprite concreteSprite;
    public Sprite lockedSprite;
    public Sprite slimeSprite;

    public GameObject GetCandyPrefab(EDotColor dotColor, EDotType dotType = EDotType.normal)
    {
        CandySO candy = new CandySO();
        switch (dotColor)
        {
            case EDotColor.blue:
                candy = blueCandySO;
                break;

            case EDotColor.orange:
                candy = orangeCandySO;
                break;

            case EDotColor.purple:
                candy = purpleCandySO;
                break;

            case EDotColor.red:
                candy = redCandySO;
                break;

            case EDotColor.yellow:
                break;

            case EDotColor.green:
                break;
        }
        switch (dotType)
        {
            case EDotType.normal:
                return candy.normalP;

            case EDotType.row:
                return candy.rowP;

            case EDotType.column:
                return candy.columnP;

            case EDotType.adjacent:
                return candy.adjacentP;

            case EDotType.color:
                return colorCandyP;
        }

        return null;
    }

    public Sprite GetCandySprite(EDotColor dotColor, EDotType dotType = EDotType.normal,bool getCorrectSprite=false)
    {
        CandySO candy = new CandySO();
        switch (dotColor)
        {
            case EDotColor.blue:
                candy = blueCandySO;
                break;

            case EDotColor.orange:
                candy = orangeCandySO;
                break;

            case EDotColor.purple:
                candy = purpleCandySO;
                break;

            case EDotColor.red:
                candy = redCandySO;
                break;

            case EDotColor.yellow:
                break;

            case EDotColor.green:
                break;
        }
        switch (dotType)
        {
            case EDotType.normal:
                return GetRandomSprite(candy.normalSprite);

            case EDotType.row:
                if (getCorrectSprite)
                    return candy.rowSprite[0];
                else
                    return GetRandomSprite(candy.rowSprite);
           
            case EDotType.column:
                if (getCorrectSprite)
                    return candy.columnSprite[0];
                else
                    return GetRandomSprite(candy.columnSprite);

            case EDotType.adjacent:
                return GetRandomSprite(candy.adjacentSprite);

            case EDotType.color:
                return colorCandySprite;

            case EDotType.background:
                break;

            case EDotType.breakable:
                return breakableSprite;

            case EDotType.concrete:
                return concreteSprite;

            case EDotType.locked:
                return lockedSprite;

            case EDotType.slime:
                return slimeSprite;
        }

        return null;
    }

    private Sprite GetRandomSprite(Sprite[] sprites)
    {
        return sprites[Random.Range(0, sprites.Length)];
    }
}