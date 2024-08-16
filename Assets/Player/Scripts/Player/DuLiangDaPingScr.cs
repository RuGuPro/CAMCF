using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuLiangDaPingScr : MonoBehaviour, UpdataCamera
{
    public RTSManager RTSManager;
    public PerspectiveCam ForwardCameraPerspectiveCam;

    public void UpdataCameraType(InfoConfig _infoConfig)
    {
        if (_infoConfig.CameraInfo.openMotionCapture)
        {
            RTSManager.UpdatePos = (RTSInfo, vectorRTS) =>
            {
                RTSInfo.useRTSGO.transform.localPosition = new Vector3(-vectorRTS.x * _infoConfig.CameraInfo.deltaXDirection, vectorRTS.y * _infoConfig.CameraInfo.deltaYDirection, vectorRTS.z * _infoConfig.CameraInfo.deltaZDirection) + new Vector3(_infoConfig.CameraInfo.deltaX, _infoConfig.CameraInfo.deltaY + _infoConfig.CameraInfo.deltaZ);
            };
            RTSManager.UpdateRot = (RTSInfo, quaternionRTS) =>
            {
                Quaternion quaternionTemp = new Quaternion(quaternionRTS.x, -quaternionRTS.y, -quaternionRTS.z, quaternionRTS.w);
                RTSInfo.useRTSGO.transform.localEulerAngles = new Vector3(quaternionTemp.eulerAngles.x * _infoConfig.CameraInfo.deltaXRotDirection,
                                -quaternionTemp.eulerAngles.y * _infoConfig.CameraInfo.deltaYRotDirection, quaternionTemp.eulerAngles.z * _infoConfig.CameraInfo.deltaZRotDirection) + new Vector3(_infoConfig.CameraInfo.deltaRotX, _infoConfig.CameraInfo.deltaRotY + _infoConfig.CameraInfo.deltaRotZ); ;
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
        ForwardCameraPerspectiveCam.left = -_infoConfig.CameraInfo.frontSceneLong / 2;
        ForwardCameraPerspectiveCam.right = _infoConfig.CameraInfo.frontSceneLong / 2;
        ForwardCameraPerspectiveCam.top = _infoConfig.CameraInfo.allSceneHigh / 2;
        ForwardCameraPerspectiveCam.bottom = -_infoConfig.CameraInfo.allSceneHigh / 2;
        ForwardCameraPerspectiveCam.isOpen3D = _infoConfig.CameraInfo.open3D;
    }
}
