using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SugarFrame.Tool
{
    [CreateAssetMenu(menuName = "SugarzoFrame/EditorTool/EnumTool")]
    public class EnumTool : ScriptableObject
    {
        public MonoScript enumFile;
        [Space]
        [InlineButton(nameof(Inject))]
        public string enumInject;
        [InlineButton(nameof(Remove))]
        [ValueDropdown(nameof(GetAllTypeFromThisEnum))]
        public string enumRemove;

        public void Inject()
        {
            if(enumFile != null && !string.IsNullOrEmpty(enumInject))
            {
                var list = GetAllTypeFromThisEnum();
                foreach (var item in list)
                {
                    if (item.Equals(enumInject))
                    {
                        Debug.Log("该枚举已存在");
                        return;
                    }
                }

                AddEnumValue(enumInject, AssetDatabase.GetAssetPath(enumFile));
                AssetDatabase.Refresh();
                // CompilationPipeline.RequestScriptCompilation();
                OnValidate();
            }
        }

        //请谨慎使用枚举移除功能。在Unity Inspector引用枚举时使用的是数字序号标记
        //移除中间的枚举类型时，会导致后续的枚举顺序向前移动一位，以前的引用容易乱序

        /// <summary>
        /// 枚举移除
        /// </summary>
        public void Remove()
        {
            if (enumFile != null && !string.IsNullOrEmpty(enumRemove))
            {
                RemoveEnumValue(enumRemove, AssetDatabase.GetAssetPath(enumFile));
                AssetDatabase.Refresh();
                //CompilationPipeline.RequestScriptCompilation();
                OnValidate();
            }
            
        }

        public List<string> GetAllTypeFromThisEnum()
        {
            if (enumFile == null || enumFile == null)
                return null;

            //在枚举脚本中给添加namespace，使用getclass会返回空引用报错...
            //查到了这是unity monoscript的一个bug，在2022.2.x版本后修复了。这里获取列表还是使用文本读入读出吧

            //if (enumFile.GetClass().IsEnum)
            //{
            //    return Enum.GetNames(enumFile.GetClass())?.ToList();
            //}

            string[] lines = enumFile.text.Split("\n");

            List<string> ret = new();
            bool isEnum = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Contains($"enum"))
                {
                    isEnum = true;
                }
                if (line.Contains(",") && isEnum)
                {
                    ret.Add(lines[i].Trim().Replace(",", ""));
                }
            }
            return ret;
        }

        
        [TextArea(20, 30), ReadOnly]
        [Title("Code Preview"), PropertyOrder(100)]
        public string code;

        protected virtual void OnValidate()
        {
            code = enumFile == null ? "缺少模板脚本" : enumFile.text;
        }

        public static void AddEnumValue(string newEnumValue, string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            bool inEnum = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];//.Trim();

                if (inEnum && line.Contains("}"))
                {
                    lines[i] = lines[i-1].Substring(0, lines[i-1].Length - lines[i - 1].TrimStart().Length) + $"{newEnumValue},\r\n{line}";
                    inEnum = false;
                    break;
                }

                if (line.Contains($"enum"))
                {
                    inEnum = true;
                }
                lines[i] = lines[i].Replace("\r", "").Replace("\n", "\r\n");
            }

            File.WriteAllLines(filePath, lines);
        }
        public static void RemoveEnumValue(string enumValueToRemove, string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.Contains(enumValueToRemove + ","))
                {
                    lines[i] = "";
                    Debug.Log("已移除对应枚举");
                }
                lines[i] = lines[i].Replace("\r", "").Replace("\n", "\r\n");
            }
            var list = lines.ToList();
            //这里的目的是移除空行
            list.RemoveAll(x => x.Trim() == "\r\n" || string.IsNullOrWhiteSpace(x.Trim()));

            File.WriteAllLines(filePath, list);
        }
    }
}