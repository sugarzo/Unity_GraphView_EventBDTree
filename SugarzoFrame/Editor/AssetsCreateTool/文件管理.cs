using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

public static class FileManager
{
    public static void OpenFile()
    {
        OpenFileDlg ofd = new OpenFileDlg();
        ofd.structSize = Marshal.SizeOf(ofd);
        ofd.filter = "txt files\0*.txt\0All Files\0*.*\0\0";
        ofd.file = new string(new char[256]);
        ofd.maxFile = ofd.file.Length;
        ofd.fileTitle = new string(new char[64]);
        ofd.maxFileTitle = ofd.fileTitle.Length;
        ofd.initialDir = Application.dataPath; //默认路径
        ofd.title = "打开文件";
        ofd.defExt = "txt";
        ofd.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (OpenFileDialog.GetOpenFileName(ofd))
        {
            string filepath = ofd.file; //选择的文件路径;  
            Debug.Log("打开 " + filepath);
        }
    }
    public static void SaveFile()
    {
        SaveFileDlg sfd = new SaveFileDlg();
        sfd.structSize = Marshal.SizeOf(sfd);
        sfd.filter = "txt files\0*.txt\0All Files\0*.*\0\0";
        sfd.file = new string(new char[256]);
        sfd.maxFile = sfd.file.Length;
        sfd.fileTitle = new string(new char[64]);
        sfd.maxFileTitle = sfd.fileTitle.Length;
        sfd.initialDir = Application.dataPath; //默认路径
        sfd.title = "保存文件";
        sfd.defExt = "txt";
        sfd.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (SaveFileDialog.GetSaveFileName(sfd))
        {
            string filepath = sfd.file; //选择的文件路径;
            Debug.Log("保存 " + filepath);
        }
    }
    //保存脚本文件
    public static void SaveScriptFile(string fileTitle,string template,string defaultFolderPath = "")
    {
        SaveFileDlg sfd = new SaveFileDlg();
        sfd.structSize = Marshal.SizeOf(sfd);
        sfd.filter = "cs files\0*.cs\0All Files\0*.*\0\0";
        sfd.file = new string(new char[256]);
        sfd.maxFile = sfd.file.Length;
        sfd.fileTitle = new string(new char[64]);
        sfd.maxFileTitle = sfd.fileTitle.Length;
        sfd.initialDir = Application.dataPath; //默认路径
        sfd.title = "保存文件";
        sfd.defExt = "txt";
        sfd.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        sfd.file = new string(fileTitle);

        if (SaveFileDialog.GetSaveFileName(sfd))
        {
            string filepath = sfd.file; //选择的文件路径;
            Debug.Log("保存 " + filepath);

            var fStream = File.Create(filepath);
            var bytes = System.Text.Encoding.UTF8.GetBytes(template);
            fStream.Write(bytes, 0, bytes.Length);

            fStream.Close();
        }
        
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class FileDlog
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileDlg : FileDlog
{
}

public class OpenFileDialog
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileDlg ofn);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class SaveFileDlg : FileDlog
{
}

public class SaveFileDialog
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] SaveFileDlg ofn);
}


public static class FileHelper
{
    public static List<T> GetFiles<T>(string dir) where T : UnityEngine.Object
    {
        string path = string.Format(dir);
        var list = new List<T>();
        //获取指定路径下面的所有资源文件  
        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*");

            for (int i = 0; i < files.Length; i++)
            {
                //忽略关联文件
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
#if UNITY_EDITOR
                var so = AssetDatabase.LoadAssetAtPath<T>(dir + "/" + files[i].Name);
                if (so != null)
                {
                    Debug.Log("加载资源" + files[i].Name);
                    list.Add(so as T);
                }
#endif
            }
        }

        return list;
    }
}
