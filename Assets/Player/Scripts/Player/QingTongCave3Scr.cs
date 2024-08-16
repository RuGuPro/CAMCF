using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QingTongCave3Scr : MonoBehaviour, UpdataCamera
{
    public CMPluginThreadManager CMPluginThreadManager;
    public BodyTracker HeadBodyTracker;
    public BodyTracker HandBodyTracker;
    public CamManagerScr CamManagerScr;

    public void UpdataCameraType(InfoConfig _infoConfig)
    {
        if (_infoConfig.CameraInfo.openMotionCapture)
        {
            HeadBodyTracker.bodyId = int.Parse(_infoConfig.CameraInfo.headTag);
            HandBodyTracker.bodyId = int.Parse(_infoConfig.CameraInfo.handTag);

            HeadBodyTracker.UpdatePos = (transformObj, vectorRTS) =>
            {
                transformObj.localPosition = new Vector3(vectorRTS.x * _infoConfig.CameraInfo.deltaXDirection, vectorRTS.y * _infoConfig.CameraInfo.deltaYDirection, vectorRTS.z * _infoConfig.CameraInfo.deltaZDirection) + new Vector3(_infoConfig.CameraInfo.deltaX, _infoConfig.CameraInfo.deltaY, _infoConfig.CameraInfo.deltaZ);
            };
            HeadBodyTracker.UpdateRot = (transformObj, quaternionRTS) =>
            {
                Quaternion quaternionTemp = quaternionRTS;
                transformObj.localEulerAngles = new Vector3(quaternionTemp.eulerAngles.x * _infoConfig.CameraInfo.deltaXRotDirection,
                                quaternionTemp.eulerAngles.y * _infoConfig.CameraInfo.deltaYRotDirection, quaternionTemp.eulerAngles.z * _infoConfig.CameraInfo.deltaZRotDirection) + new Vector3(_infoConfig.CameraInfo.deltaRotX, _infoConfig.CameraInfo.deltaRotY, _infoConfig.CameraInfo.deltaRotZ);
            };  

            CMPluginThreadManager.UpdateInfo(_infoConfig);
        }
        else
        {
            CMPluginThreadManager.enabled = false;
            HeadBodyTracker.enabled = false;
            HandBodyTracker.enabled = false;
        }
    }

    public void UpdataMotionCapture(InfoConfig _infoConfig)
    {
        CamManagerScr.UpdateInfo(_infoConfig);
    }
}
