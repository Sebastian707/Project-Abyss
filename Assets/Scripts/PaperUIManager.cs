using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaperUIManager : MonoBehaviour
{
    public static PaperUIManager Instance;

    [Header("UI References")]
    public GameObject paperPanel;
    public TextMeshProUGUI paperContentText;
    public Image paperBackgroundImage; // Reference to the Image component for background

    public static bool PaperIsOpen = false;

    private void Awake()
    {
        Instance = this;
        paperPanel.SetActive(false);

        // Add click listener to whole panel
        Button panelButton = paperPanel.GetComponent<Button>();
        if (panelButton == null)
        {
            panelButton = paperPanel.AddComponent<Button>();
        }
        panelButton.onClick.AddListener(ClosePaper);
    }

    public void ShowPaper(PaperItem paper)
    {
        // Set text and font
        paperContentText.text = paper.paperText;
        if (paper.font != null)
        {
            paperContentText.font = paper.font;
        }

        // Set background image
        if (paper.backgroundImage != null)
        {
            paperBackgroundImage.sprite = paper.backgroundImage;
            paperBackgroundImage.enabled = true;
        }
        else
        {
            paperBackgroundImage.enabled = false;
        }

        paperPanel.SetActive(true);
        PaperIsOpen = true;
    }

    public void ClosePaper()
    {
        paperPanel.SetActive(false);
        PaperIsOpen = false;
    }
}
