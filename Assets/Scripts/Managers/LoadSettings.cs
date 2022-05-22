using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSettings : MonoBehaviour
{
    public AudioSource musicMenu;
    public Resolution[] resolutions;
    public bool fullscreen;
    public int quality;
    public int resolutionIndex;
    public float volume;

    void OnEnable()
    {
        volume = PlayerPrefs.GetFloat("volume");
        resolutionIndex =  PlayerPrefs.GetInt("resolution");
        quality = PlayerPrefs.GetInt("quality");
        fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;        
        
        resolutions = Screen.resolutions;


        Screen.fullScreen = fullscreen;
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
        QualitySettings.masterTextureLimit = quality;
        musicMenu.volume = volume;
    }
}
