using UnityEngine;
using UnityEngine.Video;

public class IntroVideoPlayer : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;

    private void Start()
    {
        if (_videoPlayer == null) { Debug.LogError("VideoPlayer가 연결되지 않았습니다."); return; }
        _videoPlayer.loopPointReached += OnVideoEnd;
        _videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        LoadNext();
    }

    public void LoadNext()
    {
        _videoPlayer.Stop();
        GameSceneManager.Instance.LoadNextScene();
    }
}
