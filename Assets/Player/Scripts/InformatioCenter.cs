using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml;
using UnityEngine.Networking;
using System.Data;

public class InformatioCenter : ConfigDataBase
{
    public override T ReadJson<T>(string path, Action<T> e)
    {
        Type t = typeof(T);
        if (t == typeof(JsonInformation))
        {
            CoroutineStarter.Start(LoadAndParseJson("Data/" + path + ".json", e));
            return default(T); // 异步加载，立即返回默认值
        }

        OnLoadFail("Json无此结构");
        return default(T);
    }

    private IEnumerator LoadAndParseJson<T>(string configName, Action<T> callback)
    {
        string path;
#if UNITY_WIN_STANDALONE || (UNITY_IPHONE && !UNITY_EDITOR)
        path = "file://" + Application.streamingAssetsPath + "/" + configName;
#else
        path = Application.streamingAssetsPath + "/" + configName;
#endif

        Debug.Log("Loading JSON from: " + path);

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(path))
        {
            yield return unityWebRequest.SendWebRequest();

            // 兼容旧版本Unity的错误检查
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                OnLoadFail("Json加载失败: " + unityWebRequest.error);
            }
            else
            {
                string jsonContent = unityWebRequest.downloadHandler.text;
                try
                {
                    // 尝试解析JSON
                    if (typeof(T) == typeof(JsonInformation))
                    {
                        // 特殊处理JsonInformation类型
                        JsonInformation info = JsonUtility.FromJson<JsonInformation>(jsonContent);
                        callback((T)(object)info);
                    }
                    else
                    {
                        // 通用解析
                        T data = JsonUtility.FromJson<T>(jsonContent);
                        callback(data);
                    }
                    OnLoadSuccess();
                }
                catch (Exception ex)
                {
                    OnLoadFail("Json解析失败: " + ex.Message + "\n" + jsonContent);
                }
            }
        }
    }


    public IEnumerator TextReader(string configName, Action<string> action = null)
    {
        string path;
#if UNITY_WIN_STANDALONE || UNITY_IPHONE && !UNITY_EDITOR
        path ="file://"+ Application.streamingAssetsPath + configName;
#else
        path = Application.streamingAssetsPath + "/" + configName;
        Debug.Log(path);
#endif
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(path);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.error != null)
        {
            OnLoadFail("Json路径错误\n" + unityWebRequest.error);
        }
        else
        {
            string content = unityWebRequest.downloadHandler.text;
            if (action != null)
                action(content);
        }
    }

    public void AddConfigData()
    {

    }

    public void GetConfigData()
    {

    }

    public void OnLoadSuccess()
    {

    }

    public void OnLoadFail(string str)
    {
        Debug.LogError(str);
    }

    public override object ReadDateSet()
    {
        throw new NotImplementedException();
    }
}
