using UnityEngine;

public class BasicTransition : MonoBehaviour
{

    [SerializeField] private string targetScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        SceneTransitionManager.Instance.TransitionToScene(targetScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
