using UnityEngine;

public class CharacterDamageController : MonoBehaviour
{
    [System.Serializable]
    public struct BombAnimationSettings
    {
        [Tooltip("인스펙터에서 연결할 폭탄 오브젝트")]
        public GameObject bombObject;

        [Tooltip("이 폭탄에 맞았을 때 날릴 Trigger 파라미터 이름 (isMiddleDamage 등)")]
        public string animationParameterName;
    }

    private Animator animator;

    [Header("폭탄 오브젝트와 재생할 Trigger 애니메이션 매칭")]
    [SerializeField] private BombAnimationSettings[] bombSettingsList;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("캐릭터에 Animator 컴포넌트가 없습니다!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        BombAnimationSettings matchedSettings;
        if (TryGetBombSettings(collision.gameObject, out matchedSettings))
        {
            Debug.Log($"폭탄 충돌 확인! Trigger 발동: {matchedSettings.animationParameterName}");
            
            if (animator != null && !string.IsNullOrEmpty(matchedSettings.animationParameterName))
            {
                // 코루틴 없이 깔끔하게 한 방으로 트리거 작동!
                animator.SetTrigger(matchedSettings.animationParameterName);
            }
        }
    }

    private bool TryGetBombSettings(GameObject struckObject, out BombAnimationSettings resultSettings)
    {
        resultSettings = default;

        if (bombSettingsList == null || bombSettingsList.Length == 0) return false;

        foreach (var settings in bombSettingsList)
        {
            if (settings.bombObject == struckObject)
            {
                resultSettings = settings;
                return true;
            }
        }
        return false;
    }
}