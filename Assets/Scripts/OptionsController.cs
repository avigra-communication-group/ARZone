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
        Init();
    }

    public void Init()
    {

        soundVolume.onValueChanged.AddListener(SetVolumeLevel);

        if(!PlayerPrefs.HasKey(GameSettingPrefType.VOLUME))
        {
            PlayerPrefs.SetFloat(GameSettingPrefType.VOLUME, 0.75f);
        }
        else 
        {
            soundVolume.value = PlayerPrefs.GetFloat(GameSettingPrefType.VOLUME);
            AudioListener.volume = soundVolume.value;
        }

        if (!PlayerPrefs.HasKey(GameSettingPrefType.GRAPHIC_QUALITY_LEVEL))
        {
            PlayerPrefs.SetInt(GameSettingPrefType.GRAPHIC_QUALITY_LEVEL, 2);
        }
        else
        {
            switch(PlayerPrefs.GetInt(GameSettingPrefType.GRAPHIC_QUALITY_LEVEL))
            {
                case 0:
                    low.Select();
                break;
                case 2:
                    medium.Select();
                break;
                case 4:
                    high.Select();
                break;
                default:
                    medium.Select();
                break;
            }
        }
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
        PlayerPrefs.SetInt(GameSettingPrefType.GRAPHIC_QUALITY_LEVEL, level);
    }

    public void SetVolumeLevel(float volume)
    {
        float choosenVolumeLevel = soundVolume.value;
        AudioListener.volume = choosenVolumeLevel;
        PlayerPrefs.SetFloat(GameSettingPrefType.VOLUME, volume);
    }
}
