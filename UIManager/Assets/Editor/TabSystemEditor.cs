using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [CustomEditor(typeof(TabSystem))]
    public class TabSystemEditor : ExtendedEditor
    {
        private static int currentTabs = 0;
        private static TabSystem tabSystem = null;
        private static HorizontalLayoutGroup horizontalLayoutGroup = null;
        private static VerticalLayoutGroup verticalLayoutGroup = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            tabSystem = (TabSystem)target;

            if (tabSystem.Tabs.Count > 0 && currentTabs != tabSystem.Tabs.Count) CreateNewTabs();
            if (tabSystem.TabButtonParent != null) SetButtonLayout();

            //Update the name for each tab if they have changed
            foreach (var tab in tabSystem.Tabs)
            {
                if (tab.Container!=null && tab.Container.gameObject!=null && tab.Container.gameObject.name.Equals(tab.Name + " " + tabSystem.ContainerNameSuffix))
                    tab.Container.gameObject.name = tab.Name + " " + tabSystem.ContainerNameSuffix;
            }


            //tabSystem.SetButtonLayout();
        }

        private void CreateNewTabs()
        {
            if (tabSystem.Tabs.Count == 0 || currentTabs == tabSystem.Tabs.Count)
            {
                UnityEngine.Debug.Log($"Tried to create new tabs in {tabSystem.gameObject.name} but there either 0 tabs, or no tabs were changed!");
                return;
            }

            //if we have more tabs, we add those tabs
            if (tabSystem.Tabs.Count> currentTabs)
            {
                UnityEngine.Debug.Log($"Create new tabs called with: {tabSystem.Tabs.Count} tabs and previous tabs: {currentTabs}");
                for (int i = currentTabs; i < tabSystem.Tabs.Count; i++)
                {
                    //only create a new tab only if we have a name for it
                    if (string.IsNullOrEmpty(tabSystem.Tabs[i].Name)) continue;

                    //Create a new tab
                    GameObject newTab = EditorHelperFunctions.InstantiateAsPrefab(tabSystem.TabPrefab, parent: tabSystem.TabParent.transform);
                    newTab.name = tabSystem.Tabs[i].Name + " " + tabSystem.ContainerNameSuffix;

                    tabSystem.Tabs[i].Container = newTab.GetComponent<TabContainer>();
                    tabSystem.Tabs[i].Container.SetTitle(tabSystem.Tabs[i].Name);
                    newTab.transform.SetSiblingIndex(i);

                    //Create a corresponding button
                    GameObject newButton = EditorHelperFunctions.InstantiateAsPrefab(tabSystem.TabButtonPrefab, parent: tabSystem.TabButtonParent.transform);
                    newButton.name = tabSystem.Tabs[i].Name + " " + tabSystem.ButtonNameSuffix;

                    tabSystem.Tabs[i].Button = newButton.GetComponent<TabButton>();
                    tabSystem.Tabs[i].Button.SetTitle(tabSystem.Tabs[i].Name);
                    tabSystem.Tabs[i].Button.AddClickAction(() => tabSystem.EnableTabContainer(i));
                }
                

                UnityEngine.Debug.Log($"Create new tabs ended with new current tabs: {currentTabs}");
            }

            //if we have less tabs, we destroy the old ones
            else if (tabSystem.Tabs.Count < currentTabs)
            {
                for (int i = currentTabs - 1; i > tabSystem.Tabs.Count; i--)
                {
                    DestroyImmediate(tabSystem.Tabs[i].Container.gameObject);
                    DestroyImmediate(tabSystem.Tabs[i].Button.gameObject);
                }
                    
            }
            currentTabs = tabSystem.Tabs.Count;
        }

        public void SetButtonLayout()
        {
            if (tabSystem.TabButtonParent == null)
            {
                UnityEngine.Debug.Log($"Tried to set {tabSystem.gameObject.name} button layout, but the the button parent is NULL!");
                return;
            }

            tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out horizontalLayoutGroup);
            tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out verticalLayoutGroup);

            if (horizontalLayoutGroup != null)
            {
                horizontalLayoutGroup.childAlignment = tabSystem.ButtonLocation;
                horizontalLayoutGroup.spacing = tabSystem.ButtonDistance;
            }
            else if (verticalLayoutGroup != null)
            {
                verticalLayoutGroup.childAlignment = tabSystem.ButtonLocation;
                verticalLayoutGroup.spacing = tabSystem.ButtonDistance;
            }


            if (tabSystem.ButtonLayoutType == TabSystem.LayoutType.Horizontal && !tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out _))
            {
                if (tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out VerticalLayoutGroup vGroup)) DestroyImmediate(vGroup);
                horizontalLayoutGroup = tabSystem.TabButtonParent.AddComponent<HorizontalLayoutGroup>();
                verticalLayoutGroup = null;
            }
            else if (tabSystem.ButtonLayoutType == TabSystem.LayoutType.Vertical && !tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out _))
            {
                if (tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out HorizontalLayoutGroup hGroup)) DestroyImmediate(hGroup);
                VerticalLayoutGroup group = tabSystem.TabButtonParent.AddComponent<VerticalLayoutGroup>();
                horizontalLayoutGroup = null;
            }
        }
    }
}

