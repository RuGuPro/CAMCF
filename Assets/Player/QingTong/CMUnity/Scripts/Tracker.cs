using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChingMU;
public class Tracker : MonoBehaviour {

    Vector3 Pos = new Vector3();
    Quaternion quat = new Quaternion();
    //CMPluginCommonInterface cm;
    private void Start()
    {
         //cm = CMPluginThreadManager.CMPlugin;
    }

    /*
    void FixedUpdate () {

        // 获取追踪体位置和旋转信息，第一个参数代表追踪系统的IP，第二个参数代表追踪体ID
        //Debug.Log(Config.Instance.CMTrackPreset.Humans[0]);

        Pos = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);
        quat = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);

        //cm.GetTrackerPose(0, out Pos, out quat);
        transform.position = Pos;
        transform.rotation = quat;
    }
    */

    void FixedUpdate()
    {
        int vrpntimecode = 0;
        double[] trackerPosition = new double[3];
        double[] trackerAttitude = new double[4];
        CMVrpn.CMTrackerExternTC(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0], ref vrpntimecode, trackerPosition, trackerAttitude);

        transform.position = new Vector3((float)trackerPosition[0] / 1000, (float)trackerPosition[2] / 1000, (float)trackerPosition[1] / 1000);
        transform.rotation = new Quaternion((float)trackerAttitude[0], (float)trackerAttitude[2], (float)trackerAttitude[1], -(float)trackerAttitude[3]);
    }

}
