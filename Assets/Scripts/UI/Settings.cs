using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider sliderMaster, sliderMusic, sliderEffects;

    List<Audio> audioSettings = new();

    [Serializable]
    struct Audio
    {
        public string playerPrefsName, mixerGroupName;
        [SerializeField]
        public Slider slider;
        float multi;

        public Audio(string playerPrefs, string mixerGroup, Slider settingSlider)
        {
            playerPrefsName = playerPrefs;
            mixerGroupName = mixerGroup;
            slider = settingSlider;
            multi = 20f;
        }

        public float CalcValue()
        {
            return Mathf.Log10(slider.value) * multi;
        }
    }

    void Start()
    {
        audioSettings.Add(new Audio("audioMaster", "Master", sliderMaster));
        audioSettings.Add(new Audio("audioMusic", "Music", sliderMusic));
        audioSettings.Add(new Audio("audioEffects", "Effects", sliderEffects));

        foreach (Audio a in audioSettings)
            if (!PlayerPrefs.HasKey(a.playerPrefsName))
                PlayerPrefs.SetFloat(a.playerPrefsName, a.slider.value);

        SettingsCancel();
    }

    public void SettingsApplyOnChange()
    {
        foreach (Audio a in audioSettings)
            mixer.SetFloat(a.mixerGroupName, a.CalcValue());
    }

    public void SettingsCancel()
    {
        foreach (Audio a in audioSettings)
            a.slider.value = PlayerPrefs.GetFloat(a.playerPrefsName);

        SettingsApplyOnChange();
        gameObject.SetActive(false);
    }

    public void SettingsSave()
    {
        foreach (Audio a in audioSettings)
            PlayerPrefs.SetFloat(a.playerPrefsName, a.slider.value);

        gameObject.SetActive(false);
    }
}
