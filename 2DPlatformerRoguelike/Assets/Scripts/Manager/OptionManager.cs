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

    public void OpenOption()
    {
        if (optionPanel != null) optionPanel.SetActive(true);
    }

    public void CloseOption()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
    }

    public void SetBGMVolume(float volume)
    {
        masterMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0); 
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && optionPanel != null && optionPanel.activeSelf)
        {
            CloseOption();
        }
    }
}