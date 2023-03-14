using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SugarFrame.Tool;
using System.IO;

namespace SugarFrame.Node
{
    [CreateAssetMenu(menuName = "SugarzoFrame/EditorTool/NodeScriptCreator")]
    public class NodeScriptCreator : ScriptCreator
    {
        [Space]
        [Title("配置新节点信息")]
        public string upFolderName = "";
        [LabelText("节点说明")]
        public string nodeNote = "";
        [ValueDropdown(nameof(TypeValueDrop))]
        public string packageType = "UnityBase";

        public override void Create()
        {
            //获取路径
            createPath = NodePackageType.GetPathByType(packageType);
            if(!string.IsNullOrEmpty(createPath))
            {
                createPath += "/" + upFolderName;
                base.Create();
            }
            else
            {
                Debug.Log("未检测到路径");
            }
        }

        protected override void OnValidate()
        {
            if (prototype)
            {
                code = prototype.ToString().Replace("#TTT#", createFileName).Replace("#T1#",nodeNote).Replace("#T2#",packageType.ToString());
            }
            else
            {
                code = "缺少脚本的模板文件";
            }
        }
        public List<string> TypeValueDrop()
        {
            List<string> result = new List<string>();
            result = NodePackageType.GetAllTypeStr();
            return result;
        }
    }
   
}