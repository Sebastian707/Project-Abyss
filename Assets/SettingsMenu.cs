using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject DisplayMenu;
    [SerializeField] private GameObject AudioMenu;
    public AudioMixer audioMixer;
   
    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }


    public void SetFullscreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

 
    public void OnAudio()
    {
        DisplayMenu.SetActive(false);
        AudioMenu.SetActive(true);
    }

    public void OnDisplay()
    {
        AudioMenu.SetActive(false);
        DisplayMenu.SetActive(true);
    }


}
