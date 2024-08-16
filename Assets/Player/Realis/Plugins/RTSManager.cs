using UnityEngine;
using Realis;
using System;

[System.Serializable]
public struct RTSInfo
{
    public GameObject useRTSGO;
    public string RTSName;
    public bool isUsePosition;
    public bool isUseRotation;
}
public class RTSManager : MonoBehaviour
{

    [HeaderAttribute("RTS参数")]
    [SerializeField]
    private string RTSServer = "192.168.43.110";

    [HeaderAttribute("RTS信息适配")]
    [SerializeField]
    private RTSInfo[] RTSInfos;

    public bool eyeLock = false;

    public Action<RTSInfo, Vector3> UpdatePos;
    public Action<RTSInfo, Quaternion> UpdateRot;

    // Update is called once per frame
    void Update()
    {
        UpdateUseRTSGOTransform();
    }

    /// <summary>
    /// 将所有需要RTS的游戏物体的相关信息储存到结构体数组中，遍历，设置每个游戏物体的位置和旋转
    /// </summary>
    void UpdateUseRTSGOTransform()
    {
        foreach (var item in RTSInfos)
        {
            if (item.isUsePosition)
            {
                UpdateRTSGOPosition(item);
            }
            if (item.isUseRotation)
            {
                UpdateRTSGORotation(item);
            }
        }
    }

    void UpdateRTSGOPosition(RTSInfo RTSInfo)
    {
        if (eyeLock && RTSInfo.useRTSGO.name == "ForwardCamera")
        {
            RTSInfo.useRTSGO.transform.localPosition = new Vector3(0, 0, 0);
            return;
        }

        Vector3 vectorRTS = TrackerPoint.GetInstance().GetTrackerPointPostion(RTSInfo.RTSName, RTSServer);
        UpdatePos?.Invoke(RTSInfo, vectorRTS);
    }

    void UpdateRTSGORotation(RTSInfo RTSInfo)
    {
        Quaternion quaternionRTS = TrackerPoint.GetInstance().GetTrackerPointRotation(RTSInfo.RTSName, RTSServer);
        UpdateRot?.Invoke(RTSInfo, quaternionRTS);
    }

    public void UpdateInfo(InfoConfig _infoConfig)
    {
        RTSServer = _infoConfig.CameraInfo.ipServer;
        RTSInfos[0].RTSName = _infoConfig.CameraInfo.headTag;
        RTSInfos[1].RTSName = _infoConfig.CameraInfo.handTag;
    }
}
