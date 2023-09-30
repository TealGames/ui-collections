using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class SettingsMenu : BaseUI
    {
        [Header("Settings Menu")]
        [SerializeField] private SettingsSO defaultSettings;
        [SerializeField] private List<TabContainer> tabContainers = new List<TabContainer>();

        [Header("Graphics")]
        [SerializeField] private OptionSelector qualityLevelSelector;
        [SerializeField] private OptionSelector antiAliasingSelector;
        [SerializeField] private OptionSelector anisotropicFilteringSelector;

        [SerializeField] private Toggle vsyncToggle;
        [SerializeField] private Toggle renderParticlesToggle;

        [SerializeField] private Slider maxParticlesPerObjectSlider;

        private bool doRenderParticles;

        public float MasterVolumeDefault { get=> defaultSettings.Audio.MasterVolume; }
        public float MusicVolumeDefault { get=> defaultSettings.Audio.MusicVolume; }
        public float SoundEffectsVolumeDefault { get=> defaultSettings.Audio.SoundEffectsVolume; }
        public float VoiceVolumeDefault { get=> defaultSettings.Audio.VoiceVolume; }


        private UniversalRenderPipelineAsset renderAsset= null;

        private void Awake()
        {
            renderAsset= (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;

            qualityLevelSelector.SetAllOptions(HelperFunctions.GetListFromEnum(typeof(OptionSelectEnums.QualityLevels)));
            antiAliasingSelector.SetAllOptions(HelperFunctions.GetListFromEnum(typeof(OptionSelectEnums.AntiAliasingOptions)));
            anisotropicFilteringSelector.SetAllOptions(HelperFunctions.GetListFromEnum(typeof(AnisotropicFiltering)));

            SetSettingsOptions(defaultSettings);
        }

        // Start is called before the first frame update
        void Start()
        {
            //since the setting option only changes the current scene's particle systems, we have to update them anytime we change scenes
            SceneManager.sceneLoaded += (Scene newScene, LoadSceneMode mode) => SetRenderParticles(doRenderParticles);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EnableTabContainer(TabContainer tabContainer)
        {
            foreach (var container in tabContainers)
            {
                if (container == tabContainer) container.gameObject.SetActive(true);
                else container.gameObject.SetActive(false);
            }
        }

        public void SetVsync(bool isOn)
        {
            if (isOn) QualitySettings.vSyncCount = 1;
            else QualitySettings.vSyncCount = 0;
        }


        //Potential issue- what if a particle is instantiated?
        public void SetRenderParticles(bool renderParticles)
        {
            doRenderParticles= renderParticles;

            ParticleSystem[] particles = GameObject.FindObjectsOfType<ParticleSystem>(true);
            foreach (var particle in particles)
            {
                if (!renderParticles && particle.isPlaying)
                {
                    particle.Stop();
                    UnityEngine.Debug.Log($"Render partcles is set to: {renderParticles} and {particle.gameObject.name} was stopped!");
                }

                else if (renderParticles && !particle.isPlaying)
                {
                    particle.Play();
                    UnityEngine.Debug.Log($"Render partcles is set to: {renderParticles} and {particle.gameObject.name} has begun playing!");
                }
            } 
        }

        public void SetQualityLevel(string levelName)
        {
            levelName= levelName.Replace("_", " ");

            List<string> qualityLevels = QualitySettings.names.ToList();

            int index = -1;
            for (int i = 0; i < qualityLevels.Count(); i++) if (qualityLevels[i].Equals(levelName)) index = i;

            if (index==-1)
            {
                UnityEngine.Debug.LogError($"Tried to set quality level:{levelName}, but it was not found in QualitySettings.names! Make sure it is spelled correctly!");
                return;
            }

            QualitySettings.SetQualityLevel(index, applyExpensiveChanges: false);
        }

        public void SetAntiAliasing(string newLevel)
        {
            if (Enum.TryParse(newLevel, true, out OptionSelectEnums.AntiAliasingOptions newLevelAsEnum)) 
                QualitySettings.antiAliasing = (int)newLevelAsEnum;
        }

        public void SetAnisotropicFiltering(string newFilteringSetting)
        {
            if (Enum.TryParse(newFilteringSetting, true, out AnisotropicFiltering newFilteringAsEnum))
                QualitySettings.anisotropicFiltering = newFilteringAsEnum;
        }


        public void SetSettingsOptions(SettingsSO newSettings)
        {
            SetAudioSettings(newSettings);
            SetInputSettings(newSettings);
            SetGraphicSettings(newSettings);
            SetDialogueSettings(newSettings);
            SetOtherSettings(newSettings);
        }

        private void SetAudioSettings(SettingsSO newSettings)
        {

        }

        private void SetGraphicSettings(SettingsSO newSettings)
        {
            SetVsync(newSettings.Graphics.IsVsyncOn);

            qualityLevelSelector.SetCurrentOption(newSettings.Graphics.QualityLevel.ToString());
            antiAliasingSelector.SetCurrentOption(newSettings.Graphics.AntiAliasingLevel.ToString());
            anisotropicFilteringSelector.SetCurrentOption(newSettings.Graphics.AnisotropicFiltering.ToString());
        }

        private void SetInputSettings(SettingsSO newSettings)
        {

        }

        private void SetDialogueSettings(SettingsSO newSettings)
        {

        }

        private void SetOtherSettings(SettingsSO newSettings)
        {

        }
    }
}

