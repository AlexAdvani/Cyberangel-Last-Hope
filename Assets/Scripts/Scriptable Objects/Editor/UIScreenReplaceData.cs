using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Editor Data/Platform UI Screen Data")]
public class UIScreenReplaceData : ScriptableObject
{
    [Inspectionary("Target Platform", "Screen Prefab")]
    public PlatformUIScreenDictionary dPlatformScreens;

    public GameObject goDestinationScreenPrefab;
}

[System.Serializable]
public class PlatformUIScreenDictionary : SerializableDictionary<BuildTarget, GameObject>
{ }
