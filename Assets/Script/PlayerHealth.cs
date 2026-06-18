using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine(코루틴) 사용을 위해 추가

public class PlayerHealth : Defense
{
    [Header("--- UI Elements ---")]
    [SerializeField] private Slider healthSlider; // 슬라이더 UI

    [Header("--- Respawn Settings ---")]
    [SerializeField] private Transform respawnPoint;
    private CharacterController controller;

    [Header("--- Animation Settings (Bool 변경) ---")]
    [SerializeField] private Animator animator;
    [SerializeField] private string hitBoolParam = "Hit";             // 이제 Trigger가 아닌 Bool 파라미터 이름입니다.
    [SerializeField] private string attackBoolParam = "Attack";       // 이제 Trigger가 아닌 Bool 파라미터 이름입니다.
    [SerializeField] private string attackDirParam = "AttackDir";
    [SerializeField] private string defendBoolParam = "IsDefending";

    [Header("--- Input Settings (공격 키) ---")]
    [SerializeField] private KeyCode leftAttackKey = KeyCode.A;   // 좌측 공격
    [SerializeField] private KeyCode centerAttackKey = KeyCode.S; // 중앙 공격
    [SerializeField] private KeyCode rightAttackKey = KeyCode.D;  // 우측 공격
    [SerializeField] private KeyCode defendKey = KeyCode.Mouse1;  // 방어 (마우스 우클릭)

    [Header("--- Animation Timing ---")]
    [SerializeField] private float attackResetDelay = 0.1f; // 공격 Bool을 켜두었다가 꺼줄 시간 (초)
    [SerializeField] private float hitResetDelay = 0.2f;    // 피격 Bool을 켜두었다가 꺼줄 시간 (초)

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected override void Start()
    {
        if (ScoreManager.Instance != null)
        {
            maxHp = ScoreManager.Instance.maxHealth;
        }

        base.Start();
        UpdateHealthSlider();
    }

    private void Update()
    {
        // 1. 공격 입력 처리
        HandleAttackInput();

        // 2. 방어 입력 처리
        HandleDefenseInput();
    }

    // ⚔️ 좌/중/우 공격 입력을 감지하고 Bool 파라미터를 제어하는 함수
    private void HandleAttackInput()
    {
        if (animator == null) return;

        // [좌측 공격]
        if (Input.GetKeyDown(leftAttackKey))
        {
            ExecuteAttack(1);
        }
        // [중앙 공격]
        else if (Input.GetKeyDown(centerAttackKey))
        {
            ExecuteAttack(2);
        }
        // [우측 공격]
        else if (Input.GetKeyDown(rightAttackKey))
        {
            ExecuteAttack(3);
        }
    }

    // 공격 방향을 정하고, Bool을 켰다가 일정 시간 뒤에 꺼주는 핵심 로직
    private void ExecuteAttack(int direction)
    {
        animator.SetInteger(attackDirParam, direction);

        // 이미 작동 중인 공격 코루틴이 있다면 중복 방지를 위해 멈춤
        StopCoroutine("ResetAttackBoolCo");
        // 공격 Bool 작동 시작!
        StartCoroutine(ResetAttackBoolCo());
    }

    private IEnumerator ResetAttackBoolCo()
    {
        animator.SetBool(attackBoolParam, true);  // Attack = true (화살표 통과!)
        yield return new WaitForSeconds(attackResetDelay); // 잠깐 대기 (트랜지션이 안전하게 넘어갈 시간)
        animator.SetBool(attackBoolParam, false); // Attack = false (다시 원래대로 원상복구)
    }

    // 🛡️ 방어 버튼 상태 동기화 함수
    private void HandleDefenseInput()
    {
        bool holdDefend = Input.GetKey(defendKey);
        isDefending = holdDefend;

        if (animator != null)
        {
            animator.SetBool(defendBoolParam, holdDefend);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // 방어 상태가 아닐 때 피격 Bool 작동
        if (!isDefending && damage > 0 && animator != null)
        {
            StopCoroutine("ResetHitBoolCo");
            StartCoroutine(ResetHitBoolCo());
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.TakeDamage(damage, isDefending);
        }

        UpdateHealthSlider();
    }

    private IEnumerator ResetHitBoolCo()
    {
        animator.SetBool(hitBoolParam, true);  // Hit = true (피격 화살표 통과!)
        yield return new WaitForSeconds(hitResetDelay); // 잠깐 대기
        animator.SetBool(hitBoolParam, false); // Hit = false (원상복구)
    }

    protected override void Die()
    {
        base.Die();

        if (controller != null) controller.enabled = false;
        if (respawnPoint != null) transform.position = respawnPoint.position;
        if (controller != null) controller.enabled = true;

        currentHp = maxHp;
        UpdateHealthSlider();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.currentHealth = ScoreManager.Instance.maxHealth;
            ScoreManager.Instance.UpdateUI();
        }

        if (animator != null)
        {
            animator.Rebind();
        }

        Debug.Log("플레이어가 부활하여 체력과 UI가 초기화되었습니다.");
    }

    public void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHp / maxHp;
        }
    }
}