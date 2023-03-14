using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SugarFrame.Tool
{
    public static class ScriptableObjectTool
    {
        public static T CreateSubAssetsIn<T>(this ScriptableObject so,string name = "")
            where T : ScriptableObject
        {
            var soData = ScriptableObject.CreateInstance<T>();
            soData.name = name;
            AssetDatabase.AddObjectToAsset(soData, so);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(so));
            return soData;
        }
    }
}
