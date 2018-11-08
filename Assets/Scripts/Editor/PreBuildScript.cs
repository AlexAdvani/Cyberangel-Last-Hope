using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PreBuildScript : IPreprocessBuildWithReport
{
    UIScreenReplaceData[] aScreenData;

    int iCallbackOrder = 0;

    #region Preprocess Build Integration

    public int callbackOrder
    {
        get { return iCallbackOrder; }
    }

    #endregion

    // On Preprocess Build
    public void OnPreprocessBuild(BuildReport report)
    {
        aScreenData = GetAllInstances<UIScreenReplaceData>();

        Debug.Log(aScreenData.Length);

        for (int i = 0; i < aScreenData.Length; i++)
        {
            ReplaceScreenUIPrefab(report, aScreenData[i]);
        }

        AssetDatabase.Refresh();
    }

    private void ReplaceScreenUIPrefab(BuildReport report, UIScreenReplaceData screenData)
    {
        if (!screenData.dPlatformScreens.ContainsKey(report.summary.platform))
        {
            return;
        }

        Debug.Log("Replace!");

        string originalPath = AssetDatabase.GetAssetPath(screenData.dPlatformScreens[report.summary.platform]);
        Debug.Log(originalPath);
        string destinationPath = AssetDatabase.GetAssetPath(screenData.goDestinationScreenPrefab);
        Debug.Log(destinationPath);

        FileUtil.ReplaceFile(originalPath, destinationPath);
    }

    private T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }
}
