using Game.UI;
using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.CameraManagement
{
    /// <summary>
    /// Manages the game's camera. Only one ever exists in a scene. Access the Singleton Instance with <see cref="CameraManager.Instance"/>
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        private UniversalAdditionalCameraData additionalMainCameraData;

        [Header("UI Blur")]
        [SerializeField] private Volume volume;
        [SerializeField][Range(1f, 300f)] private float focalLength = 162f;
        private DepthOfField volumeDepthOfField;
        private bool canEnableUIBackgroundBlur;

        public event Action<IUIBlurUser> OnUIObjectBlurEnabled;
        public event Action<IUIBlurUser> OnUIObjectBlurDisabled;


        public static CameraManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            additionalMainCameraData = HelperFunctions.MainCamera.GetUniversalAdditionalCameraData();
        }
        // Start is called before the first frame update
        void Start()
        {
            foreach (var UIBlurObject in HelperFunctions.GetInterfacesOfType<IUIBlurUser>(false))
            {
                UIBlurObject.OnEnableBlur += () =>
                {
                    SetUIBackgroundBlur(true);
                    OnUIObjectBlurEnabled?.Invoke(UIBlurObject);
                };
                UIBlurObject.OnDisableBlur += () =>
                {
                    SetUIBackgroundBlur(false);
                    OnUIObjectBlurDisabled?.Invoke(UIBlurObject);
                };
            }

            if (volume != null && volume.profile.TryGet<DepthOfField>(out volumeDepthOfField)) SetUIBackgroundBlur(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetIfRenderBackgroundUIBlur(bool doRender)
        {
            UnityEngine.Debug.Log($"Set render background ui blur: {doRender}");
            canEnableUIBackgroundBlur = doRender;
        }

        private void SetUIBackgroundBlur(bool enable)
        {
            if (!canEnableUIBackgroundBlur)
            {
                UnityEngine.Debug.LogWarning("Tried to enable background UI blur, but the current settings options does not allow it!");
                return;
            }

            UnityEngine.Debug.Log($"Set Ui background blur: {enable}");
            if (enable)
            {
                volumeDepthOfField.focalLength.value = focalLength;
                volumeDepthOfField.active = true;
            }
            else volumeDepthOfField.active = false;
        }

    }

}

