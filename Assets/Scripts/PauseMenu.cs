using UnityEngine;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject PauseObjectMenu;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject ConfirmMenu;

    public void OnDisable()
    {
        PauseObjectMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }


    public void OnSettings()
    {
        PauseObjectMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void OnReturn()
    {
        PauseObjectMenu.SetActive(true);
        SettingsMenu.SetActive(false);

    }

    public void OnQuit()
    {
        ConfirmMenu.SetActive(true);
    }

    public void OnConfirmQuit()
    {
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void OnConfirmDeny()
    {
        ConfirmMenu.SetActive(false);

    }
}
