using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

namespace SugarFrame.Tool
{
    public enum AssetType
    {
        ScriptableObject,
        Prefab,
    }


    public class AssetConfig : ScriptableObject
    {
        public AssetType assetType;
        [Space]

        [ShowIf(nameof(IsSO))]
        public ScriptableObject assetSo;
        [ShowIf(nameof(IsGO)),AssetsOnly]
        public GameObject assetGo;

        private UnityEngine.Object currentAsset
        {
            get { return assetType == AssetType.ScriptableObject ? assetSo : assetGo; ; }
        }

        [Space]
        [LabelText("创建时的默认名称")]
        public string defaultName = "New Data";

        [FolderPath, InlineButton(nameof(AutoSetPath))]
        public string assetsFolderPath;

        [Title("拓展SO")]
        public List<ScriptableObject> extraData;

        [Title("事件")]
        public bool useDefaultCreateFunction = true;
        public bool useDefaultDeleteFunction = true;

        public UnityEvent OnAssetChange;
        public UnityEvent OnAssetCreate;
        public UnityEvent OnAssetDelete;



        protected bool FileCheck()
        {
            if (string.IsNullOrEmpty(assetsFolderPath))
                return false;
            if (assetSo == null && assetType == AssetType.ScriptableObject)
                return false;
            if (assetGo == null && assetType == AssetType.Prefab)
                return false;

            return true;
        }

        public virtual UnityEngine.Object CreateAsset()
        {
            if (FileCheck() == false)
                return null;

            UnityEngine.Object obj = null;

            if(useDefaultCreateFunction)
            {
                if (assetType == AssetType.ScriptableObject)
                {
                    var so = ScriptableObject.CreateInstance(assetSo.GetType());
                    so.name = defaultName;
                    var createPath = $"{assetsFolderPath}/{so.name}.asset";
                    int tot = 0;

                    while (File.Exists(createPath) && tot++ < 1000)
                    {
                        so.name = defaultName + tot.ToString();
                        createPath = $"{assetsFolderPath}/{so.name}.asset";
                    }
                    AssetDatabase.CreateAsset(so, createPath);
                    obj = so;
                }
                if (assetType == AssetType.Prefab)
                {
                    var newGo = Instantiate(assetGo);
                    newGo.name = defaultName;

                    var createPath = $"{assetsFolderPath}/{newGo.name}.asset";
                    int tot = 0;

                    while (File.Exists(createPath) && tot++ < 1000)
                    {
                        newGo.name = defaultName + tot.ToString();
                        createPath = $"{assetsFolderPath}/{newGo.name}.asset";
                    }

                    PrefabUtility.SaveAsPrefabAsset(newGo, assetsFolderPath + "/" + newGo.name + ".prefab");
                    obj = PrefabUtility.LoadPrefabContents(assetsFolderPath + "/" + newGo.name + ".prefab");
                    DestroyImmediate(newGo);
                }
            }

            AssetDatabase.SaveAssets();
            OnAssetCreate?.Invoke();
            OnAssetChange?.Invoke();
            return obj;

        }

        public virtual void DeleteAsset(string assetFilePath)
        {
            if (FileCheck() == false)
                return;

            if(useDefaultDeleteFunction)
            {
                AssetDatabase.DeleteAsset(assetFilePath);
                AssetDatabase.Refresh();
            }
            OnAssetDelete?.Invoke();
            OnAssetChange?.Invoke();
        }

        public List<UnityEngine.Object> GetAllDatas()
        {
            List<UnityEngine.Object> list = new List<UnityEngine.Object>();

            if(extraData?.Count > 0)
            {
                extraData.ForEach(x => list.Add(x));
            }

            if (FileCheck() == false)
                return list;

            if (assetType == AssetType.ScriptableObject)
            {
                AssetsTool.GetPathAssets<ScriptableObject>(assetsFolderPath)?.ForEach(x => {
                    if (x.GetType() == currentAsset.GetType())
                        list.Add(x);
                });
            }
                
            if (assetType == AssetType.Prefab)
                AssetsTool.GetPathAssets<GameObject>(assetsFolderPath)?.ForEach((x) => list.Add(x));

            //list.Sort();
            

            return list;
        }
        /// <summary>
        /// 获取当前资源的目录路径
        /// </summary>
        private void AutoSetPath()
        {
            UnityEngine.Object asset = assetType == AssetType.ScriptableObject ? assetSo : assetGo;
            if (asset != null)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                int index = path.LastIndexOf('/');
                int len = path.Length - index;
                assetsFolderPath = path.Remove(index, len);
            }
        }


        private bool IsSO()
        {
            return assetType==AssetType.ScriptableObject;
        }
        private bool IsGO()
        {
            return assetType == AssetType.Prefab;
        }
    }
}


