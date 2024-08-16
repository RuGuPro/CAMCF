using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManagerScr : MonoBehaviour
{
    public float frontSceneLong = 3.84f;
    public float sideSceneLong = 3.2f;
    public float allSceneHigh = 2.56f;
    public float groundSceneWide = 0;
    private float PianYiNear = 0.1f;

    public PerspectiveCam ForwardCam;
    public PerspectiveCam LeftCam;
    public PerspectiveCam RightCam;
    public PerspectiveCam GroundCam;

    private Vector3 pos = new Vector3();
    public bool eyeLock = false;

    void Update()
    {
        if (eyeLock)
        {
            //前摄像机值
            ForwardCam.GetComponent<Camera>().nearClipPlane = (sideSceneLong / 2) / 10;
            ForwardCam.left = (-(frontSceneLong / 2)) / 10;
            ForwardCam.right = (frontSceneLong / 2) / 10;
            ForwardCam.top = (allSceneHigh / 2) / 10;
            ForwardCam.bottom = (-(allSceneHigh / 2)) / 10;
            //左摄像机
            LeftCam.GetComponent<Camera>().nearClipPlane = (frontSceneLong / 2) / 10;
            LeftCam.left = (-(sideSceneLong / 2)) / 10;
            LeftCam.right = (sideSceneLong / 2) / 10;
            LeftCam.top = (allSceneHigh / 2) / 10;
            LeftCam.bottom = (-(allSceneHigh / 2)) / 10;
            //右摄像机
            RightCam.GetComponent<Camera>().nearClipPlane = (frontSceneLong / 2) / 10;
            RightCam.left = (-(sideSceneLong / 2)) / 10;
            RightCam.right = (sideSceneLong / 2) / 10;
            RightCam.top = (allSceneHigh / 2) / 10;
            RightCam.bottom = (-(allSceneHigh / 2)) / 10;
            //地摄像机
            if (GroundCam)
            {
                GroundCam.GetComponent<Camera>().nearClipPlane = (allSceneHigh / 2) / 10;
                GroundCam.left = (-(frontSceneLong / 2)) / 10;
                GroundCam.right = (frontSceneLong / 2) / 10;
                GroundCam.top = (groundSceneWide / 2 + (sideSceneLong - groundSceneWide) / 2) / 10;
                GroundCam.bottom = (-(groundSceneWide / 2 - (sideSceneLong - groundSceneWide) / 2)) / 10;
            }
        }
        else
        {
            pos = ForwardCam.transform.localPosition;
            //前摄像机值
            ForwardCam.GetComponent<Camera>().nearClipPlane = (sideSceneLong / 2 - pos.z) / 10;
            ForwardCam.left = (-(frontSceneLong / 2 + pos.x)) / 10;
            ForwardCam.right = (frontSceneLong / 2 - pos.x) / 10;
            ForwardCam.top = (allSceneHigh / 2 - pos.y + (allSceneHigh / 2) + PianYiNear) / 10;
            ForwardCam.bottom = (-(allSceneHigh / 2 + pos.y) + (allSceneHigh / 2) + PianYiNear) / 10;
            //左摄像机
            LeftCam.GetComponent<Camera>().nearClipPlane = (frontSceneLong / 2 + pos.x) / 10;
            LeftCam.left = (-(sideSceneLong / 2 + pos.z)) / 10;
            LeftCam.right = (sideSceneLong / 2 - pos.z) / 10;
            LeftCam.top = (allSceneHigh / 2 - pos.y + (allSceneHigh / 2) + PianYiNear) / 10;
            LeftCam.bottom = (-(allSceneHigh / 2 + pos.y) + (allSceneHigh / 2) + PianYiNear) / 10;
            //右摄像机
            RightCam.GetComponent<Camera>().nearClipPlane = (frontSceneLong / 2 - pos.x) / 10;
            RightCam.left = (-(sideSceneLong / 2 - pos.z)) / 10;
            RightCam.right = (sideSceneLong / 2 + pos.z) / 10;
            RightCam.top = (allSceneHigh / 2 - pos.y + (allSceneHigh / 2) + PianYiNear) / 10;
            RightCam.bottom = (-(allSceneHigh / 2 + pos.y) + (allSceneHigh / 2) + PianYiNear) / 10;
            //地摄像机
            if (GroundCam)
            {
                float nearGround = 0;
                if ((pos.y - PianYiNear) / 10 < 0.01)
                {
                    nearGround = 0.01f;
                }
                else
                {
                    nearGround = (pos.y - PianYiNear) / 10;
                }
                GroundCam.GetComponent<Camera>().nearClipPlane = nearGround;
                GroundCam.left = (-(frontSceneLong / 2 + pos.x)) / 10;
                GroundCam.right = (frontSceneLong / 2 - pos.x) / 10;
                GroundCam.top = (groundSceneWide / 2 - pos.z + (sideSceneLong - groundSceneWide) / 2) / 10;
                GroundCam.bottom = (-(groundSceneWide / 2 + pos.z - (sideSceneLong - groundSceneWide) / 2)) / 10;
            }
        }
    }

    public void UpdateInfo(InfoConfig _infoConfig)
    {
        frontSceneLong = _infoConfig.CameraInfo.frontSceneLong;
        sideSceneLong = _infoConfig.CameraInfo.sideSceneLong;
        allSceneHigh = _infoConfig.CameraInfo.allSceneHigh;
        groundSceneWide = _infoConfig.CameraInfo.groundSceneWide;
        ForwardCam.isOpen3D = _infoConfig.CameraInfo.open3D;
        LeftCam.isOpen3D = _infoConfig.CameraInfo.open3D;
        RightCam.isOpen3D = _infoConfig.CameraInfo.open3D;
        if (GroundCam)
        {
            GroundCam.isOpen3D = _infoConfig.CameraInfo.open3D;
        }

        //屏幕分布
        float allLong = sideSceneLong * 2 + frontSceneLong;
        if (GroundCam)
        {
            allLong += frontSceneLong;
        }

        float sideScenePer = Mathf.Round((sideSceneLong / allLong) * 10000) / 10000;
        float frontScenePer = Mathf.Round((frontSceneLong / allLong) * 10000) / 10000;

        if (_infoConfig.CameraInfo.open3D)
        {
            ForwardCam.GetComponent<Camera>().rect = new Rect(sideScenePer, 0, frontScenePer - 0.00001f, 1);
            LeftCam.GetComponent<Camera>().rect = new Rect(0, 0, sideScenePer, 1);
            RightCam.GetComponent<Camera>().rect = new Rect((sideScenePer + frontScenePer), 0, sideScenePer, 1);
            if (GroundCam)
            {
                GroundCam.GetComponent<Camera>().rect = new Rect((sideScenePer + frontScenePer + sideScenePer), 0, frontScenePer, 1);
            }
        }
        else
        {
            ForwardCam.GetComponent<Camera>().rect = new Rect(sideScenePer, 0, frontScenePer, 1);
            LeftCam.GetComponent<Camera>().rect = new Rect(0, 0, sideScenePer, 1);
            RightCam.GetComponent<Camera>().rect = new Rect((sideScenePer + frontScenePer), 0, sideScenePer, 1);
            if (GroundCam)
            {
                GroundCam.GetComponent<Camera>().rect = new Rect((sideScenePer + frontScenePer + sideScenePer), 0, frontScenePer, 1);
            }
        }
    }
}
