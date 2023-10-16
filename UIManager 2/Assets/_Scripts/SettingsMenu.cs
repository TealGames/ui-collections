using Game.Input;
using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class SettingsMenu : BaseUI
    {
        [System.Serializable]
        public class References
        {
            [Header("Volume")]
            [SerializeField] public ExtendedSlider masterSlider;
            [SerializeField] public ExtendedSlider musicSlider;
            [SerializeField] public ExtendedSlider sfxSlider;
            [SerializeField] public ExtendedSlider voiceSlider;

            [Header("Graphics")]
            [SerializeField] public OptionSelector qualityLevelSelector;
            [SerializeField] public OptionSelector antiAliasingSelector;
            [SerializeField] public OptionSelector anisotropicFilteringSelector;

            [SerializeField] public ExtendedToggle vsyncToggle;
            [SerializeField] public ExtendedToggle renderParticlesToggle;

            [SerializeField] public ExtendedSlider maxParticlesPerObjectSlider;


            [Header("Input")]
            [SerializeField] public InputSection[] inputSections;
            [SerializeField] public GameObject rebindOverlay;
            
        }

        [Header("Settings Menu")]
        [SerializeField] private References references;
        private bool doRenderParticles;

        [SerializeField] public ExtendedRebindActionUI.RebindDisplay displayType;

        [field: SerializeField] public SettingsSO DefaultSettings { get; private set; }
        //[SerializeField] private List<TabContainer> tabContainers = new List<TabContainer>();

       

        public float MasterVolumeDefault { get=> DefaultSettings.Audio.MasterVolume; }
        public float MusicVolumeDefault { get=> DefaultSettings.Audio.MusicVolume; }
        public float SoundEffectsVolumeDefault { get=> DefaultSettings.Audio.SoundEffectsVolume; }
        public float VoiceVolumeDefault { get=> DefaultSettings.Audio.VoiceVolume; }

        private UniversalRenderPipelineAsset renderAsset= null;

        [System.Serializable]
        public class SettingOption
        {
            [field: SerializeField] public SettingType Type { get; set; }
            [field: SerializeField] public GameObject UIReference { get; set; }
            [field: SerializeField] public UnityEvent<System.Object> OnValueSet { get; set; }
        }
        public enum SettingType
        {
            Slider,
            Toggle,
            OptionSelector,
        }
        [SerializeField] private List<SettingOption> settingsOptions = new List<SettingOption>();

        private void Awake()
        {
            renderAsset= (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;

            if (references.qualityLevelSelector!=null) references.qualityLevelSelector.SetAllOptions(HelperFunctions.GetListFromEnum(typeof(OptionSelectEnums.QualityLevels)));
            if (references.antiAliasingSelector != null) references.antiAliasingSelector.SetAllOptions(HelperFunctions.GetListFromEnum(typeof(OptionSelectEnums.AntiAliasingOptions)));
            if (references.anisotropicFilteringSelector != null) references.anisotropicFilteringSelector.SetAllOptions(HelperFunctions.GetListFromEnum(typeof(AnisotropicFiltering)));

            SetSettingsOptions(DefaultSettings);

            DisableUI();
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

        private void OnValidate()
        {
            if (references.inputSections.Length > 0) foreach (var section in references.inputSections) section.SetBindingDisplayType(displayType);

            //Validate Custom Settings Options
            foreach (var option in settingsOptions)
            {
                if (option.UIReference==null) continue;
                else if((option.Type== SettingType.Slider && !option.UIReference.TryGetComponent<ExtendedSlider>(out _)) ||
                   (option.Type== SettingType.Toggle && !option.UIReference.TryGetComponent<ExtendedToggle>(out _)) ||
                   (option.Type== SettingType.OptionSelector && !option.UIReference.TryGetComponent<OptionSelector>(out _)))
                {
                    UnityEngine.Debug.LogWarning($"Tried to set new setting option of type {option.Type} but the corresponding UI Reference Object does not have the right component! " +
                        $"The UI reference should be the parent GameObject that has: ExtendedSlider.cs if Slider; ExtendedToggle.cs is Toggle; OptionSelector.cs if OptionSelector");
                    option.UIReference = null;
                }
            }
        }

        /*
        public void EnableTabContainer(TabContainer tabContainer)
        {
            foreach (var container in tabContainers)
            {
                if (container == tabContainer) container.gameObject.SetActive(true);
                else container.gameObject.SetActive(false);
            }
        }
        */

        #region Graphics Methods
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
        #endregion

        public void ResetAllRebindActions()
        {
            foreach (var section in references.inputSections) section.ResetSectionBindingsToDefault();
        }

        #region Input Methods
        public void SetRebindOverlayStatus(bool status) => references.rebindOverlay.SetActive(status);
        public void EnableRebindOverlay(RebindActionUI rebindAction, InputActionRebindingExtensions.RebindingOperation operation) => SetRebindOverlayStatus(true);
        public void DisableRebindOverlay(RebindActionUI rebindAction, InputActionRebindingExtensions.RebindingOperation operation) => SetRebindOverlayStatus(false);

        public void SetSettingsOptions(SettingsSO newSettings)
        {
            SetAudioSettings(newSettings);
            SetInputSettings(newSettings);
            SetGraphicSettings(newSettings);
            SetDialogueSettings(newSettings);
            SetOtherSettings(newSettings);
        }
        #endregion

        private void SetAudioSettings(SettingsSO newSettings)
        {
            if (references.masterSlider != null) references.masterSlider.SetSliderValue(newSettings.Audio.MasterVolume);
            if (references.musicSlider != null) references.musicSlider.SetSliderValue(newSettings.Audio.MusicVolume);
            if (references.sfxSlider != null) references.sfxSlider.SetSliderValue(newSettings.Audio.SoundEffectsVolume);
            if (references.voiceSlider != null) references.voiceSlider.SetSliderValue(newSettings.Audio.VoiceVolume);
        }
        public void SetDefaultAudioSettings() => SetAudioSettings(DefaultSettings);


        private void SetGraphicSettings(SettingsSO newSettings)
        {
            if (references.vsyncToggle!=null) references.vsyncToggle.SetToggleValue(newSettings.Graphics.IsVsyncOn);
            if (references.renderParticlesToggle != null) references.renderParticlesToggle.SetToggleValue(newSettings.Graphics.DoRenderParticles);

            if (references.qualityLevelSelector != null) references.qualityLevelSelector.SetCurrentOption(newSettings.Graphics.QualityLevel.ToString());
            if (references.antiAliasingSelector != null) references.antiAliasingSelector.SetCurrentOption(newSettings.Graphics.AntiAliasingLevel.ToString());
            if (references.anisotropicFilteringSelector != null) references.anisotropicFilteringSelector.SetCurrentOption(newSettings.Graphics.AnisotropicFiltering.ToString());
        }
        public void SetDefaultGraphicSettings() => SetGraphicSettings(DefaultSettings);


        private void SetInputSettings(SettingsSO newSettings)
        {

        }
        public void SetDefaultInputSettings() => SetInputSettings(DefaultSettings);


        private void SetDialogueSettings(SettingsSO newSettings)
        {

        }
        public void SetDefaultDialogueSettings() => SetDialogueSettings(DefaultSettings);
        

        private void SetOtherSettings(SettingsSO newSettings)
        {

        }
        public void SetDefaultOtherSettings() => SetOtherSettings(DefaultSettings);
    }
}

