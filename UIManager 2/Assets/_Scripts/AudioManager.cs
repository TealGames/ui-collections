using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioMixer audioMixer;

    public AudioManager Instance { get; private set; }

    private const string masterVolumeParam= "MIXER_MASTER_VOLUME";
    private const string musicVolumeParam= "MIXER_MUSIC_VOLUME";
    private const string sfxVolumeParam= "MIXER_VOICE_VOLUME";
    private const string voiceVolumeParam= "MIXER_SOUNDEFFECTS_VOLUME";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Converts a volume (int) from 0-100 to a dB (float) from -80 to 20
    /// </summary>
    /// <param name="startVolume"></param>
    /// <returns></returns>
    private float ConvertVolumeToDecibels(int startVolume)
    {
        //we set the volume from 0-1 and then switch it to dB which Mixer uses (-80dB to 20dB)
        float newVolume = Mathf.Clamp(startVolume, 0, 100);
        newVolume /= 100;
        newVolume = Mathf.Log10(newVolume) * 20;
        return newVolume;
    }

    /// <summary>
    /// Converts a dB (float) from -80 to 20 to a volume (int) from 0-100
    /// </summary>
    /// <param name="decibelVolume"></param>
    /// <returns></returns>
    private int ConvertDecibelsToVolume(float decibelVolume)
    {
        int newVolume = (int)Mathf.Clamp(decibelVolume, -80f, 20f);
        newVolume = (int)Mathf.Pow(10, newVolume / 20);
        newVolume *= 100;
        return newVolume;
    }

    public void SetMasterVolume(float volume) => audioMixer.SetFloat(masterVolumeParam, ConvertVolumeToDecibels((int)volume));
    public void SetMusicVolume(float volume) => audioMixer.SetFloat(musicVolumeParam, ConvertVolumeToDecibels((int)volume));
    public void SetSFXVolume(float volume) => audioMixer.SetFloat(sfxVolumeParam, ConvertVolumeToDecibels((int)volume));
    public void SetVoiceVolume(float volume) => audioMixer.SetFloat(voiceVolumeParam, ConvertVolumeToDecibels((int)volume));


}
