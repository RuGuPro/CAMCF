using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HumanTracker : MonoBehaviour
{
    [Header("ChingMUTrackerSeting")]
    public int HumanID;
    public ChingMU_HumanBones HumanBones;
    Dictionary<int, Transform> BonesIndexMapToTransform;

    CMVrpn.VrpnHumanMap CurHumanMapResult;
    CMVrpn.VrpnHumanMap CurHumanMap;
    CMVrpn.UpdateHumanMapCallback GetCurHumanMapCallback;
    IntPtr callback_args;

    [HideInInspector]
    Vector3 pos = Vector3.zero;
    Quaternion[] rot = new Quaternion[150];
    //bool[] isSegmentDetected = new bool[23];
    Quaternion[] TPoseStateJoints = new Quaternion[150];
    Quaternion[] TPoseStateParentJoints = new Quaternion[150];
    CMPluginCommonInterface CMPlugin;
    [HideInInspector]
    public string ServerID = string.Empty;
    bool IsRegisterCallBack_Finished = false;
    // public int HumanID;

    

    void Start()
    {
        if (CMPluginThreadManager.CMPlugin != null)
        {
            CMPlugin = CMPluginThreadManager.CMPlugin;
        }
        BonesIndexMapToTransform = new Dictionary<int, Transform>();
        CreateBonesIndexMap(HumanBones);
        SetModelArmsAndLegsInTpose();

        getTPoseStatePreJoints_trans();
        getTPoseStatePreParentJoints_trans();

        CurHumanMap = new CMVrpn.VrpnHumanMap();
        CurHumanMapResult = new CMVrpn.VrpnHumanMap();

        callback_args = Marshal.AllocHGlobal(Marshal.SizeOf(CurHumanMap));
        GetCurHumanMapCallback = GetHumanMap;
        StartCoroutine(AsyRegisterCallBack_GetVrpnDataOrder());
    }

    bool RegisterCallback_IsFinished()
    {
        bool IsFinished = CMVrpn.CMPluginRegisterHumanMap(CMPlugin.ServerIp+":"+CMPlugin.Port, callback_args, GetCurHumanMapCallback);
        return !IsFinished;
    }
    IEnumerator AsyRegisterCallBack_GetVrpnDataOrder()
    {
        yield return new WaitWhile(RegisterCallback_IsFinished);
        IsRegisterCallBack_Finished = true;
    }
    void GetHumanMap(IntPtr CallBackFun_agrs, CMVrpn.VrpnHumanMap CurHumanMap)
    {
        CurHumanMapResult = CurHumanMap;
        Debug.Log("Human name:" + CurHumanMapResult.name + " "+ "Human id:" + CurHumanMapResult.sensor);
    }

    void FixedUpdate()
    {
        //ServerIP：格式为”服务器名@主机名”。列如 "MCAvatar@Sh1dt010"，其中主机名可以用主机的IP替换，如 "MCAvatar@192.168.3.178"
        //HumanID：int型，等于0，表示0号人物的动捕数据
        //pos：根骨骼关节Hip的世界空间下的位置信息；
        //rot：长度为23的Quaternion[]数组，表示相对于父关节的localrotation相对旋转信息。

        //bool IsTrackedHuman = CMVrpn.CMHumanAttitude(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Humans[0], out pos, rot, isSegmentDetected);

        bool IsInit =CMPlugin == null ? false : true;
        if (IsInit) 
        {
            //bool IsTrackedHuman = CMPlugin.GetHumanWithoutRetargetPose(HumanID, out pos, rot);

            //if (CMPlugin.cMserverType == ChingMU.CMPluginAPI.CMServerType.MCAvatar)
            //{
            //    if (HumanID < 300)
            //    {
            //        HumanID += 300;
            //    }
            //}

            bool IsTrackedHuman = CMPlugin.GetHumanWithoutRetargetPose(HumanID, out pos, rot);

            //bool IsTrackedHuman = CMVrpn.CMHumanAttitude(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Humans[0], out pos, rot, isSegmentDetected);

            if (IsTrackedHuman)
            {
                //BonesIndexMapToTransform[0].position = pos;

                //模拟UE4组件空间旋转
                BonesIndexMapToTransform[0].localPosition = pos;
                for (int i = 0; i < 63; i++)
                {
                    if (BonesIndexMapToTransform[i] == null)
                    {
                        continue;
                    }

                    if (rot[i]!=Quaternion.identity)
                    {
                        if(BonesIndexMapToTransform[i].ToString().Contains("Hand"))
                        {
                            Debug.Log(BonesIndexMapToTransform[i] + "||" + BonesIndexMapToTransform[i].localRotation);
                        }
                        
                        BonesIndexMapToTransform[i].localRotation = Quaternion.Inverse(TPoseStateParentJoints[i]) * rot[i] * TPoseStateJoints[i];
                        //将局部旋转转为世界旋转 - way1；
                        //BonesIndexMapToTransform[i].rotation = BonesIndexMapToTransform[i].parent.rotation * Quaternion.Inverse(TPoseStateParentJoints[i]) * rot[i] * TPoseStateJoints[i];
                        //将局部旋转转为世界旋转 - way2；
                        #region
                        /*
                        //hips
                        human[0].rotation = rot[0] * TPoseStateJoints[0];
                        // spine
                        human[1].rotation = human[0].rotation * Quaternion.Inverse(TPoseStateParentJoints[1]) * rot[1] * TPoseStateJoints[1];
                        // spine1
                        human[2].rotation = human[1].rotation * Quaternion.Inverse(TPoseStateParentJoints[2]) * rot[2] * TPoseStateJoints[2];
                        // spine2
                        human[3].rotation = human[2].rotation * Quaternion.Inverse(TPoseStateParentJoints[3]) * rot[3] * TPoseStateJoints[3];
                        // spine3
                        human[4].rotation = human[3].rotation * Quaternion.Inverse(TPoseStateParentJoints[4]) * rot[4] * TPoseStateJoints[4];
                        // neck
                        human[5].rotation = human[4].rotation * Quaternion.Inverse(TPoseStateParentJoints[5]) * rot[5] * TPoseStateJoints[5];
                        // head
                        human[6].rotation = human[5].rotation * Quaternion.Inverse(TPoseStateParentJoints[6]) * rot[6] * TPoseStateJoints[6];
                        // left shoulder
                        human[7].rotation = human[4].rotation * Quaternion.Inverse(TPoseStateParentJoints[7]) * rot[7] * TPoseStateJoints[7];
                        // left arm
                        human[8].rotation = human[7].rotation * Quaternion.Inverse(TPoseStateParentJoints[8]) * rot[8] * TPoseStateJoints[8];
                        // left fore arm
                        human[9].rotation = human[8].rotation * Quaternion.Inverse(TPoseStateParentJoints[9]) * rot[9] * TPoseStateJoints[9];
                        // left hand
                        human[10].rotation = human[9].rotation * Quaternion.Inverse(TPoseStateParentJoints[10]) * rot[10] * TPoseStateJoints[10];
                        // right shoulder
                        human[11].rotation = human[4].rotation * Quaternion.Inverse(TPoseStateParentJoints[11]) * rot[11] * TPoseStateJoints[11];
                        // right arm
                        human[12].rotation = human[11].rotation * Quaternion.Inverse(TPoseStateParentJoints[12]) * rot[12] * TPoseStateJoints[12];
                        // right fore arm
                        human[13].rotation = human[12].rotation * Quaternion.Inverse(TPoseStateParentJoints[13]) * rot[13] * TPoseStateJoints[13];
                        // right hand
                        human[14].rotation = human[13].rotation * Quaternion.Inverse(TPoseStateParentJoints[14]) * rot[14] * TPoseStateJoints[14];
                        // left up leg
                        human[15].rotation = human[0].rotation * Quaternion.Inverse(TPoseStateParentJoints[15]) * rot[15] * TPoseStateJoints[15];
                        // left leg
                        human[16].rotation = human[15].rotation * Quaternion.Inverse(TPoseStateParentJoints[16]) * rot[16] * TPoseStateJoints[16];
                        // left foot
                        human[17].rotation = human[16].rotation * Quaternion.Inverse(TPoseStateParentJoints[17]) * rot[17] * TPoseStateJoints[17];
                        // left toe
                        human[18].rotation = human[17].rotation * Quaternion.Inverse(TPoseStateParentJoints[18]) * rot[18] * TPoseStateJoints[18];
                        // right up leg
                        human[19].rotation = human[0].rotation * Quaternion.Inverse(TPoseStateParentJoints[19]) * rot[19] * TPoseStateJoints[19];
                        // right leg
                        human[20].rotation = human[19].rotation * Quaternion.Inverse(TPoseStateParentJoints[20]) * rot[20] * TPoseStateJoints[20];
                        // right foot
                        human[21].rotation = human[20].rotation * Quaternion.Inverse(TPoseStateParentJoints[21]) * rot[21] * TPoseStateJoints[21];
                        // right toe
                        human[22].rotation = human[21].rotation * Quaternion.Inverse(TPoseStateParentJoints[22]) * rot[22] * TPoseStateJoints[22];
                        */
                        #endregion
                    }
                }
            }

        }

    }


    void CreateBonesIndexMap(ChingMU_HumanBones HumanBones)
    {
        BonesIndexMapToTransform.Add(0, HumanBones.Hips);
        BonesIndexMapToTransform.Add(1, HumanBones.Spine1);
        BonesIndexMapToTransform.Add(2, HumanBones.Spine2);
        BonesIndexMapToTransform.Add(3, HumanBones.Spine3);
        BonesIndexMapToTransform.Add(4, HumanBones.Spine4);
        BonesIndexMapToTransform.Add(5, HumanBones.Neck);
        BonesIndexMapToTransform.Add(6, HumanBones.Head);
        BonesIndexMapToTransform.Add(7, HumanBones.LeftShoulder);
        BonesIndexMapToTransform.Add(8, HumanBones.LeftArm);
        BonesIndexMapToTransform.Add(9, HumanBones.LeftForeArm);
        BonesIndexMapToTransform.Add(10, HumanBones.LeftHand);
        BonesIndexMapToTransform.Add(11, HumanBones.RightShoulder);
        BonesIndexMapToTransform.Add(12, HumanBones.RightArm);
        BonesIndexMapToTransform.Add(13, HumanBones.RightForeArm);
        BonesIndexMapToTransform.Add(14, HumanBones.RightHand);
        BonesIndexMapToTransform.Add(15, HumanBones.LeftUpleg);
        BonesIndexMapToTransform.Add(16, HumanBones.LeftLeg);
        BonesIndexMapToTransform.Add(17, HumanBones.LeftFoot);
        BonesIndexMapToTransform.Add(18, HumanBones.LeftToeBase);
        BonesIndexMapToTransform.Add(19, HumanBones.RightUpLeg);
        BonesIndexMapToTransform.Add(20, HumanBones.RightLeg);
        BonesIndexMapToTransform.Add(21, HumanBones.RightFoot);
        BonesIndexMapToTransform.Add(22, HumanBones.RightToeBase);

        BonesIndexMapToTransform.Add(23, HumanBones.LeftHandThumb1);
        BonesIndexMapToTransform.Add(24, HumanBones.LeftHandThumb2);
        BonesIndexMapToTransform.Add(25, HumanBones.LeftHandThumb3);
        BonesIndexMapToTransform.Add(26, HumanBones.LeftHandThumb4);
        BonesIndexMapToTransform.Add(27, HumanBones.LeftHandIndex1);
        BonesIndexMapToTransform.Add(28, HumanBones.LeftHandIndex2);
        BonesIndexMapToTransform.Add(29, HumanBones.LeftHandIndex3);
        BonesIndexMapToTransform.Add(30, HumanBones.LeftHandIndex4);
        BonesIndexMapToTransform.Add(31, HumanBones.LeftHandMiddle1);
        BonesIndexMapToTransform.Add(32, HumanBones.LeftHandMiddle2);
        BonesIndexMapToTransform.Add(33, HumanBones.LeftHandMiddle3);
        BonesIndexMapToTransform.Add(34, HumanBones.LeftHandMiddle4);
        BonesIndexMapToTransform.Add(35, HumanBones.LeftHandRing1);
        BonesIndexMapToTransform.Add(36, HumanBones.LeftHandRing2);
        BonesIndexMapToTransform.Add(37, HumanBones.LeftHandRing3);
        BonesIndexMapToTransform.Add(38, HumanBones.LeftHandRing4);
        BonesIndexMapToTransform.Add(39, HumanBones.LeftHandPinky1);
        BonesIndexMapToTransform.Add(40, HumanBones.LeftHandPinky2);
        BonesIndexMapToTransform.Add(41, HumanBones.LeftHandPinky3);
        BonesIndexMapToTransform.Add(42, HumanBones.LeftHandPinky4);

        BonesIndexMapToTransform.Add(43, HumanBones.RightHandThumb1);
        BonesIndexMapToTransform.Add(44, HumanBones.RightHandThumb2);
        BonesIndexMapToTransform.Add(45, HumanBones.RightHandThumb3);
        BonesIndexMapToTransform.Add(46, HumanBones.RightHandThumb4);
        BonesIndexMapToTransform.Add(47, HumanBones.RightHandIndex1);
        BonesIndexMapToTransform.Add(48, HumanBones.RightHandIndex2);
        BonesIndexMapToTransform.Add(49, HumanBones.RightHandIndex3);
        BonesIndexMapToTransform.Add(50, HumanBones.RightHandIndex4);
        BonesIndexMapToTransform.Add(51, HumanBones.RightHandMiddle1);
        BonesIndexMapToTransform.Add(52, HumanBones.RightHandMiddle2);
        BonesIndexMapToTransform.Add(53, HumanBones.RightHandMiddle3);
        BonesIndexMapToTransform.Add(54, HumanBones.RightHandMiddle4);
        BonesIndexMapToTransform.Add(55, HumanBones.RightHandRing1);
        BonesIndexMapToTransform.Add(56, HumanBones.RightHandRing2);
        BonesIndexMapToTransform.Add(57, HumanBones.RightHandRing3);
        BonesIndexMapToTransform.Add(58, HumanBones.RightHandRing4);
        BonesIndexMapToTransform.Add(59, HumanBones.RightHandPinky1);
        BonesIndexMapToTransform.Add(60, HumanBones.RightHandPinky2);
        BonesIndexMapToTransform.Add(61, HumanBones.RightHandPinky3);
        BonesIndexMapToTransform.Add(62, HumanBones.RightHandPinky4);
    }


    void getTPoseStatePreJoints_trans()
    {

        for (int i = 0; i < 63; i++)
        {
            if (BonesIndexMapToTransform[i] == null)
            {
                continue;
            }
            TPoseStateJoints[i] = BonesIndexMapToTransform[i].rotation;
        }
    }

    void getTPoseStatePreParentJoints_trans()
    {
        for (int i = 0; i < 63; i++)
        {
            if (BonesIndexMapToTransform[i] == null)
            {
                continue;
            }

            TPoseStateParentJoints[i] = BonesIndexMapToTransform[i].parent.rotation;

        }
    }

    void SetModelArmsAndLegsInTpose()
    {
        Vector3 vTposeLeftDir = transform.TransformDirection(Vector3.left);
        Vector3 vTposeRightDir = transform.TransformDirection(Vector3.right);
        Vector3 vTposeDownDir = transform.TransformDirection(Vector3.down);

        Transform transLeftUparm = BonesIndexMapToTransform[8]; // animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform transLeftLowerarm = BonesIndexMapToTransform[9]; // animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform transLeftHand = BonesIndexMapToTransform[10];
        if (transLeftUparm != null && transLeftLowerarm != null)
        {
            Vector3 vLeftUparmDir = transLeftLowerarm.position - transLeftUparm.position;
            float fLeftUparmAngle = Vector3.Angle(vLeftUparmDir, vTposeLeftDir);

            if (Mathf.Abs(fLeftUparmAngle) >= 5f)
            {
                Quaternion vFixRotation = Quaternion.FromToRotation(vLeftUparmDir, vTposeLeftDir);
                transLeftUparm.rotation = vFixRotation * transLeftUparm.rotation;
            }

            if (transLeftHand != null)
            {
                Vector3 vLeftLowerarmDir = transLeftHand.position - transLeftLowerarm.position;
                float fLeftLowerarmAngle = Vector3.Angle(vLeftLowerarmDir, vTposeLeftDir);

                if (Mathf.Abs(fLeftLowerarmAngle) >= 5f)
                {
                    Quaternion vFixRotation = Quaternion.FromToRotation(vLeftLowerarmDir, vTposeLeftDir);
                    transLeftLowerarm.rotation = vFixRotation * transLeftLowerarm.rotation;
                }
            }
        }

        Transform transRightUparm = BonesIndexMapToTransform[12]; // animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Transform transRightLowerarm = BonesIndexMapToTransform[13]; // animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        Transform transRightHand = BonesIndexMapToTransform[14];

        if (transRightUparm != null && transRightLowerarm != null)
        {
            Vector3 vRightUparmDir = transRightLowerarm.position - transRightUparm.position;
            float fRightUparmAngle = Vector3.Angle(vRightUparmDir, vTposeRightDir);

            if (Mathf.Abs(fRightUparmAngle) >= 5f)
            {
                Quaternion vFixRotation = Quaternion.FromToRotation(vRightUparmDir, vTposeRightDir);
                transRightUparm.rotation = vFixRotation * transRightUparm.rotation;
            }

            if (transRightHand != null)
            {
                Vector3 vRightLowerarmDir = transRightHand.position - transRightLowerarm.position;
                float fRightLowerarmAngle = Vector3.Angle(vRightLowerarmDir, vTposeRightDir);

                if (Mathf.Abs(fRightLowerarmAngle) >= 5f)
                {
                    Quaternion vFixRotation = Quaternion.FromToRotation(vRightLowerarmDir, vTposeRightDir);
                    transRightLowerarm.rotation = vFixRotation * transRightLowerarm.rotation;
                }
            }
        }

        Transform transLeftUpleg = BonesIndexMapToTransform[15]; // animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform transLeftLowerleg = BonesIndexMapToTransform[16]; // animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform transLeftFoot = BonesIndexMapToTransform[17];
        if (transLeftUpleg != null && transLeftLowerleg != null)
        {
            Vector3 vLeftUplegDir = transLeftLowerleg.position - transLeftUpleg.position;
            float fLeftUplegAngle = Vector3.Angle(vLeftUplegDir, vTposeDownDir);

            if (Mathf.Abs(fLeftUplegAngle) >= 5f)
            {
                Quaternion vFixRotation = Quaternion.FromToRotation(vLeftUplegDir, vTposeDownDir);
                transLeftUpleg.rotation = vFixRotation * transLeftUpleg.rotation;
            }

            if (transLeftFoot != null)
            {
                Vector3 vLeftLowerlegDir = transLeftFoot.position - transLeftLowerleg.position;
                float fLowerlegLeftAngle = Vector3.Angle(vLeftLowerlegDir, vTposeDownDir);

                if (Mathf.Abs(fLowerlegLeftAngle) >= 5f)
                {
                    Quaternion vFixRotation = Quaternion.FromToRotation(vLeftLowerlegDir, vTposeDownDir);
                    transLeftLowerleg.rotation = vFixRotation * transLeftLowerleg.rotation;
                }
            }
        }

        Transform transRightUpleg = BonesIndexMapToTransform[19]; // animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform transRightLowerleg = BonesIndexMapToTransform[20]; // animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform transRightFoot = BonesIndexMapToTransform[21];
        if (transRightUpleg != null && transRightLowerleg != null)
        {
            Vector3 vRightUplegDir = transRightLowerleg.position - transRightUpleg.position;
            float fRightUplegAngle = Vector3.Angle(vRightUplegDir, vTposeDownDir);

            if (Mathf.Abs(fRightUplegAngle) >= 5f)
            {
                Quaternion vFixRotation = Quaternion.FromToRotation(vRightUplegDir, vTposeDownDir);
                transRightUpleg.rotation = vFixRotation * transRightUpleg.rotation;
            }

            if (transRightFoot != null)
            {
                Vector3 vRightLowerlegDir = transRightFoot.position - transRightLowerleg.position;
                float fLeftLowerlegAngle = Vector3.Angle(vRightLowerlegDir, vTposeDownDir);

                if (Mathf.Abs(fLeftLowerlegAngle) >= 5f)
                {
                    Quaternion vFixRotation = Quaternion.FromToRotation(vRightLowerlegDir, vTposeDownDir);
                    transRightLowerleg.rotation = vFixRotation * transRightLowerleg.rotation;
                }
            }
        }
    }

    string GetServerType(string mcServerName)
    {

        return "";
    }
}
