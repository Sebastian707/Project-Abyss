using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmailListItem : MonoBehaviour
{
    public TextMeshProUGUI subjectText;
    public TextMeshProUGUI senderText;
    public Button button;

    EmailData email;

    public void Setup(EmailData data, System.Action<EmailData> onClick)
    {
        email = data;

        subjectText.text = data.subject;
        senderText.text = data.sender;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(email));
    }
}