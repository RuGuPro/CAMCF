using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTracker : MonoBehaviour
{
    Vector3 wPos = new Vector3();
    Quaternion wQuat = new Quaternion();

    CMPluginCommonInterface CMPlugin;

    [Header("ChingMUTrackerSeting")]
    public int bodyId;

    string _address;

    public bool isLockRot = false;
    public bool eyeLock = false;

    public Action<Transform, Vector3> UpdatePos;
    public Action<Transform, Quaternion> UpdateRot;


    private void Start()
    {
        if (CMPluginThreadManager.CMPlugin != null) 
        {
            CMPlugin = CMPluginThreadManager.CMPlugin;
            _address = CMPlugin.ServerIp + ":" + CMPlugin.Port;
        }
    }
    void FixedUpdate()
    {

        // 获取追踪体位置和旋转信息，第一个参数代表追踪系统的IP，第二个参数代表追踪体ID
        bool IsInit = CMPlugin == null ? false : true;
        if (IsInit) 
        {
            CMPluginThreadManager.CMPlugin.GetTrackerPose(bodyId, out wPos, out wQuat);
        }


        //Pos = CMUnity.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);
        //quat = CMUnity.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);

        if (!eyeLock)
        {
            UpdatePos?.Invoke(this.transform, CMVrpn.CMPos(_address, bodyId));
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }

        if (!isLockRot)
        {
            UpdateRot?.Invoke(this.transform, CMVrpn.CMQuat(_address, bodyId));
            transform.localRotation = CMVrpn.CMQuat(_address, bodyId);
        }
    }
}
