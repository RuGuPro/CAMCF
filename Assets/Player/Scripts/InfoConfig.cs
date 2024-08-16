using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更新数据
/// </summary>
public interface UpdataCamera
{
    void UpdataMotionCapture(InfoConfig _infoConfig);
    void UpdataCameraType(InfoConfig _infoConfig);
}

/// <summary>
/// 动捕品牌
/// </summary>
public enum MotionCaptureBrand
{
    青瞳,
    瑞立视,
    度量
}

/// <summary>
/// 相机类型
/// </summary>
public enum CameraType
{
    大屏,
    三面屏,
    四面屏
}

/// <summary>
/// 相机参数
/// </summary>
[System.Serializable]
public struct CameraInfo
{
    //是否开启动捕
    public bool openMotionCapture;
    //服务器IP
    public string ipServer;
    //端口号
    public int port;
    //动捕品牌
    public MotionCaptureBrand motionCaptureBrand;
    //相机类型
    public CameraType cameraType;
    //头部动捕标签
    public string headTag;
    //手柄动捕标签
    public string handTag;
    //3D效果
    public bool open3D;
    //整体屏高（单位：米）
    public float allSceneHigh;
    //侧面屏长（单位：米）
    public float sideSceneLong;
    //正面屏长（单位：米）
    public float frontSceneLong;
    //地面屏宽（单位：米）
    public float groundSceneWide;
    //X轴增量
    public float deltaX;
    //Y轴增量
    public float deltaY;
    //Z轴增量
    public float deltaZ;
    //X轴旋转增量
    public float deltaRotX;
    //Y轴旋转增量
    public float deltaRotY;
    //Z轴旋转增量
    public float deltaRotZ;
}

[CreateAssetMenu(fileName = "InfoConfig", menuName = "CreatInfoConfig", order = 0)]

public class InfoConfig : ScriptableObject
{
    public CameraInfo CameraInfo;
}
