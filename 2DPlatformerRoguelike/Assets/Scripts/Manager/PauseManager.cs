using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI 연결")]
    public GameObject pausePanel;

    [Header("스텟 텍스트 연결")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI goldText;

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

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");

        if (pObj != null) 
        {
            player = pObj.GetComponent<Player>();
            playerAttack = pObj.GetComponent<PlayerAttack>();
        }
    }

    public void TogglePause()
    {
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
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
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
        if(player != null && playerAttack != null)
        {
            hpText.text = "HP : " + player.maxHP;
            attackText.text = "ATK : " + playerAttack.AttackDamage;
            goldText.text = "Gold : " + player.currentGold;
        }
    }

}
