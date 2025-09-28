using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New  Key Item", menuName = "Inventory/Tetris/KeyItem")]
public class Key : TetrisItem
{
    [Header("Key Settings")]
    public string keyID; // unique identifier for this key
}