using UnityEngine;

public class Defense : MonoBehaviour
{
    [Header("--- Stats ---")]
    public string entityName;
    public float maxHp = 30f;
    public float currentHp;
    public bool isDefending = false;

    protected virtual void Start()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDead()) return;

        float finalDamage = isDefending ? 0 : damage;

        currentHp -= finalDamage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (isDefending)
            Debug.Log($"<color=blue>[{entityName}]</color> 방어 성공! (데미지: 0) | 남은 HP: {currentHp}");
        else
            Debug.Log($"<color=red>[{entityName}]</color> 방어 실패! (데미지: {finalDamage}) | 남은 HP: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"<color=black><b>[{entityName}] 사망!</b></color>");
    }

    public bool IsDead() => currentHp <= 0;
}