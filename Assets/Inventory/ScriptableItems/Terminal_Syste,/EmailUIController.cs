using UnityEngine;

public class EmailUIController : MonoBehaviour
{
    public ComputerEmails computerEmails;

    public Transform listParent;
    public EmailListItem itemPrefab;
    public int chosenEmail;

    public EmailViewer viewer;

    void Start()
    {
        PopulateList();

        // Automatically open the first email
        if (computerEmails.emails.Count > 0)
        {
            OnEmailClicked(computerEmails.emails[chosenEmail]);
        }
    }

    void PopulateList()
    {
        // Clear existing items (safety if refreshed)
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        // Create list items
        foreach (var email in computerEmails.emails)
        {
            var item = Instantiate(itemPrefab, listParent);
            item.Setup(email, OnEmailClicked);
        }
    }

    void OnEmailClicked(EmailData email)
    {
        viewer.DisplayEmail(email);
    }
}