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
            JsonInformation tempJsonInformations = new JsonInformation();
            CoroutineStarter.Start(TextReader("Data/" + path + ".json", (str) =>
            {
                string temp = str;
                tempJsonInformations = JsonUtility.FromJson<JsonInformation>(temp);
                e((T)(System.Object)tempJsonInformations);
            }));
            return (T)(System.Object)tempJsonInformations;
        }
        OnLoadFail("Json无此结构");
        return default(T);
    }
   
    public IEnumerator TextReader(string configName, Action<string> action = null)
    {
        string path;
#if UNITY_WIN_STANDALONE || UNITY_IPHONE && !UNITY_EDITOR
        path ="file://"+ Application.streamingAssetsPath + configName;
#else
        path = Application.streamingAssetsPath + "/" + configName;
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
