using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SugarzoFrame/EditorTool/ScriptCreator")]
public class ScriptCreator : BaseAssetCreator
{
    public TextAsset prototype;

    public override void Create()
    {
        if (IsEmptyVariable() || prototype == null)
            return;

        string filepath = createPath + "/" + createFileName + ".cs"; //选择的文件路径;
        Debug.Log("保存 " + filepath);

        var fStream = File.Create(filepath);
        var bytes = System.Text.Encoding.UTF8.GetBytes(code);
        fStream.Write(bytes, 0, bytes.Length);

        fStream.Close();

        AssetDatabase.Refresh();
    }

    [TextArea(20, 30), ReadOnly]
    public string code;

    private void OnValidate()
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
