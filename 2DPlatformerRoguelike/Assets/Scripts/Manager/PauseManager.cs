using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI 연결")]
    public GameObject pausePanel;
    public GameObject optionPanel;

    [Header("옵션 UI 요소")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    [Header("스텟 텍스트 연결")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI goldText;

    [Header("사운드 & 믹서 설정")]
    public AudioClip pauseClickSound;
    public AudioMixer masterMixer;

    bool isPaused = false;
    Player player;
    PlayerAttack playerAttack;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);

        if (Player.Instance != null)
        {
            player = Player.Instance;
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        SyncUI();
    }

    private void SyncUI()
    {
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bool isFull = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (bgmSlider != null) bgmSlider.value = bgm;
        if (sfxSlider != null) sfxSlider.value = sfx;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFull;

        SetBGMVolume(bgm);
        SetSFXVolume(sfx);

        SetFullscreen(isFull);
    }

    public void TogglePause()
    {
        if (pauseClickSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(pauseClickSound);
        }

        if (optionPanel != null && optionPanel.activeSelf)
        {
            CloseOptionPanel();
            return;
        }

        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        UpdateStatUI();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);

        bool isRewardOpen = (RewardManager.Instance != null && RewardManager.Instance.rewardPanel != null && RewardManager.Instance.rewardPanel.activeSelf);
        bool isShopOpen = (ShopManager.Instance != null && ShopManager.Instance.shopPanel != null && ShopManager.Instance.shopPanel.activeSelf);

        if (!isRewardOpen && !isShopOpen)
        {
            Time.timeScale = 1f;
        }
    }

    public void OpenOptionPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void CloseOptionPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void SetBGMVolume(float volume)
    {
        if (masterMixer != null)
        {
            float safeVolume = Mathf.Clamp(volume, 0.0001f, 1f);
            masterMixer.SetFloat("BGM", Mathf.Log10(safeVolume) * 20);
            PlayerPrefs.SetFloat("BGMVolume", volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (masterMixer != null)
        {
            float safeVolume = Mathf.Clamp(volume, 0.0001f, 1f);
            masterMixer.SetFloat("SFX", Mathf.Log10(safeVolume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.SetResolution(1280, 720, false);
        }

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLobby()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby");
    }

    private void UpdateStatUI()
    {
        if (player != null && playerAttack != null)
        {
            hpText.text = "HP : " + player.maxHP;
            attackText.text = "ATK : " + playerAttack.AttackDamage;
            goldText.text = "Gold : " + player.currentGold;
        }
    }
}