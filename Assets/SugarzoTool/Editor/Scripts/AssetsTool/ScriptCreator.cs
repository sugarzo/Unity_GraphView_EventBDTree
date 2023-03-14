using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace SugarFrame.Tool
{
    [CreateAssetMenu(menuName = "SugarzoFrame/EditorTool/ScriptCreator")]
    public class ScriptCreator : BaseAssetCreator
    {
        public TextAsset prototype;

        public override void Create()
        {
            if (IsEmptyVariable() || prototype == null)
                return;
            

            //如果不存在该文件夹，创建它
            if (!Directory.Exists(createPath))
            {
                Directory.CreateDirectory(createPath);
            }
            string filepath = createPath + "/" + createFileName + ".cs"; //选择的文件路径;
            if(File.Exists(filepath))
            {
                Debug.Log("目标路径中存在同名文件，请检查配置");
                return;
            }
            Debug.Log("保存文件 " + filepath);

            var fStream = File.Create(filepath);
            var bytes = System.Text.Encoding.UTF8.GetBytes(code);
            fStream.Write(bytes, 0, bytes.Length);

            fStream.Close();

            AssetDatabase.Refresh();
        }

        [TextArea(20, 30), ReadOnly]
        [Title("Preview"), PropertyOrder(100)]
        public string code;

        protected virtual void OnValidate()
        {
            if (prototype)
            {
                code = prototype.ToString().Replace("#TTT#", createFileName);
            }
            else
            {
                code = "缺少脚本的模板文件";
            }
        }
    }
}