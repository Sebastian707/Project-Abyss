using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Paper", menuName = "Inventory/Tetris/Paper")]
public class PaperItem : TetrisItem
{
    [TextArea(5, 10)]
    public string paperText; // Text content of the paper

    public Sprite backgroundImage; // The background image for this paper
    public TMP_FontAsset font;     // Font to use for the text

    public override void Use()
    {
        Debug.Log("Reading paper: " + itemName);

        // Call UI Manager to display the paper
        PaperUIManager.Instance.ShowPaper(this);
    }
}
