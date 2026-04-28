using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class OptionManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject optionPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    [Header("오디오 믹서 연결")]
    public AudioMixer masterMixer;

    private void Start()
    {
        if (optionPanel != null) optionPanel.SetActive(false);

        LoadSettings();
    }

    private void LoadSettings()
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

    public void OpenOption() { if (optionPanel != null) optionPanel.SetActive(true); }
    public void CloseOption() { if (optionPanel != null) optionPanel.SetActive(false); }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && optionPanel != null && optionPanel.activeSelf)
        {
            CloseOption();
        }
    }
}