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

    [Header("스텟 텍스트 연결")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI goldText;

    [Header("사운드 & 믹서 설정")]
    public AudioClip pauseClickSound;
    public AudioMixer masterMixer;

    [Header("옵션 UI 요소")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

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

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");

        if (pObj != null)
        {
            player = pObj.GetComponent<Player>();
            playerAttack = pObj.GetComponent<PlayerAttack>();
        }
        SyncUI();
    }

    private void SyncUI()
    {
        if (bgmSlider != null) bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        if (fullscreenToggle != null) fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
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
        masterMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
 

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
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