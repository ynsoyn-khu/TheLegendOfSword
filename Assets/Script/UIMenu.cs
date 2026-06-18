using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] Button _loadNext;

    // Start is called before the first frame update
    void Start()
    {
        _loadNext.onClick.AddListener(loadNext);
      
    }

    private void loadNext()
    {
        ScenesManager.Instance.LoadNextScene();
    }
    
}