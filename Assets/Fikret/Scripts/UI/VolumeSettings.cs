using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Mixer'e eriþmek için þart

public class VolumeSettings : MonoBehaviour
{
    [Header("Ayarlar")]
    public AudioMixer myMixer; // Oluþturduðumuz MainMixer'ý buraya sürükle

    [Header("UI Elemanlarý")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Oyun açýlýnca kayýtlý ses ayarlarýný yükle (yoksa varsayýlan 0.75 olsun)
        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadVolume();
        else
        {
            SetMusicVolume(0.75f);
            SetSFXVolume(0.75f);
        }
    }

    // Slider'ý hareket ettirince bu çalýþacak
    public void SetMusicVolume(float sliderValue)
    {
        // Logaritmik dönüþüm: 0.0001 ile 1 arasý deðeri -80dB ile 0dB arasýna çevirir
        myMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        // Ayarý kaydet
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        myMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }
}