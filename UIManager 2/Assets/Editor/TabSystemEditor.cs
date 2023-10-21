using Game.UI;
using Game.Utilities;
using Game.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.UI
{
    [CustomEditor(typeof(TabSystem))]
    internal class TabSystemEditor : ExtendedEditor
    {
        private static TabSystem tabSystem = null;
        private static HorizontalLayoutGroup horizontalLayoutGroup = null;
        private static VerticalLayoutGroup verticalLayoutGroup = null;

        public override void OnInspectorGUI()
        {
            try
            {
                DrawDefaultInspector();
                tabSystem = (TabSystem)target;

                bool pressed = GUILayout.Button(new GUIContent() { text = "Update Tabs" });
                if (pressed)
                {
                    //tabSystem.UpdateTabNames();

                    //Check to make sure that tabs are in the right order and have the right names
                    TabNameUpdateCheck();

                    //Add tabs that do not have a container for the tab info
                    AddedTabCheck();

                    //Delete any children in tabs that do not have a tab info corresponding to it anymore
                    RemovedTabCheck();

                    //Sets the button layout component if it needs a new one
                    if (tabSystem.TabButtonParent != null) SetButtonLayout();

                    //If a button layout component data has changed, we update it
                    UpdateButtonLayoutData();

                    tabSystem.UpdateRectTransforms();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Tried to update the inspector for TabSystem, but something went wrong! Error: {e}");
                return;
            }
        }

        public void OnDestroy()
        {
            //if (PlayerPrefs.HasKey(tabSystem.TabSystemID)) PlayerPrefs.DeleteKey(tabSystem.TabSystemID);
        }

        private void CreateNewTab(TabSystem.TabInfo tabInfo, int tabIndex)
        {
            UnityEngine.Debug.Log($"Began creating new tab with tab amount for {tabInfo.Name} at index: {tabIndex}");
            /*
            if (tabSystem.Tabs.Count == 0 || GetStoredTabAmount() == tabSystem.Tabs.Count)
            {
                UnityEngine.Debug.Log($"Tried to create new tabs in {tabSystem.gameObject.name} but there either 0 tabs, or no tabs were changed!");
                return;
            }
            */
            if (tabInfo == null || tabIndex == -1 || tabInfo.Container != null)
            {
                UnityEngine.Debug.LogWarning($"Tried to create a new tab for {(tabInfo!=null? tabInfo.Name : null)} but it either does not exist or it already has a tab assigned! (index@{tabIndex})");
                return;
            }


            //Create a new tab
            GameObject newTab = EditorHelperFunctions.InstantiateAsPrefab(tabSystem.TabPrefab, parent: tabSystem.TabParent.transform);
            newTab.name = tabInfo.Name + " " + tabSystem.ContainerNameSuffix;

            tabInfo.Container = newTab.GetComponent<TabContainer>();
            tabInfo.Container.SetTitle(tabInfo.Name);
            newTab.transform.SetSiblingIndex(tabIndex);

            RectTransform transform = newTab.GetComponent<RectTransform>();
            transform.SetAnchorPreset(AnchorPresets.StretchStretch);
            transform.localPosition = Vector3.zero;

            //Create a corresponding button
            GameObject newButton = EditorHelperFunctions.InstantiateAsPrefab(tabSystem.TabButtonPrefab, parent: tabSystem.TabButtonParent.transform);
            newButton.name = tabInfo.Name + " " + tabSystem.ButtonNameSuffix;

            tabInfo.Button = newButton.GetComponent<TabButton>();
            tabInfo.Button.SetTitle(tabInfo.Name);
            //tabInfo.Button.AddClickAction(() => tabSystem.EnableTabContainer(tabIndex));
            tabInfo.Button.transform.SetSiblingIndex(tabIndex);

            UnityEngine.Debug.Log($"Create new tabs ended with new current tabs: {tabSystem.Tabs.Count} (set for {tabSystem.TabSystemID})");
        }

        private void SetButtonLayout()
        {
            if (tabSystem.TabButtonParent == null)
            {
                UnityEngine.Debug.Log($"Tried to set {tabSystem.gameObject.name} button layout, but the the button parent is NULL!");
                return;
            }

            tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out horizontalLayoutGroup);
            tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out verticalLayoutGroup);

            //Add new layout group if it has been changed
            if (tabSystem.ButtonLayoutType == TabSystem.LayoutType.Horizontal && !tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out _))
            {
                if (tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out VerticalLayoutGroup vGroup)) DestroyImmediate(vGroup);
                horizontalLayoutGroup = tabSystem.TabButtonParent.AddComponent<HorizontalLayoutGroup>();
                horizontalLayoutGroup.childForceExpandHeight = false;
                horizontalLayoutGroup.childForceExpandWidth= false;
                verticalLayoutGroup = null;
            }
            else if (tabSystem.ButtonLayoutType == TabSystem.LayoutType.Vertical && !tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out _))
            {
                if (tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out HorizontalLayoutGroup hGroup)) DestroyImmediate(hGroup);
                verticalLayoutGroup = tabSystem.TabButtonParent.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.childForceExpandHeight = false;
                verticalLayoutGroup.childForceExpandWidth = false;
                horizontalLayoutGroup = null;
            }

            //Based on layout type, update the anchors so it maintains the same aspect ratio
            RectTransform rect = tabSystem.TabButtonParent.GetComponent<RectTransform>();
            if (tabSystem.ButtonLayoutType == TabSystem.LayoutType.Horizontal) rect.SetAnchorPreset(AnchorPresets.TopStretch);
            else if (tabSystem.ButtonLayoutType == TabSystem.LayoutType.Vertical) rect.SetAnchorPreset(AnchorPresets.StretchLeft);

            rect.position = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        private void UpdateButtonLayoutData()
        {
            if (tabSystem.TabButtonParent != null)
            {
                UnityEngine.Debug.Log("Updating layout group data!");
                tabSystem.TabButtonParent.TryGetComponent<HorizontalLayoutGroup>(out horizontalLayoutGroup);
                tabSystem.TabButtonParent.TryGetComponent<VerticalLayoutGroup>(out verticalLayoutGroup);

                //Udpate alignment and distance if changed
                if (horizontalLayoutGroup != null)
                {
                    horizontalLayoutGroup.childAlignment = tabSystem.ButtonLocation;
                    horizontalLayoutGroup.spacing = tabSystem.ButtonDistance;
                    horizontalLayoutGroup.reverseArrangement = tabSystem.ReverseButtonOrder;
                    horizontalLayoutGroup.padding = tabSystem.ButtonMargins;
                }
                else if (verticalLayoutGroup != null)
                {
                    verticalLayoutGroup.childAlignment = tabSystem.ButtonLocation;
                    verticalLayoutGroup.spacing = tabSystem.ButtonDistance;
                    verticalLayoutGroup.reverseArrangement = tabSystem.ReverseButtonOrder;
                    verticalLayoutGroup.padding = tabSystem.ButtonMargins;
                }
            }
        }

        private void ReorderTabs()
        {
            for (int k = 0; k < tabSystem.TabParent.transform.childCount; k++)
            {
                //We account for the tab buttons child, which is not a tabContainer by allowing one more than the total tabs
                if (k > tabSystem.Tabs.Count)
                {
                    UnityEngine.Debug.LogWarning($"Tried to set tabs of {tabSystem.TabParent.name} but there are other children that are not part of the TabSystem! " +
                        $"Remove them to ensure that the TabSystem behaves properly. Note: DO NOT REMOVE THE BUTTON PARENT GAMEOBJECT, ONLY OTHER GAMEOBJECTS");
                    return;
                }

                if (tabSystem.TabParent.transform.GetChild(k).TryGetComponent<TabContainer>(out TabContainer reorderTabContainer))
                {
                    for (int l = 0; l < tabSystem.Tabs.Count; l++)
                    {
                        if (tabSystem.Tabs[l].Name == reorderTabContainer.GetTitle())
                        {
                            reorderTabContainer.transform.SetSiblingIndex(l);
                            UnityEngine.Debug.Log($"Tab {reorderTabContainer.gameObject.name} is now at index {l}");
                            break;
                        }
                    }
                }
            }
            for (int k = 0; k < tabSystem.TabButtonParent.transform.childCount; k++)
            {
                if (k >= tabSystem.Tabs.Count)
                {
                    UnityEngine.Debug.LogWarning($"Tried to set tab buttons of {tabSystem.TabButtonParent.name} but there are other children that are not part of the TabSystem! " +
                        $"Remove them to ensure that the TabSystem behaves properly.");
                    return;
                }

                if (tabSystem.TabButtonParent.transform.GetChild(k).TryGetComponent<TabButton>(out TabButton reorderTabButton))
                {
                    for (int l = 0; l < tabSystem.Tabs.Count; l++)
                    {
                        if (tabSystem.Tabs[l].Name == reorderTabButton.GetTitle())
                        {
                            reorderTabButton.transform.SetSiblingIndex(l);
                            break;
                        }
                    }
                }
            }
        }

        private void AddedTabCheck()
        {
            Dictionary<TabSystem.TabInfo, int> tabsToCreate = new Dictionary<TabSystem.TabInfo, int>();
            for (int i = 0; i < tabSystem.Tabs.Count; i++)
            {
                bool isFound = false;
                TabSystem.TabInfo currentInfo = tabSystem.Tabs[i];
                for (int j = 0; j < tabSystem.TabParent.transform.childCount; j++)
                {
                    if (tabSystem.TabParent.transform.GetChild(j).TryGetComponent<TabContainer>(out TabContainer container) && container.GetTitle() == currentInfo.Name)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (!isFound) tabsToCreate.Add(currentInfo, i);
            }
            foreach (var tab in tabsToCreate) CreateNewTab(tab.Key, tab.Value);
        }

        private void RemovedTabCheck()
        {
            List<GameObject> removedObjects = new List<GameObject>();
            for (int i = 0; i < tabSystem.TabParent.transform.childCount; i++)
            {
                GameObject child = tabSystem.TabParent.transform.GetChild(i).gameObject;
                if (!child.TryGetComponent<TabContainer>(out TabContainer container)) continue;

                bool isFound = false;
                foreach (var tab in tabSystem.Tabs)
                {
                    if (tab.Name == container.GetTitle())
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    if (container.HasContents())
                    {
                        UnityEngine.Debug.LogWarning($"Tab {container.GetTitle()} no longer exists in the tab list and was marked for deletion, but it has content within it! Make sure to not remove tabs that have content!");
                        continue;
                    }

                    removedObjects.Add(child);

                    //Also delete that object's corresponding button
                    for (int j = 0; j < tabSystem.TabButtonParent.transform.childCount; j++)
                    {
                        if (tabSystem.TabButtonParent.transform.GetChild(j).TryGetComponent<TabButton>(out TabButton tabButton) && tabButton.GetTitle() == container.GetTitle())
                        {
                            removedObjects.Add(tabButton.gameObject);
                            break;
                        }
                    }
                }
            }
            foreach (var obj in removedObjects) DestroyImmediate(obj.gameObject);
        }

        private void TabNameUpdateCheck()
        {
            for (int i = 0; i < tabSystem.TabParent.transform.childCount; i++)
            {
                if (tabSystem.TabParent.transform.GetChild(i).TryGetComponent<TabContainer>(out TabContainer tabContainer))
                {
                    if (i >= tabSystem.Tabs.Count)
                    {
                        UnityEngine.Debug.LogWarning($"Tried to access tabs of {tabSystem.TabParent.name} but there are other children that are not part of the TabSystem! " +
                            $"Remove them to ensure that the TabSystem behaves properly. Note: DO NOT REMOVE THE BUTTON PARENT GAMEOBJECT, ONLY OTHER GAMEOBJECTS");
                        return;
                    }

                    if (tabContainer.GetTitle() != tabSystem.Tabs[i].Name)
                    {
                        int newIndex = -1;
                        for (int j = 0; j < tabSystem.Tabs.Count; j++)
                        {
                            if (tabContainer.GetTitle()== tabSystem.Tabs[j].Name) newIndex = j;
                        }

                        //If the index was found, it means that the tabs were reordered
                        if (newIndex != -1) ReorderTabs();

                        //if the index was not found it means we have changed the name
                        else
                        {
                            tabSystem.UpdateTabNames();
                            tabSystem.UpdateRectTransforms();
                        }
                    }
                }
            }
        }
    }
}

