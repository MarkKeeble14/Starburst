using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cinemachine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;
    private CinemachinePOV inGamePOV;
    [SerializeField] private float minMouseSensitivity = 50f;
    [SerializeField] private float maxMouseSensitivity = 300f;
    [SerializeField] private float yAxisMouseSensitivityModifier = 1f;

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private StoreSetting mouseSensitivity;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private StoreSetting musicVolume;

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private StoreSetting sfxVolume;

    [SerializeField] private AudioMixer mixer;

    public void SetMouseSensitivity(float percent)
    {
        mouseSensitivity.Value = percent;
        float dif = (maxMouseSensitivity - minMouseSensitivity) * percent;
        inGamePOV.m_HorizontalAxis.m_MaxSpeed = minMouseSensitivity + dif;
        inGamePOV.m_VerticalAxis.m_MaxSpeed = minMouseSensitivity + (dif * yAxisMouseSensitivityModifier);
    }

    public void SetMusicVolume(float percent)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(percent) * 20);
        musicVolume.Value = percent;
    }

    public void SetSFXVolume(float percent)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(percent) * 20);
        sfxVolume.Value = percent;
    }

    private void Start()
    {
        inGamePOV = vCam.GetCinemachineComponent<CinemachinePOV>();

        sensitivitySlider.value = mouseSensitivity.Value;
        SetMouseSensitivity(mouseSensitivity.Value);

        SetMusicVolume(musicVolume.Value);
        musicVolumeSlider.value = musicVolume.Value;

        SetSFXVolume(sfxVolume.Value);
        sfxVolumeSlider.value = sfxVolume.Value;
    }
}
