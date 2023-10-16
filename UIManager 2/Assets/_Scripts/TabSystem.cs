using Game.Utilities;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;
using static Game.UI.TabSystem;

namespace Game.UI
{
    public class TabSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject tabParent;
        [SerializeField] private GameObject tabPrefab;

        [SerializeField] private GameObject tabButtonParent;
        [SerializeField] private GameObject tabButtonPrefab;
        private HorizontalLayoutGroup horizontalLayoutGroup = null;
        private VerticalLayoutGroup verticalLayoutGroup = null;

        [Header("Tabs")]
        [Tooltip("This will be the first tab that will be displayed for this tabSystem. THIS MUST BE PART OF THIS TAB SYSTEM ONLY (if there are multiple systems)")]
        [SerializeField] private TabContainer defaultTab;
        [SerializeField] private List<TabInfo> tabs = new List<TabInfo>();

        private int currentTabs = 0;

        [System.Serializable]
        public class TabInfo
        {
            [field: SerializeField] public string Name { get; private set; } = "";
            public TabContainer Container { get; set; } = null;
            public TabButton Button { get; set; } = null;
        }

        public enum LayoutType
        {
            Horizontal,
            Vertical,
        }


        [Header("Customization")]
        [SerializeField] public LayoutType buttonLayoutType = LayoutType.Vertical;
        [SerializeField] private TextAnchor buttonLocation = TextAnchor.MiddleLeft;

        [SerializeField] private string buttonNameSuffix = "Button";
        [Tooltip("The distance between the tab buttons. Higher positive values means they are further apart. Higher negative values means they are closer together")]
        [SerializeField] private float buttonDistance;
        [Tooltip("The distance of the buttons from the corresponding edge")][SerializeField] private RectOffset buttonMargins;
        [SerializeField] private bool reverseButtonOrder;

        [SerializeField] private string containerNameSuffix = "Container";


#if UNITY_EDITOR
        public List<TabInfo> Tabs
        {
            get => tabs;
            set => tabs = value;
        }

        public GameObject TabParent { get => tabParent; }
        public GameObject TabPrefab { get => tabPrefab; }

        public GameObject TabButtonParent { get => tabButtonParent; }
        public GameObject TabButtonPrefab { get => tabButtonPrefab; }

        public LayoutType ButtonLayoutType { get => buttonLayoutType; }
        public TextAnchor ButtonLocation { get => buttonLocation; }
        public string ButtonNameSuffix { get => buttonNameSuffix; }
        public float ButtonDistance { get => buttonDistance; }
        public RectOffset ButtonMargins { get => buttonMargins; }
        public bool ReverseButtonOrder { get => reverseButtonOrder; }

        public string ContainerNameSuffix { get =>  containerNameSuffix; }
        public readonly string TabSystemID=HelperFunctions.GenerateRandomID();
#endif

        // Start is called before the first frame update
        void Start()
        {
            //We set the tabs (not including the button parent) at Start() because the references are lost when reloading in the inspector (so we can't do it then)
            for (int i=0; i< tabParent.transform.childCount-1; i++)
            {
                //We account for the tab buttons child, which is not a tabContainer by allowing one more than the total tabs
                if (i > tabs.Count)
                {
                    UnityEngine.Debug.LogError($"Tried to set tabs of {tabParent.name} but there are other children that are not part of the TabSystem! " +
                        $"Remove them to ensure that the TabSystem behaves properly. Note: DO NOT REMOVE THE BUTTON PARENT GAMEOBJECT, ONLY OTHER GAMEOBJECTS");
                    return;
                }
                if (tabParent.transform.GetChild(i).TryGetComponent<TabContainer>(out TabContainer tabContainer)) tabs[i].Container = tabContainer;
            }

            for (int i=0; i< tabButtonParent.transform.childCount; i++)
            {
                if (i >= tabs.Count)
                {
                    UnityEngine.Debug.LogError($"Tried to set tab buttons of {tabButtonParent.name} but there are other children that are not part of the TabSystem! " +
                        $"Remove them to ensure that the TabSystem behaves properly.");
                    return;
                }
                if (tabButtonParent.transform.GetChild(i).TryGetComponent<TabButton>(out TabButton tabButton)) tabs[i].Button = tabButton;
            }

            //Add the click action for each button
            for (int j=0; j < tabs.Count; j++)
            {
                int currentIndex = j;
                UnityEngine.Debug.Log($"Tab button {tabs[currentIndex].Button.GetTitle()} has action set to enable container {currentIndex}");
                tabs[currentIndex].Button.AddClickAction(() => EnableTabContainer(currentIndex));
            }

            UpdateRectTransforms();

            //Set only the default tab to be enabled on start
            bool isDefaultTabFound = false;
            foreach (var tab in tabs)
            {
                if (tab.Container== defaultTab)
                {
                    isDefaultTabFound = true;
                    break;
                }
            }

            if (!isDefaultTabFound)
            {
                UnityEngine.Debug.LogWarning($"Tried to set start tab {defaultTab.GetTitle()} in {gameObject.name}, but it does not exist in this TabSystem! " +
                    $"Make sure it default tabs are only tabs found in that system.");
                return;
            }

            foreach (var tab in tabs)
            {
                if (tab.Container == defaultTab) tab.Container.gameObject.SetActive(true);
                else tab.Container.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var tab in tabs)
            {
                UnityEngine.Debug.Log($"Tab {tab.Name} has container: {(tab.Container!=null? tab.Container.gameObject.name : "NULL")}");
            }
            UpdateRectTransforms();  
        }

