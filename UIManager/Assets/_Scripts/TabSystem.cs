using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private List<TabInfo> tabs = new List<TabInfo>();




        private int currentTabs = 0;

        [System.Serializable]
        public class TabInfo
        {
            [field: SerializeField] public string Name { get; private set; }
            public TabContainer Container { get; set; }
            public TabButton Button { get; set; }
        }

        public enum LayoutType
        {
            Horizontal,
            Vertical,
        }


        [Header("Customization")]
        [SerializeField] public LayoutType buttonLayoutType;
        [SerializeField] private TextAnchor buttonLocation;

        [SerializeField] private string buttonNameSuffix = "Button";
        [Tooltip("The distance between the tab buttons. Higher positive values means they are further apart. Higher negative values means they are closer together")]
        [SerializeField] private float buttonDistance;
        [Tooltip("The distance of the buttons from the corresponding edge")][SerializeField] private RectOffset buttonMargins;

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

        public string ContainerNameSuffix { get =>  containerNameSuffix; }
#endif

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnValidate()
        {
            //if (tabs.Count > 0 && currentTabs != tabs.Count) CreateNewTabs();
        }

        public void EnableTabContainer(TabContainer tabContainer)
        {
            foreach (var container in tabs)
            {
                if (container.Container == tabContainer) container.Container.gameObject.SetActive(true);
                else container.Container.gameObject.SetActive(false);
            }
        }

        public void EnableTabContainer(int index) => EnableTabContainer(tabs[index].Container);

        private void CreateNewTabs()
        {
            if (tabs.Count == 0 || currentTabs == tabs.Count)
            {
                UnityEngine.Debug.Log($"Tried to create new tabs in {gameObject.name} but there either 0 tabs, or no tabs were changed!");
                return;
            }

            for (int i = currentTabs; i < tabs.Count; i++)
            {
                //Create a new tab
                GameObject newTab = Instantiate(tabPrefab, Vector2.zero, Quaternion.identity, tabParent.transform);
                newTab.name = tabs[i].Name;

                tabs[i].Container = newTab.GetComponent<TabContainer>();
                tabs[i].Container.SetTitle(tabs[i].Name);

                //Create a corresponding button
                GameObject newButton = Instantiate(tabButtonPrefab, Vector2.zero, Quaternion.identity, tabButtonParent.transform);
                newButton.name = tabs[i].Name + " " + buttonNameSuffix;

                tabs[i].Button = newButton.GetComponent<TabButton>();
                tabs[i].Button.SetTitle(tabs[i].Name);
                tabs[i].Button.AddClickAction(() => EnableTabContainer(i));
            }
            currentTabs = tabs.Count;
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
    }
}

