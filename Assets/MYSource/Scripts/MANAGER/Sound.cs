using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] string parameterName = "";

    public void ToggleAudioVolume()
    {//�������� ������ 0�϶� true�� 1 false�� 0;
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }

    public void OnValueChanged()
    {
        audioMixer.SetFloat(parameterName,
        (volumeSlider.value <= volumeSlider.minValue) ? -80f : volumeSlider.value);
    }
}
