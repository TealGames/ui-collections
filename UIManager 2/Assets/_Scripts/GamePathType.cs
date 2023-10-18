using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The type of path starts allowed when managing and doing System.IO actions. It allows shortened paths when performing file-related actions
    /// <br></br>
    /// Since the paths and structures can be different for different devices,
    /// by abstracting the root path, it prevent hard-coded paths by using the <see cref="Game.Utilities.HelperFunctions.GetPathFromPathType(GamePathType)"/> to get the path from the type.
    /// <br></br>
    /// <br><see cref="GamePathType.Game"/> is used for file actions within the game files. It references the path from <see cref="Application.dataPath"/>.</br>
    /// <br>See --> https://docs.unity3d.com/ScriptReference/Application-dataPath.html for more info</br>
    /// <br></br>
    /// <br><see cref="GamePathType.StreamingAsset"/> is used when getting assets from the 'StreamingAssets' folder. It references the path from <see cref="Application.streamingAssetsPath"/></br>
    /// <br>See --> https://docs.unity3d.com/ScriptReference/Application-streamingAssetsPath.html for more info</br>
    /// <br></br>
    /// <br><see cref="GamePathType.PersistentSave"/> is used for storing persisent data, such as player preferences, player save data, save files, etc. It references the path from <see cref="Application.persistentDataPath"/>.</br>
    /// <br>See --> https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html for more info</br>
    /// </summary>
    public enum GamePathType
    {
        Game,
        StreamingAsset,
        PersistentSave
    }

}
