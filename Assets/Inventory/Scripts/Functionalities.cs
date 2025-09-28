using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functionalities : MonoBehaviour //inventoryTab
{
    [Header("Inventory Opening")]
    public GameObject inventoryTab;
    private Vector2 finalPos;
    private Vector2 startPos;

    [Header("Item Description")]
    public Text itemTitle;
    public Text itemBody;
    public Text atributte1;
    public Image image_atributte1;

    [Header("Player Reference")]
    public PlayerController playerController; // reference to disable/enable movement

    bool active = false;

    private void Start()
    {
        // clean description
        image_atributte1.enabled = false;
        itemTitle.text = "";
        itemBody.text = "";
        atributte1.text = "";

        startPos = new Vector2(1153f, -275f);
        finalPos = new Vector2(400f, 225f);
        inventoryTab.GetComponent<RectTransform>().anchoredPosition = startPos;
    }

    private void Update()
    {
        // toggle inventory using key I
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (active)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    private void OpenInventory()
    {
        active = true;
        inventoryTab.GetComponent<RectTransform>().anchoredPosition = finalPos;

        // pause game
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // disable player movement
        if (playerController != null) playerController.DisableMovement();
    }

    private void CloseInventory()
    {
        active = false;
        inventoryTab.GetComponent<RectTransform>().anchoredPosition = startPos;

        // resume game
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // enable player movement
        if (playerController != null) playerController.EnableMovement();
    }

    // This function is called when the mouse passes through an item in the inventory
    public void changeDescription(string title, string body, int att1 = 0, string rarity = "", Sprite icon1 = null)
    {
        itemTitle.text = title;
        itemBody.text = body;

        if (att1 > 0)
            atributte1.text = "+" + att1.ToString();
        else if (att1 < 0)
            atributte1.text = "-" + att1.ToString();
        else
            atributte1.text = "";

        if (icon1 != null)
        {
            image_atributte1.enabled = true;
            image_atributte1.sprite = icon1;
        }
        else
            image_atributte1.enabled = false;
    }
}
