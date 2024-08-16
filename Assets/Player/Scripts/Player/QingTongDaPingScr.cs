using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QingTongDaPingScr : MonoBehaviour, UpdataCamera
{
    public CMPluginThreadManager CMPluginThreadManager;
    public BodyTracker HeadBodyTracker;
    public BodyTracker HandBodyTracker;
    public PerspectiveCam ForwardCameraPerspectiveCam;

    public void UpdataCameraType(InfoConfig _infoConfig)
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

    public void UpdataMotionCapture(InfoConfig _infoConfig)
    {
        ForwardCameraPerspectiveCam.left = -_infoConfig.CameraInfo.frontSceneLong / 2;
        ForwardCameraPerspectiveCam.right = _infoConfig.CameraInfo.frontSceneLong / 2;
        ForwardCameraPerspectiveCam.top = _infoConfig.CameraInfo.allSceneHigh / 2;
        ForwardCameraPerspectiveCam.bottom = -_infoConfig.CameraInfo.allSceneHigh / 2;
        ForwardCameraPerspectiveCam.isOpen3D = _infoConfig.CameraInfo.open3D;
    }
}
