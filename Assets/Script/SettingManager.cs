using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    public float viewControl { get; private set; }
    public float reachControl { get; private set; }

    [SerializeField] Slider _viewControlSlider;
    [SerializeField] Slider _reachControlSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        viewControl = PlayerPrefs.GetFloat("viewControl", 0.5f);
        reachControl = PlayerPrefs.GetFloat("reachControl", 0.5f);
    }

    private void Start()
    {
        if (_viewControlSlider != null) _viewControlSlider.value = viewControl;
        if (_reachControlSlider != null) _reachControlSlider.value = reachControl;
    }

    public void SetViewControl(float value)
    {
        viewControl = value;
        PlayerPrefs.SetFloat("viewControl", value);
    }

    public void SetReachControl(float value)
    {
        reachControl = value;
        PlayerPrefs.SetFloat("reachControl", value);
    }
}
