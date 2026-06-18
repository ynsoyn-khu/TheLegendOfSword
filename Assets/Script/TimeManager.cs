using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    // 싱글톤 인스턴스 안전하게 보완
    public static TimeManager Instance;

    public enum TurnState { PlayerAttack, PlayerDefense }
    public TurnState currentTurnState = TurnState.PlayerAttack;

    [Header("--- Settings ---")]
    public float timePerPhase = 15f; // 공격/방어 각각 15초
    public int maxTurns = 4;        // 총 4턴

    [Header("--- Current Status ---")]
    public float currentTime;
    public int currentTurn = 1;
    public bool isGameOver = false;

    [Header("--- UI Elements ---")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI turnText;  // "Turn 1 / 4"
    public TextMeshProUGUI stateText; // "ATTACK" / "DEFENSE"
    public GameObject gameOverPanel;

    void Awake()
    {
        // 싱글톤 구조 안전 장치
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        ResetGame();
    }

    void ResetGame()
    {
        currentTurn = 1;
        currentTurnState = TurnState.PlayerAttack;
        currentTime = timePerPhase;
        isGameOver = false;
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // [연결] 매 프레임 플레이어나 적의 생사 여부를 체크하여 게임 오버 연동
        CheckEntitiesStatus();

        if (isGameOver) return;

        // 시간 감소 연산
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            // 15초가 다 지나 타임아웃이 되었을 때의 처리
            bool wasAttackPhase = (currentTurnState == TurnState.PlayerAttack);
            
            // 페이즈를 강제로 전환합니다.
            NextPhase();

            // [연결 버그 수정] 플레이어가 공격 키를 안 누르고 15초 동안 잠수타서 방어 턴으로 넘어간 경우,
            // Attack 스크립트가 스스로 실행하지 못하는 '적 공격 패턴'을 대행하여 예약 실행해 줍니다.
            if (wasAttackPhase && currentTurnState == TurnState.PlayerDefense)
            {
                Attack attackManager = FindObjectOfType<Attack>();
                if (attackManager != null)
                {
                    attackManager.CancelInvoke("EnemyAttackLogic"); // 중복 실행 방지
                    attackManager.Invoke("EnemyAttackLogic", 0.5f);
                }
            }
        }

        UpdateUI();
    }

    // 단계 전환 (공격 -> 방어 또는 방어 -> 다음 턴 공격)
    public void NextPhase()
    {
        if (isGameOver) return;

        if (currentTurnState == TurnState.PlayerAttack)
        {
            // 공격 끝 -> 방어 시작
            currentTurnState = TurnState.PlayerDefense;
            currentTime = timePerPhase;
            Debug.Log($"턴 {currentTurn}: 방어 시작!");
        }
        else
        {
            // 방어 끝 -> 다음 턴 공격 시작
            if (currentTurn < maxTurns)
            {
                currentTurn++;
                currentTurnState = TurnState.PlayerAttack;
                currentTime = timePerPhase;
                Debug.Log($"턴 {currentTurn}: 공격 시작!");
            }
            else
            {
                // 4턴 방어까지 모두 끝남 -> 타임아웃 게임 종료
                FinishGame();
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeText != null) timeText.text = $"Time: {Mathf.Ceil(currentTime)}s";
        if (turnText != null) turnText.text = $"Turn: {currentTurn} / {maxTurns}";
        if (stateText != null)
        {
            stateText.text = (currentTurnState == TurnState.PlayerAttack) ? 
                "<color=red>ATTACK</color>" : "<color=blue>DEFENSE</color>";
        }
    }

    // 다른 스크립트(PlayerHealth 등)에서 개체가 죽었을 때 즉시 실시간 호출할 수 있도록 public 확장
    public void FinishGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("전투 조건 종료! 게임 시스템 정지.");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // 플레이어 개체들의 실시간 사망 상태를 체크하는 감지 시스템
    private void CheckEntitiesStatus()
    {
        Attack attackManager = FindObjectOfType<Attack>();
        if (attackManager != null)
        {
            // 리스폰 구역이 없는 적(Enemy)이 완벽하게 쓰러졌다면 즉시 스테이지 클리어 종료
            if (attackManager.enemy != null && attackManager.enemy.IsDead())
            {
                Debug.Log("<color=green><b>전투 승리! 적을 처치했습니다.</b></color>");
                FinishGame();
            }
        }
    }
}