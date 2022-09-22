using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer spriteRenderer;
    public EDotType dotType;

    private void Start()
    {
        spriteRenderer = transform.Find("sprite").GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    public void Die()
    {
        if (GoalManager.Instance != null)
        {
            GoalManager.Instance.CompareGoal(dotType);
            GoalManager.Instance.UpdateGoals();
        }
        Destroy(this.gameObject);
    }

    private void MakeLighter()
    {
        Color c = spriteRenderer.color;
        float new_a = c.a * .5f;
        spriteRenderer.color = new Color(c.r, c.g, c.b, new_a);
    }
}