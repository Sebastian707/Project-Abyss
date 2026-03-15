using UnityEngine;
using TMPro;

public class EmailViewer : MonoBehaviour
{
    public TextMeshProUGUI subject;
    public TextMeshProUGUI sender;
    public TextMeshProUGUI sentTo;
    public TextMeshProUGUI cc;
    public TextMeshProUGUI date;
    public TextMeshProUGUI content;

    public void DisplayEmail(EmailData email)
    {
        subject.text = email.subject;
        sender.text = email.sender;
        sentTo.text = email.sentTo;
        cc.text = email.cc;
        date.text = email.date;
        content.text = email.content;

        email.isUnread = false;
    }
}