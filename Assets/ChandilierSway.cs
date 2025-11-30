using UnityEngine;

public class ChandelierSway : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingAngleX = 2f;
    public float swingAngleZ = 15f;       
    public float swingSpeed = 1f;        

    [Header("Sound Settings")]
    public AudioSource squeakAudio;    
    public float squeakThreshold = 0.98f;

    private float swingTimer = 0f;
    private bool squeakPlayed = false;

    void Update()
    {
  
        swingTimer += Time.deltaTime * swingSpeed;


        float angleX = Mathf.Sin(swingTimer) * swingAngleX;
        float angleZ = Mathf.Sin(swingTimer) * swingAngleZ;
        transform.localRotation = Quaternion.Euler(angleX, 0f, angleZ);

        if (Mathf.Abs(Mathf.Sin(swingTimer)) > squeakThreshold)
        {
            if (!squeakPlayed && squeakAudio != null)
            {
                squeakAudio.Play();
                squeakPlayed = true;
            }
        }
        else
        {
            squeakPlayed = false;
        }
    }
}
