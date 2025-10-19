using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Paper", menuName = "Inventory/Tetris/Paper")]
public class PaperItem : TetrisItem
{
    [TextArea(5, 10)]
    public string paperText;

    public Sprite backgroundImage; 
    public TMP_FontAsset font;
    public Color color;

    public override void Use()
    {
        Debug.Log("Reading paper: " + itemName);

        PaperUIManager.Instance.ShowPaper(this);
    }
}
