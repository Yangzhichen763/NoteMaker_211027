using UnityEngine;
using System.Text;
using LitJson;// ����һ��dll
using System.IO;
using System.Runtime.InteropServices;
using System;

public static class JsonDo
{
    /// <summary>д���ļ�</summary>
    /// <typeparam name="T">Ҫ��д�������</typeparam>
    /// <param name="UnitySubPath"></param>
    /// <returns></returns>
    public static string SaveToJson<T>(T t, string UnitySubPath)
    {
        string filePath = Application.dataPath + UnitySubPath;

        // �½�һ���ļ��з���洢
        new FileInfo(filePath);

        // �� T ���͵� t ת��Ϊ Json
        string jsonStr = JsonMapper.ToJson(t);

        // д������
        using (StreamWriter fileWrite = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            fileWrite.WriteLine(jsonStr);
        }

#if UNITY_EDITOR
        Debug.Log($"<color=green>���ݴ洢�ɹ�</color>!�洢��ַΪ:{filePath}");
        UnityEditor.AssetDatabase.Refresh();
#endif
        return jsonStr;
    }
    public static string SaveToJson<T>(T t, string filePath,string fileName,string name)
    {
        // �½�һ���ļ��з���洢
        string fileTotalPath = $"{filePath}/{fileName}";
        if (!Directory.Exists(fileTotalPath))
            Directory.CreateDirectory(fileTotalPath);

        string jsonPath = $"{fileTotalPath}/{name}";
        new FileInfo(jsonPath);

        // �� T ���͵� t ת��Ϊ Json
        string jsonStr = JsonMapper.ToJson(t);

        // д������
        using (StreamWriter fileWrite = new StreamWriter(jsonPath, false, Encoding.UTF8))
        {
            fileWrite.WriteLine(jsonStr);
        }

#if UNITY_EDITOR
        Debug.Log($"<color=green>���ݴ洢�ɹ�</color>!�洢��ַΪ:{jsonPath}");
        UnityEditor.AssetDatabase.Refresh();
#endif
        return jsonStr;
    }
    /// <summary>���ļ��ж�ȡ</summary>
    /// <typeparam name="T">Ҫ����ȡ������</typeparam>
    /// <param name="UnitySubPath"></param>
    /// <returns></returns>
    public static T LoadFromJson<T>(string UnitySubPath)
    {
        try
        {
            string filePath = Application.dataPath + UnitySubPath;

            // ���ļ�ת��Ϊָ������
            T t = JsonMapper.ToObject<T>(File.ReadAllText(filePath));

            if (t == null)
            {
#if UNITY_EDITOR
                Debug.Log("<color=red>��ȡJson�ļ�ʧ��</color>!");
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
    /// <summary>���ļ��ж�ȡ</summary>
    /// <typeparam name="T">Ҫ����ȡ������</typeparam>
    /// <param name="UnitySubPath"></param>
    /// <returns></returns>
    public static T LoadFromJson<T>(TextAsset asset)
    {
        try
        {
            // ���ļ�ת��Ϊָ������
            T t = JsonMapper.ToObject<T>(asset.text);

            if (t == null)
            {
#if UNITY_EDITOR
                Debug.Log("<color=red>��ȡJson�ļ�ʧ��</color>!");
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


    // �� explorer �Ĳ���
    public static void Save(string saveName)
    {
        
    }
}
