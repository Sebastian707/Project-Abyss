using UnityEngine;
using TMPro;

public class TerminalLogin : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loginScreen;
    public GameObject desktopScreen;

    public TMP_InputField passwordInput;
    public TMP_Text errorText;

    [Header("Password")]
    public string correctPassword = "1234";

    void Start()
    {
        desktopScreen.SetActive(false);
        errorText.text = "";
    }

    void Update()
    {
        // Press Enter to login
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryLogin();
        }
    }

    public void TryLogin()
    {
        if (passwordInput.text == correctPassword)
        {
            LoginSuccess();
        }
        else
        {
            errorText.text = "Incorrect password";
            passwordInput.text = "";
        }
    }

    void LoginSuccess()
    {
        loginScreen.SetActive(false);
        desktopScreen.SetActive(true);
    }
}