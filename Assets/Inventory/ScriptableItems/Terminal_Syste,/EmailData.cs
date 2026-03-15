using UnityEngine;

[CreateAssetMenu(fileName = "NewEmail", menuName = "ImmersiveSim/Email")]
public class EmailData : ScriptableObject
{
    [Header("Header Info")]
    public string subject;
    public string sender;
    public string sentTo;
    public string cc;
    public string date;

    [Header("Body")]
    [TextArea(10, 20)]
    public string content;

    [Header("Optional")]
    public bool isUnread = true;
}