using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game
{
    /// <summary>
    /// Contains data about setting options. Can be used to store the default value, setting presets, or saving player saved setting preferences.
    /// </summary>
    [CreateAssetMenu(fileName = "SettingsSO", menuName = "ScriptableObjects/Settings")]
    public class SettingsSO : ScriptableObject
    {
        [System.Serializable]
        public class AudioSettings
        {
            [Range(0, 100)][SerializeField] private int masterVolume;
            public int MasterVolume { get => masterVolume; set => masterVolume = value; }

            [Range(0, 100)][SerializeField] private int musicVolume;
            public int MusicVolume { get => musicVolume; set => musicVolume = value; }

            [Range(0, 100)][SerializeField] private int soundEffectsVolume;
            public int SoundEffectsVolume { get => soundEffectsVolume; set => soundEffectsVolume = value; }

            [Range(0, 100)][SerializeField] private int voiceVolume;
            public int VoiceVolume { get => voiceVolume; set => voiceVolume = value; }

            [field: SerializeField] public bool BypassAudioEffects { get; set; }
        }

        [System.Serializable]
        public class InputSettings
        {
            public List<InputSaveData> InputSaveData { get; set;} = new List<InputSaveData>();
        }

        [System.Serializable]
        public class GraphicSettings
        {
            [field: SerializeField] public bool IsVsyncOn { get; set; }
            [field: SerializeField] public OptionSelectEnums.QualityLevels QualityLevel { get; set; }
            [field: SerializeField] public OptionSelectEnums.AntiAliasingOptions AntiAliasingLevel { get; set; }
            [field: SerializeField] public AnisotropicFiltering AnisotropicFiltering { get; set; }

            [field: SerializeField] public bool DoRenderParticles { get; set; }

            [Range(100, 2000)][SerializeField] private int maxParticlesPerObject;
            public int MaxParticlesPerObject { get => maxParticlesPerObject; set => maxParticlesPerObject = value; }

            [field: SerializeField] public bool DoBackgroundUIBlur { get; set; }
        }

        [System.Serializable]
        public class DialogueSettings
        {

        }

        [System.Serializable]
        public class OtherSettings
        {

        }


        [field: SerializeField] public AudioSettings Audio { get; set; }
        [field: SerializeField] public InputSettings Input { get; set; }
        [field: SerializeField] public GraphicSettings Graphics { get; set; }
        [field: SerializeField] public DialogueSettings Dialogue { get; set; }
        [field: SerializeField] public OtherSettings Other { get; set; }


        //[Header("Input")]
        //[Header("Dialogue")]
        //[Header("Other")]
    }

}

