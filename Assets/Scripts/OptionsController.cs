using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum GraphicQualityPreset 
{
    low,
    medium,
    high
}

public class OptionsController : MonoBehaviour
{
    [Header("Graphics")]
    public ToggleGroup toggleGroup;
    public Toggle low, medium, high;
    public Slider soundVolume, musicVolume;
    private GraphicQualityPreset graphicSettings;

    private void Start() 
    {
        soundVolume.onValueChanged.AddListener(SetVolumeLevel);
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
        Debug.Log("Quality settings : " +QualitySettings.GetQualityLevel());
    }

    public void SetVolumeLevel(float volume)
    {
        float choosenVolumeLevel = soundVolume.value;
        AudioListener.volume = choosenVolumeLevel;
        Debug.Log(AudioListener.volume);
    }
}
