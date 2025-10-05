using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New VHS Item", menuName = "Inventory/Tetris/VHSItem")]
public class VHS : TetrisItem
{
    public string tapeID;
    public VideoClip videoClip;
}