using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QingTongCave4Scr : MonoBehaviour, UpdataCamera
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
                transformObj.localPosition = new Vector3(vectorRTS.x + _infoConfig.CameraInfo.deltaX, vectorRTS.y + _infoConfig.CameraInfo.deltaY, vectorRTS.z + _infoConfig.CameraInfo.deltaZ);
            };
            HeadBodyTracker.UpdateRot = (transformObj, quaternionRTS) =>
            {
                Quaternion quaternionTemp = quaternionRTS;
                transformObj.localEulerAngles = new Vector3(quaternionTemp.eulerAngles.x + _infoConfig.CameraInfo.deltaRotX,
                                quaternionTemp.eulerAngles.y + _infoConfig.CameraInfo.deltaRotY, quaternionTemp.eulerAngles.z + _infoConfig.CameraInfo.deltaRotZ);
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
