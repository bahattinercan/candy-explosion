[System.Serializable]
public class Serializable_Level
{
    public int id;
    public bool canPlay;
    public int score;
    public int star;

    public Serializable_Level(int id)
    {
        this.id = id;
        canPlay = false;
        score = 0;
        star = 0;
    }
}