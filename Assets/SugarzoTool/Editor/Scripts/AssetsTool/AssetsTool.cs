using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SugarFrame.Tool
{
    public static class AssetsTool
    {
        /// <summary>
        /// 获得路径下所有类型资源
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isRecursive"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetPathAssets<T>(string dir,
            bool isRecursive = true) where T : class
        {
            var list = new List<T>();
#if UNITY_EDITOR
            if (!Directory.Exists(dir))
            {
                return list;
            }
            //获取指定路径下面的所有资源文件  
            var files =
                Directory.GetFiles(dir,
                    "*",
                    isRecursive
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var f = file.Replace(Application.dataPath, "Assets");
                var script = AssetDatabase.LoadAssetAtPath(f, typeof(T));
                if (script != null)
                {
                    list.Add(script as T);
                }
            }
#endif


            return list;
        }

        public static List<T> GetPathsAssets<T>(params string[] paths) where T : class
        {
            var list = new List<T>();
            //获取指定路径下面的所有资源文件  
            foreach (var path in paths)
            {
                list.AddRange(GetPathAssets<T>(path, true));
            }
            return list;
        }

        /// <summary>
        /// 获取路径下所有类型资源的路径
        /// </summary>
        /// <param name="dir"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetFolderAssetsPaths<T>(string dir)
            where T : class
        {
            var list = new List<string>();
#if UNITY_EDITOR
            //获取指定路径下面的所有资源文件  
            var files =
                Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var script = AssetDatabase.LoadAssetAtPath(file, typeof(T));
                if (script != null)
                {
                    list.Add(file);
                }
            }
#endif
            return list;
        }



        /// <summary>
        /// 从模板创建新的asset
        /// </summary>
        /// <param name="pfbPath"></param>
        /// <param name="createPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreatePfbFromTemplate<T>(string pfbPath,
            string createPath) where T : class
        {
#if UNITY_EDITOR
            var asset = AssetDatabase.LoadAssetAtPath(pfbPath, typeof(T));
            if (asset)
            {
                if (AssetDatabase.CopyAsset(pfbPath, createPath))
                {
                    return AssetDatabase.LoadAssetAtPath(createPath, typeof(T))
                        as T;
                }
            }
#endif
            return default;
        }

        /// <summary>
        /// 获取对应组件引用该资源的预制体
        /// </summary>
        /// <param name="assetsPath"></param>
        /// <param name="pfbsPath"></param>
        /// <param name="referenceComponents"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public static void GetReferenceAssetInPfbComponents<T1, T2>(
            List<string> assetsPath, string pfbsPath,
            out List<T2> referenceComponents)
            where T1 : class where T2 : Component
        {
            var assets = new List<T1>();
            referenceComponents = new List<T2>();

#if UNITY_EDITOR
            foreach (var assetPath in assetsPath)
            {
                if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(T1)) is T1
                    asset)
                {
                    assets.Add(asset);
                }
            }

            var pfbs = GetPathAssets<GameObject>(pfbsPath);
            foreach (var pfb in pfbs)
            {
                var components = pfb.GetComponentsInChildren<T2>();
                foreach (var component in components)
                {
                    if (assets is List<Sprite> sprites)
                    {
                        if (component is Image image)
                        {
                            if (sprites.Contains(image.sprite))
                            {
                                referenceComponents.Add(component);
                            }
                        }
                        else if (component is SpriteRenderer spriteRenderer)
                        {
                            if (sprites.Contains(spriteRenderer.sprite))
                            {
                                referenceComponents.Add(component);
                            }
                        }
                        else
                        {
                            Debug.Log("未配置规则");
                        }
                    }

                    else
                    {
                        Debug.Log("未配置规则");
                    }
                }
            }
#endif
        }

        public static void BindRenderSprite(SpriteRenderer spriteRenderer, string spritePath, string name)
        {
            var sprites = GetPathAssets<Sprite>(spritePath);
            foreach (var sprite in sprites.Where(sprite => sprite.name.Equals(name)))
            {
                spriteRenderer.sprite = sprite;
                break;
            }
        }

        public static void BindRenderSprite(Image image, string spritePath, string name)
        {
            var sprites = GetPathAssets<Sprite>(spritePath);
            foreach (var sprite in sprites.Where(sprite => sprite.name.Equals(name)))
            {
                image.sprite = sprite;
                break;
            }
        }
    }
}