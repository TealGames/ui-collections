using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public static class EditorHelperFunctions
{

    /// <summary>
    /// Normally when instantiating it creates a clone, but this can instantiate it as a prefab instance instead
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateAsPrefab(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        GameObject gameObject= InstantiateAsPrefab(prefab, parent: parent);

        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.transform.localScale = scale;

        return gameObject;
    }

    /// <summary>
    /// Normally when instantiating it creates a clone, but this can instantiate it as a prefab instance instead
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateAsPrefab(GameObject prefab, Transform parent = null)
    {
        GameObject gameObject;
        if (parent != null) gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        else gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        return gameObject;
    }

    public static Sprite GetSpriteAtPath(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }


}