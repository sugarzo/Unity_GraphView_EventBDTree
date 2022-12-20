using UnityEditor;
using UnityEngine;

public class ScriptableObjectCreatorT<T> : BaseAssetCreator where T : ScriptableObject
{
    public override void Create()
    {
        if (IsEmptyVariable())
            return;

        var go = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(go, createPath + "/" + createFileName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}