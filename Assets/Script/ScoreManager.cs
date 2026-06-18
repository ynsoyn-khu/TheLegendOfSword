using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ScoreManager Instance;

    [Header("--- Health Settings ---")]
    public float maxHealth = 30f; // 최대 체력
    public float currentHealth;   // 현재 체력

    [Header("--- UI Elements ---")]
    public TextMeshProUGUI HealthText; // UI 텍스트 (TMP)

    // 동기화를 위해 씬에 있는 플레이어 스크립트를 참조합니다.
    private PlayerHealth playerHealth;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 체력 초기화
        currentHealth = maxHealth;
        UpdateUI();

        // 씬에서 플레이어 컴포넌트를 찾아 연결합니다.
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // 포션 섭취 등 외부에서 체력을 회복시킬 때 호출 (기존 AddScore 역할)
    public void AddHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth; // 최대치 초과 방지
        
        Debug.Log("체력 회복! 현재 체력 : " + currentHealth);
        UpdateUI();

        // [연결] 플레이어 개체의 체력과 슬라이더 UI도 함께 동기화합니다.
        if (playerHealth != null)
        {
            playerHealth.currentHp = currentHealth;
            playerHealth.UpdateHealthSlider();
        }
    }

    // 플레이어가 공격을 받았을 때 호출 (PlayerHealth에서 안전하게 넘겨받음)
    public void TakeDamage(float attackScore, bool isDefending)
    {
        // 방어 성공 시 데미지 0, 실패 시 데미지 적용
        float finalDamage = isDefending ? 0 : attackScore;
        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("<color=red>플레이어 사망!</color>");
            // 사망 처리는 물리적인 리스폰을 담당하는 PlayerHealth에서 처리하도록 유도됨
        }

        Debug.Log($"[ScoreManager] 피격! 데미지: {finalDamage} | 남은 체력: {currentHealth}");
        UpdateUI();
    }

    // UI 텍스트 업데이트 로직
    public void UpdateUI()
    {
        if (HealthText != null)
        {
            // 예: "HP : 25 / 30" 형식으로 표시
            HealthText.text = "HP : " + currentHealth + " / " + maxHealth;
        }
    }
}