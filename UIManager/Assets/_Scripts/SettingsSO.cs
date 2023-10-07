using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SettingsSO", menuName = "ScriptableObjects/Settings")]
    public class SettingsSO : ScriptableObject
    {
        [System.Serializable]
        public class AudioSettings
        {
            [Range(0, 100)][SerializeField] private int masterVolume;
            public int MasterVolume { get => masterVolume; }

            [Range(0, 100)][SerializeField] private int musicVolume;
            public int MusicVolume { get => musicVolume; }

            [Range(0, 100)][SerializeField] private int soundEffectsVolume;
            public int SoundEffectsVolume { get => soundEffectsVolume; }

            [Range(0, 100)][SerializeField] private int voiceVolume;
            public int VoiceVolume { get => voiceVolume; }

            [field: SerializeField] public bool BypassAudioEffects { get; private set; }
        }

        [System.Serializable]
        public class InputSettings
        {

        }

        [System.Serializable]
        public class GraphicSettings
        {
            [field: SerializeField] public bool IsVsyncOn { get; private set; }
            [field: SerializeField] public OptionSelectEnums.QualityLevels QualityLevel { get; private set; }
            [field: SerializeField] public OptionSelectEnums.AntiAliasingOptions AntiAliasingLevel { get; private set; }
            [field: SerializeField] public AnisotropicFiltering AnisotropicFiltering { get; private set; }

            [field: SerializeField] public bool DoRenderParticles { get; private set; }

            [Range(100, 2000)][SerializeField] private int maxParticlesPerObject;
            public int MaxParticlesPerObject { get => maxParticlesPerObject; }
        }

        [System.Serializable]
        public class DialogueSettings
        {

        }

        [System.Serializable]
        public class OtherSettings
        {

        }


        [field: SerializeField] public AudioSettings Audio { get; private set; }
        [field: SerializeField] public InputSettings Input { get; private set; }
        [field: SerializeField] public GraphicSettings Graphics { get; private set; }
        [field: SerializeField] public DialogueSettings Dialogue { get; private set; }
        [field: SerializeField] public OtherSettings Other { get; private set; }


        //[Header("Input")]
        //[Header("Dialogue")]
        //[Header("Other")]
    }

}

