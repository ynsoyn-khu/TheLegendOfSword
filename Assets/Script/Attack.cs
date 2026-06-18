using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("--- References ---")]
    public Defense player; // 여기에 PlayerHealth 스크립트를 드래그앤드롭 해도 작동합니다!
    public Defense enemy;  

    [Header("--- Player Status ---")]
    public int attackCount = 0;
    public const int MaxAttackCountForUltimate = 5;

    void Update()
    {
        if (player == null || enemy == null) return;
        if (player.IsDead() || enemy.IsDead() || TimeManager.Instance.isGameOver) return;

        TimeManager.TurnState state = TimeManager.Instance.currentTurnState;

        // 1. [방어 턴] 로직
        if (state == TimeManager.TurnState.PlayerDefense)
        {
            player.isDefending = Input.GetKey(KeyCode.Space);
        }
        else
        {
            player.isDefending = false;
        }

        // 2. [공격 턴] 로직
        if (state == TimeManager.TurnState.PlayerAttack)
        {
            if (Input.GetKeyDown(KeyCode.A)) ExecutePlayerAttack("좌 공격", 1f, false);
            if (Input.GetKeyDown(KeyCode.S)) ExecutePlayerAttack("중 공격", 2f, false);
            if (Input.GetKeyDown(KeyCode.D)) ExecutePlayerAttack("우 공격", 1f, false);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (attackCount >= MaxAttackCountForUltimate) 
                {
                    ExecutePlayerAttack("필살기", 5f, true);
                }
                else 
                {
                    Debug.Log("<color=yellow>필살기 불가!</color> 공격 횟수 부족.");
                }
            }
        }
    }

    void ExecutePlayerAttack(string attackName, float damage, bool isUltimate)
    {
        Debug.Log($"<color=green>[플레이어 공격]</color> {attackName}!");
        
        if (isUltimate) attackCount = 0;
        else attackCount++;

        enemy.isDefending = (Random.value < 0.3f);
        enemy.TakeDamage(damage);

        FinishPhase();
    }

    void FinishPhase()
    {
        TimeManager.Instance.NextPhase();

        if (TimeManager.Instance.currentTurnState == TimeManager.TurnState.PlayerDefense)
        {
            Invoke("EnemyAttackLogic", 0.5f);
        }
    }

    void EnemyAttackLogic()
    {
        if (enemy.IsDead() || TimeManager.Instance.isGameOver) return;

        int randomAttack = Random.Range(0, 4);
        float damage = (randomAttack == 3) ? 5f : (randomAttack == 1 ? 2f : 1f);
        string[] names = { "좌 공격", "중 공격", "우 공격", "필살기" };

        Debug.Log($"<color=red>[적 공격]</color> {names[randomAttack]} 시도!");
        player.TakeDamage(damage);

        TimeManager.Instance.NextPhase(); 
    }
}