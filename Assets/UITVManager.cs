using UnityEngine;

public class UITVManager : MonoBehaviour
{
    public static UITVManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowMessage(string msg)
    {
        Debug.Log(msg);
        // Optional: hook this into an on-screen UI text popup
    }
}
