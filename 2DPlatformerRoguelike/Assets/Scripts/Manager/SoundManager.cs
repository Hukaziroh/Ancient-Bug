using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM 설정")]
    public AudioSource bgmSource;
    public AudioClip lobbyBgm; 

    [Header("SFX 설정")]
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            if (lobbyBgm != null)
            {
                PlayBGM(lobbyBgm);
            }
        }
    }

    public void PlayBGM(AudioClip newClip)
    {
        if (bgmSource == null || newClip == null) return;

        if (bgmSource.clip == newClip) return;

        bgmSource.clip = newClip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip);
    }
}