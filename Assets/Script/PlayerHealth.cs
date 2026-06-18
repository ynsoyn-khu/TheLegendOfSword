using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Defense
{
    [Header("--- UI Elements ---")]
    [SerializeField] private Slider healthSlider; // 슬라이더 UI

    [Header("--- Respawn Settings ---")]
    [SerializeField] private Transform respawnPoint;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    protected override void Start()
    {
        // [연결] ScoreManager에 세팅된 최대 체력 값이 있다면 그것으로 동기화합니다.
        if (ScoreManager.Instance != null)
        {
            maxHp = ScoreManager.Instance.maxHealth;
        }

        base.Start(); // 부모(Defense)의 hp 초기화 로직 실행
        UpdateHealthSlider();
    }

    public override void TakeDamage(float damage)
    {
        // 1. 부모(Defense)의 기본 피격/방어 연산 수행 (currentHp 차감 처리됨)
        base.TakeDamage(damage); 

        // 2. [연결] 전역 ScoreManager에도 데미지 정보를 전달하여 텍스트 UI를 동기화합니다.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.TakeDamage(damage, isDefending);
        }

        // 3. 슬라이더 바 업데이트
        UpdateHealthSlider();        
    }

    protected override void Die()
    {
        base.Die(); // "[플레이어] 사망!" 콘솔 로그 출력
        
        // 캐릭터 컨트롤러를 잠시 끄고 리스폰 위치로 이동
        if (controller != null) controller.enabled = false;
        if (respawnPoint != null) transform.position = respawnPoint.position;
        if (controller != null) controller.enabled = true;

        // 리스폰 시 자체 체력 풀 회복 및 슬라이더 갱신
        currentHp = maxHp;
        UpdateHealthSlider();

        // [연결] 리스폰 시 전역 ScoreManager의 체력 데이터와 텍스트 UI도 풀 회복 상태로 만듭니다.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.currentHealth = ScoreManager.Instance.maxHealth;
            ScoreManager.Instance.UpdateUI();
        }

        Debug.Log("플레이어가 부활하여 체력과 UI가 초기화되었습니다.");
    }

    // 자체 슬라이더 UI를 갱신하는 함수
    public void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHp / maxHp;
        }
    }
}