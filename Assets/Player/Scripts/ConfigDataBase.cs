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
    //�Ƿ�������
    public bool openMotionCapture;
    //������IP
    public string ipServer;
    //�˿ں�
    public int port;
    //����Ʒ��
    public string motionCaptureBrand;
    //�������
    public string cameraType;
    //ͷ��������ǩ
    public string headTag;
    //�ֱ�������ǩ
    public string handTag;
    //3DЧ��
    public bool open3D;
    //�������ߣ���λ���ף�
    public float allSceneHigh;
    //������������λ���ף�
    public float sideSceneLong;
    //������������λ���ף�
    public float frontSceneLong;
    //����������λ���ף�
    public float groundSceneWide;
    //X�᾵��
    public int deltaXDirection;
    //Y�᾵��
    public int deltaYDirection;
    //Z�᾵��
    public int deltaZDirection;
    //X����ת����
    public int deltaXRotDirection;
    //Y����ת����
    public int deltaYRotDirection;
    //Z����ת����
    public int deltaZRotDirection;
    //X������
    public float deltaX;
    //Y������
    public float deltaY;
    //Z������
    public float deltaZ;
    //X����ת����
    public float deltaRotX;
    //Y����ת����
    public float deltaRotY;
    //Z����ת����
    public float deltaRotZ;
}
