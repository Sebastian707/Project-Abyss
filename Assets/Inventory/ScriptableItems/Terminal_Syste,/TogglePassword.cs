using UnityEngine;
using TMPro;

public class PasswordVisibilityToggle : MonoBehaviour
{
    public TMP_InputField passwordField;
    private bool isVisible = false;

    public void TogglePassword()
    {
        isVisible = !isVisible;

        // Reset content type first
        passwordField.contentType = TMP_InputField.ContentType.Standard;

        if (isVisible)
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
        }

        passwordField.ForceLabelUpdate();
    }
}