        private void OnValidate()
        {
            //UpdateTabNames();
            //UpdateRectTransforms();
        }

        public void EnableTabContainer(TabContainer tabContainer)
        {
            foreach (var container in tabs)
            {
                if (container.Container == tabContainer) container.Container.gameObject.SetActive(true);
                else container.Container.gameObject.SetActive(false);
            }
        }

        public void EnableTabContainer(int index)
        {
            UnityEngine.Debug.Log($"Called to enable tab index: {index}");
            EnableTabContainer(tabs[index].Container);
        }

        private void SetButtonLayout()
        {
            if (tabButtonParent == null)
            {
                UnityEngine.Debug.Log($"Tried to set {gameObject.name} button layout, but the the button parent is NULL!");
                return;
            }

            tabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out horizontalLayoutGroup);
            tabButtonParent.TryGetComponent<VerticalLayoutGroup>(out verticalLayoutGroup);

            if (horizontalLayoutGroup != null)
            {
                horizontalLayoutGroup.childAlignment = buttonLocation;
                horizontalLayoutGroup.spacing = buttonDistance;
            }
            else if (verticalLayoutGroup != null)
            {
                verticalLayoutGroup.childAlignment = buttonLocation;
                verticalLayoutGroup.spacing = buttonDistance;
            }


            if (buttonLayoutType == LayoutType.Horizontal && !tabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out _))
            {
                if (tabButtonParent.TryGetComponent<VerticalLayoutGroup>(out VerticalLayoutGroup vGroup)) DestroyImmediate(vGroup);
                horizontalLayoutGroup = tabButtonParent.AddComponent<HorizontalLayoutGroup>();
                verticalLayoutGroup = null;
            }
            else if (buttonLayoutType == LayoutType.Vertical && !tabButtonParent.TryGetComponent<VerticalLayoutGroup>(out _))
            {
                if (tabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out HorizontalLayoutGroup hGroup)) DestroyImmediate(hGroup);
                VerticalLayoutGroup group = tabButtonParent.AddComponent<VerticalLayoutGroup>();
                horizontalLayoutGroup = null;
            }
        }

        public void UpdateRectTransforms()
        {
            UnityEngine.Debug.Log($"OnValidate called!");
            RectTransform transform = tabParent.GetComponent<RectTransform>();
            transform.SetAnchorPreset(AnchorPresets.StretchStretch);
            transform.localPosition = Vector3.zero;
            transform.sizeDelta = Vector3.zero;

            Transform parentTransform = tabParent.transform;
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                //UnityEngine.Debug.Log($"Testing {parentTransform.GetChild(i).gameObject.name}");
                if (parentTransform.GetChild(i).TryGetComponent<TabContainer>(out TabContainer container))
                {
                    RectTransform tabTransform = container.gameObject.GetComponent<RectTransform>();
                    tabTransform.SetAnchorPreset(AnchorPresets.StretchStretch);
                    tabTransform.localPosition = Vector3.zero;
                    tabTransform.sizeDelta = Vector3.zero;
                }
            }
        }

        public void UpdateTabNames()
        {
            foreach (var tab in Tabs)
            {
                if (tab.Container != null && tab.Container.gameObject != null)
                {
                    UnityEngine.Debug.Log($"Tab {tab.Container.gameObject.name} has title {tab.Container.GetTitle()}");
                    if (!tab.Container.gameObject.name.Equals(tab.Name + " " + containerNameSuffix))
                        tab.Container.gameObject.name = tab.Name + " " + containerNameSuffix;
                    if (!tab.Container.GetTitle().Equals(tab.Name)) tab.Container.SetTitle(tab.Name);

                    if (tab.Button != null && tab.Button.gameObject != null)
                    {
                        if (!tab.Button.gameObject.name.Equals(tab.Name + " " + buttonNameSuffix))
                            tab.Button.gameObject.name = tab.Name + " " + buttonNameSuffix;
                        if (!tab.Button.GetTitle().Equals(tab.Name)) tab.Button.SetTitle(tab.Name);
                    }
                }
            }
        }
    }
}

