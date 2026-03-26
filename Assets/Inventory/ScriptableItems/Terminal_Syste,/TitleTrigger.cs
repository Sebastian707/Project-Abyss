using UnityEngine;

public class TitleTrigger : MonoBehaviour
{
    [TextArea(3, 10)] // min 3 lines, max 10 lines in the inspector
    public string titleName;

    public AudioClip triggerSound;
    public bool triggerOnce = true;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && triggerOnce) return;

        if (other.CompareTag("Player"))
        {
            TitleDropManager.Instance.ShowTitle(titleName, triggerSound);
            triggered = true;
        }
    }
}