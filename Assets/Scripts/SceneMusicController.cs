using UnityEngine;

public class SceneMusicController : MonoBehaviour
{
    [SerializeField] private MusicTrack musicTrack;

    private void Start()
    {
        if (musicTrack != null)
        {
            AudioManager.Instance.PlayMusic(musicTrack.clip);
        }
    }
}