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
        [Title("�����½ڵ���Ϣ")]
        public string upFolderName = "";
        [LabelText("�ڵ�˵��")]
        public string nodeNote = "";
        [ValueDropdown(nameof(TypeValueDrop))]
        public string packageType = "UnityBase";

        public override void Create()
        {
            //��ȡ·��
            createPath = NodePackageType.GetPathByType(packageType);
            if(!string.IsNullOrEmpty(createPath))
            {
                createPath += "/" + upFolderName;
                base.Create();
            }
            else
            {
                Debug.Log("δ��⵽·��");
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
                code = "ȱ�ٽű���ģ���ļ�";
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