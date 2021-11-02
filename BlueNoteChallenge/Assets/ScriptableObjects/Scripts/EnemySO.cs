using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Enemy")]
public class EnemySO : ScriptableObject
{
    [SerializeField]
    private int hitPoints;

    [SerializeField]
    private Color enemyTint;

    [SerializeField]
    private int pointAward;

    [SerializeField]
    private int enemyDamage;

    public int HitPoints
    {
        get { return hitPoints; }
    }

    public Color EnemyTint
    {
        get { return enemyTint; }
    }

    public int PointAward
    {
        get { return pointAward; }
    }

    public int EnemyDamage
    {
        get { return enemyDamage; }
    }
}
