using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChingMU;

public class CMPluginThreadManager : MonoBehaviour
{
    // Start is called before the first frame update


    public CMPluginAPI.CMPluginType cMPluginType;
    public string ServerIP;
    public int port;

    [HideInInspector]
    public static CMPluginCommonInterface CMPlugin;

    [HideInInspector]
    public static bool IsConnected;

    private void OnDestroy()
    {
        CMPlugin.QuitCmThread();
    }

    public void UpdateInfo(InfoConfig _infoConfig)
    {
        ServerIP = "MCServer@" + _infoConfig.CameraInfo.ipServer;
        port = _infoConfig.CameraInfo.port;

        if (cMPluginType == CMPluginAPI.CMPluginType.LiveStream)
        {
            CMPlugin = new LiveStreamImpl();
            CMPlugin.ServerIp = ServerIP;
            CMPlugin.Port = port;
            CMPlugin.StartCmThread();
            //IsConnected = CMPlugin.ConnectCmServer(ServerIP, port);
        }
        else if (cMPluginType == CMPluginAPI.CMPluginType.Vrpn)
        {
            CMPlugin = new VrpnImpl();
            CMVrpn.CMUnityEnableTrackLog(false);
            CMPlugin.ServerIp = ServerIP;
            CMPlugin.Port = port;
            CMPlugin.StartCmThread();
            //IsConnected = CMPlugin.ConnectCmServer(ServerIP, port);
        }
        else
        {
            CMPlugin = null;
            //IsConnected = false;
        }

        IsConnected = CMPlugin.ConnectCmServer();
    }
}
