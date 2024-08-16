using System;
using System.Collections;
using System.Collections.Generic;

public abstract class ConfigDataBase
{
    public abstract T ReadJson<T>(string path, Action<T> e) where T : struct;

    public abstract object ReadDateSet();
}

[Serializable]
public struct JsonInformation
{
    //是否开启动捕
    public bool openMotionCapture;
    //服务器IP
    public string ipServer;
    //端口号
    public int port;
    //动捕品牌
    public string motionCaptureBrand;
    //相机类型
    public string cameraType;
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
    //X轴镜像
    public int deltaXDirection;
    //Y轴镜像
    public int deltaYDirection;
    //Z轴镜像
    public int deltaZDirection;
    //X轴旋转镜像
    public int deltaXRotDirection;
    //Y轴旋转镜像
    public int deltaYRotDirection;
    //Z轴旋转镜像
    public int deltaZRotDirection;
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
