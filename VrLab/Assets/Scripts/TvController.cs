using UnityEngine;
using UnityEngine.Video;

public class TvController : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public GameObject indicatorLight;
    private bool isTvOn = false;

    public void ToggleTV()
    {
        isTvOn = !isTvOn;

        if (isTvOn)
        {
            if (videoPlayer != null) videoPlayer.Play();
            if (indicatorLight != null) indicatorLight.SetActive(true);
            Debug.Log("TV AAN");
        }
        else
        {
            if (videoPlayer != null) videoPlayer.Pause();
            if (indicatorLight != null) indicatorLight.SetActive(false);
            Debug.Log("TV UIT");
        }
    }
}
