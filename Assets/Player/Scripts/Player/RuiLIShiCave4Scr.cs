using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuiLIShiCave4Scr : MonoBehaviour, UpdataCamera
{
    public RTSManager RTSManager;
    public CamManagerScr CamManagerScr;

    public void UpdataCameraType(InfoConfig _infoConfig)
    {
        if (_infoConfig.CameraInfo.openMotionCapture)
        {
            RTSManager.UpdatePos = (RTSInfo, vectorRTS) =>
            {
                RTSInfo.useRTSGO.transform.localPosition = new Vector3(vectorRTS.y + _infoConfig.CameraInfo.deltaX, vectorRTS.z + _infoConfig.CameraInfo.deltaY, -vectorRTS.x + _infoConfig.CameraInfo.deltaZ);
            };
            RTSManager.UpdateRot = (RTSInfo, quaternionRTS) =>
            {
                Quaternion quaternionTemp = new Quaternion(quaternionRTS.x, -quaternionRTS.y, -quaternionRTS.z, quaternionRTS.w);
                RTSInfo.useRTSGO.transform.localEulerAngles = new Vector3(quaternionTemp.eulerAngles.x + _infoConfig.CameraInfo.deltaRotX,
                    quaternionTemp.eulerAngles.y + _infoConfig.CameraInfo.deltaRotY, quaternionTemp.eulerAngles.z + _infoConfig.CameraInfo.deltaRotZ);
            };
            RTSManager.UpdateInfo(_infoConfig);
        }
        else
        {
            RTSManager.enabled = false;
        }
    }

    public void UpdataMotionCapture(InfoConfig _infoConfig)
    {
        CamManagerScr.UpdateInfo(_infoConfig);
    }
}

