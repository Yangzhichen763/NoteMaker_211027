using UnityEngine;
using System.Text;
using LitJson;// 这是一个dll
using System.IO;
using System.Runtime.InteropServices;
using System;

public static class JsonDo
{
    /// <summary>写入文件</summary>
    /// <typeparam name="T">要被写入的类型</typeparam>
    /// <param name="UnitySubPath"></param>
    /// <returns></returns>
    public static string SaveToJson<T>(T t, string UnitySubPath)
    {
        string filePath = Application.dataPath + UnitySubPath;

        // 新建一个文件夹方便存储
        new FileInfo(filePath);

        // 将 T 类型的 t 转化为 Json
        string jsonStr = JsonMapper.ToJson(t);

        // 写入数据
        using (StreamWriter fileWrite = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            fileWrite.WriteLine(jsonStr);
        }

#if UNITY_EDITOR
        Debug.Log($"<color=green>数据存储成功</color>!存储地址为:{filePath}");
        UnityEditor.AssetDatabase.Refresh();
#endif
        return jsonStr;
    }
    public static string SaveToJson<T>(T t, string filePath,string fileName,string name)
    {
        // 新建一个文件夹方便存储
        string fileTotalPath = $"{filePath}/{fileName}";
        if (!Directory.Exists(fileTotalPath))
            Directory.CreateDirectory(fileTotalPath);

        string jsonPath = $"{fileTotalPath}/{name}";
        new FileInfo(jsonPath);

        // 将 T 类型的 t 转化为 Json
        string jsonStr = JsonMapper.ToJson(t);

        // 写入数据
        using (StreamWriter fileWrite = new StreamWriter(jsonPath, false, Encoding.UTF8))
        {
            fileWrite.WriteLine(jsonStr);
        }

#if UNITY_EDITOR
        Debug.Log($"<color=green>数据存储成功</color>!存储地址为:{jsonPath}");
        UnityEditor.AssetDatabase.Refresh();
#endif
        return jsonStr;
    }
    /// <summary>从文件中读取</summary>
    /// <typeparam name="T">要被读取的类型</typeparam>
    /// <param name="UnitySubPath"></param>
    /// <returns></returns>
    public static T LoadFromJson<T>(string UnitySubPath)
    {
        try
        {
            string filePath = Application.dataPath + UnitySubPath;

            // 将文件转化为指定类型
            T t = JsonMapper.ToObject<T>(File.ReadAllText(filePath));

            if (t == null)
            {
#if UNITY_EDITOR
                Debug.Log("<color=red>读取Json文件失败</color>!");
#endif
                return default(T);
            }

            return t;
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return default(T);
        }
    }
    /// <summary>从文件中读取</summary>
    /// <typeparam name="T">要被读取的类型</typeparam>
    /// <param name="UnitySubPath"></param>
    /// <returns></returns>
    public static T LoadFromJson<T>(TextAsset asset)
    {
        try
        {
            // 将文件转化为指定类型
            T t = JsonMapper.ToObject<T>(asset.text);

            if (t == null)
            {
#if UNITY_EDITOR
                Debug.Log("<color=red>读取Json文件失败</color>!");
#endif
                return default(T);
            }

            return t;
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return default(T);
        }
    }

    public static bool FindPath(string UnitySubPath)
    {
        string filePath = Application.dataPath + UnitySubPath;
        return File.Exists(filePath);
    }


    // 与 explorer 的操作
    public static void Save(string saveName)
    {
        
    }
}
