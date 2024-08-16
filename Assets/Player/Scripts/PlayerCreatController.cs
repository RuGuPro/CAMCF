using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerCreatController : MonoBehaviour
{
    public InfoConfig infoConfig;

    private void Start()
    {
        UpdateJsonInfo();
    }

    /// <summary>
    /// 更新Json数据
    /// </summary>
    public void UpdateJsonInfo()
    {
        InformatioCenter InformatioCenter = new InformatioCenter();
        InformatioCenter.ReadJson<JsonInformation>("Jsondata", (t) =>
        {
            infoConfig.CameraInfo.openMotionCapture = t.openMotionCapture;
            infoConfig.CameraInfo.ipServer = t.ipServer;
            infoConfig.CameraInfo.port = t.port;
            infoConfig.CameraInfo.motionCaptureBrand = (MotionCaptureBrand)System.Enum.Parse(typeof(MotionCaptureBrand), t.motionCaptureBrand);
            infoConfig.CameraInfo.cameraType = (CameraType)System.Enum.Parse(typeof(CameraType), t.cameraType);
            infoConfig.CameraInfo.headTag = t.headTag;
            infoConfig.CameraInfo.handTag = t.handTag;
            infoConfig.CameraInfo.open3D = t.open3D;
            infoConfig.CameraInfo.allSceneHigh = t.allSceneHigh;
            infoConfig.CameraInfo.sideSceneLong = t.sideSceneLong;
            infoConfig.CameraInfo.frontSceneLong = t.frontSceneLong;
            infoConfig.CameraInfo.groundSceneWide = t.groundSceneWide;
            infoConfig.CameraInfo.deltaXDirection = t.deltaXDirection;
            infoConfig.CameraInfo.deltaYDirection = t.deltaYDirection;
            infoConfig.CameraInfo.deltaZDirection = t.deltaZDirection;
            infoConfig.CameraInfo.deltaXRotDirection = t.deltaXRotDirection;
            infoConfig.CameraInfo.deltaYRotDirection = t.deltaYRotDirection;
            infoConfig.CameraInfo.deltaZRotDirection = t.deltaZRotDirection;
            infoConfig.CameraInfo.deltaX = t.deltaX;
            infoConfig.CameraInfo.deltaY = t.deltaY;
            infoConfig.CameraInfo.deltaZ = t.deltaZ;
            infoConfig.CameraInfo.deltaRotX = t.deltaRotX;
            infoConfig.CameraInfo.deltaRotY = t.deltaRotY;
            infoConfig.CameraInfo.deltaRotZ = t.deltaRotZ;

            ReadJsonFinish(infoConfig);
        });
    }

    /// <summary>
    /// 删除当前玩家
    /// </summary>
    public void DeletCurPlayer()
    {
        Destroy(this.transform.Find("Player").gameObject);
    }

    /// <summary>
    /// 生成玩家
    /// </summary>
    public void CreatPlayer(InfoConfig _infoConfig, Action<GameObject> endEvent = null)
    {
        string path = motionCapturePath(_infoConfig) + "/" + cameraTypePath(_infoConfig) + "/Player";
        StartCoroutine(LoadObjOver(path, (player) =>
        {
            player.GetComponent<UpdataCamera>().UpdataMotionCapture(_infoConfig);
            player.GetComponent<UpdataCamera>().UpdataCameraType(_infoConfig);
            endEvent?.Invoke(player);
        }));
    }

    /// <summary>
    /// 动捕路径
    /// </summary>
    public string motionCapturePath(InfoConfig _infoConfig)
    {
        string tempStr = "";
        switch (_infoConfig.CameraInfo.motionCaptureBrand)
        {
            case MotionCaptureBrand.青瞳:
                tempStr = "QingTong";
                break;
            case MotionCaptureBrand.瑞立视:
                tempStr = "RuiLiShi";
                break;
            case MotionCaptureBrand.度量:
                tempStr = "DuLiang";
                break;
        }
        return tempStr;
    }

    /// <summary>
    /// 屏幕类型
    /// </summary>
    public string cameraTypePath(InfoConfig _infoConfig)
    {
        string tempStr = "";
        switch (_infoConfig.CameraInfo.cameraType)
        {
            case CameraType.大屏:
                tempStr = "DaPing";
                break;
            case CameraType.三面屏:
                tempStr = "Cave3";
                break;
            case CameraType.四面屏:
                tempStr = "Cave4";
                break;
        }
        return tempStr;
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="endEvent"></param>
    /// <returns></returns>
    IEnumerator LoadObjOver(string _path, Action<GameObject> endEvent = null)
    {
        ResourceRequest rq = Resources.LoadAsync<GameObject>(_path);
        Debug.Log(Time.frameCount);
        yield return rq;
        GameObject perfab = rq.asset as GameObject;
        GameObject obj = Instantiate(perfab, this.transform);
        obj.name = "Player";
        obj.transform.localPosition = new Vector3(0, 0, 0);
        Debug.Log(Time.frameCount);
        endEvent?.Invoke(obj);
    }

    public void ReadJsonFinish(InfoConfig _infoConfig, Action<GameObject> endEvent = null)
    {
        DeletCurPlayer();
        CreatPlayer(_infoConfig, endEvent);
    }
}
