using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class goStart : MonoBehaviour
{
    [SerializeField] Button _loadNext;
    [SerializeField] Button _loadStart;

    // Start is called before the first frame update
    void Start()
    {
        _loadNext.onClick.AddListener(loadNext);
        _loadStart.onClick.AddListener(loadStart);
    }

    private void loadNext()
    {
        ScenesManager.Instance.LoadNextScene();
    }
    private void loadStart()
    {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.Level00);
    }
}