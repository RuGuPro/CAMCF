using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using System.Text;
using System;
using Unity.Plastic.Newtonsoft.Json.Linq;

public class CaveEditor : ScriptableWizard
{
    private string jsonFilePath = Application.streamingAssetsPath + "/Data/Jsondata.json";
    private const string INFO_CONFIG_PATH = "Assets/Player/Scripts/InfoConfig.asset";
    private InfoConfig infoConfig;
    private CameraInfo CameraInfo;
    private GameObject CreatPlayerManager;
    private int headTag = 0;
    private int handTag = 1;
    private bool deltaXDirection = false;
    private bool deltaYDirection = false;
    private bool deltaZDirection = false;
    private bool deltaXRotDirection = false;
    private bool deltaYRotDirection = false;
    private bool deltaZRotDirection = false;

    [MenuItem("CaveTools/创建相机模板")]
    static void CreateCaveInfoWindow()
    {
        DisplayWizard<CaveEditor>("Cave/大屏参数", "创建并更新");
    }

    private void OnEnable()
    {
        infoConfig = AssetDatabase.LoadAssetAtPath<InfoConfig>(INFO_CONFIG_PATH);
        UpdateJsonInfo(() =>
        {
            CameraInfo = infoConfig.CameraInfo;
        });
    }

    private void OnWizardCreate()
    {
        //覆写数据
        CoverJson(() =>
        {
            UpdateJsonInfo(() =>
            {
                //更新
                CameraInfo = infoConfig.CameraInfo;

                //查找CreatPlayerManager
                if (FindObjectOfType<PlayerCreatController>())
                {
                    CreatPlayerManager = FindObjectOfType<PlayerCreatController>().gameObject;        
                }
                else
                {
                    CreatPlayerManager = Instantiate(Resources.Load<GameObject>("CreatPlayerManager"));
                    CreatPlayerManager.name = "CreatPlayerManager"; 
                }

                //Player适配
                CreatPlayerManager.GetComponent<PlayerCreatController>().EditorInitStart((player) =>
                {
                    Transform Canvas = CreatPlayerManager.transform.Find("Canvas");
                    Canvas.GetComponent<Canvas>().worldCamera = player.transform.Find("Camera Offset/MainCamera/ForwardCamera").GetComponent<Camera>();
                    Canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(CameraInfo.frontSceneLong, CameraInfo.allSceneHigh);
                    if (CameraInfo.cameraType != CameraType.大屏)
                    {
                        Canvas.localPosition = player.transform.Find("Camera Offset/MainCamera/ForwardCamera").forward.normalized * CameraInfo.sideSceneLong / 2 + player.transform.Find("Camera Offset/MainCamera/ForwardCamera").localPosition;
                    }
                    else
                    {
                        Canvas.localPosition = player.transform.Find("Camera Offset/MainCamera/ForwardCamera").forward.normalized * CameraInfo.daPingUIDis + player.transform.Find("Camera Offset/MainCamera/ForwardCamera").localPosition;
                    }
                });
            });
        });
    }

    private void OnWizardUpdate()
    {
        isValid = true;
    }

    protected override bool DrawWizardGUI()
    {
        EditorGUILayout.LabelField("动作捕捉品牌设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 绘制品牌选择下拉框
        CameraInfo.motionCaptureBrand = (MotionCaptureBrand)EditorGUILayout.EnumPopup("品牌类型", CameraInfo.motionCaptureBrand);
        EditorGUILayout.Space();

        CameraInfo.openMotionCapture = EditorGUILayout.Toggle("是否开启动捕", CameraInfo.openMotionCapture);
        CameraInfo.ipServer = EditorGUILayout.TextField("服务器IP", CameraInfo.ipServer);

        if (CameraInfo.motionCaptureBrand == MotionCaptureBrand.青瞳)
        {
            CameraInfo.port = EditorGUILayout.IntField("端口号", CameraInfo.port);

            if (int.TryParse(CameraInfo.headTag, out int _headTag))
            {
                headTag = _headTag;
            }
            else
            {
                headTag = 0;
            }

            if (int.TryParse(CameraInfo.handTag, out int _handTag))
            {
                handTag = _handTag;
            }
            else
            {
                handTag = 0;
            }

            headTag = EditorGUILayout.IntField("头部动捕标签", headTag);
            handTag = EditorGUILayout.IntField("手柄动捕标签", handTag);
            CameraInfo.headTag = headTag.ToString();
            CameraInfo.handTag = handTag.ToString();
        }
        else
        {
            CameraInfo.headTag = EditorGUILayout.TextField("头部动捕标签", CameraInfo.headTag);
            CameraInfo.handTag = EditorGUILayout.TextField("手柄动捕标签", CameraInfo.handTag);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("动作捕捉品牌设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        CameraInfo.cameraType = (CameraType)EditorGUILayout.EnumPopup("屏幕类型", CameraInfo.cameraType);
        CameraInfo.open3D = EditorGUILayout.Toggle("是否开启3D模式", CameraInfo.open3D);
        CameraInfo.eyeStereoSeparation = EditorGUILayout.FloatField("眼间距", CameraInfo.eyeStereoSeparation);
        CameraInfo.allSceneHigh = EditorGUILayout.FloatField("整体屏高（单位：米）", CameraInfo.allSceneHigh);

        if (CameraInfo.cameraType == CameraType.大屏)
        {
            CameraInfo.frontSceneLong = EditorGUILayout.FloatField("正面屏长（单位：米）", CameraInfo.frontSceneLong);
        }
        else if (CameraInfo.cameraType == CameraType.三面屏)
        {
            CameraInfo.frontSceneLong = EditorGUILayout.FloatField("正面屏长（单位：米）", CameraInfo.frontSceneLong);
            CameraInfo.sideSceneLong = EditorGUILayout.FloatField("侧面屏长（单位：米）", CameraInfo.sideSceneLong);
        }
        else if (CameraInfo.cameraType == CameraType.四面屏)
        {
            CameraInfo.frontSceneLong = EditorGUILayout.FloatField("正面屏长（单位：米）", CameraInfo.frontSceneLong);
            CameraInfo.sideSceneLong = EditorGUILayout.FloatField("侧面屏长（单位：米）", CameraInfo.sideSceneLong);
            CameraInfo.groundSceneWide = EditorGUILayout.FloatField("地面屏宽（单位：米）", CameraInfo.groundSceneWide);
        }

        if (CameraInfo.cameraType == CameraType.大屏)
        {
            CameraInfo.daPingUIDis = EditorGUILayout.FloatField("屏幕UI的距离", CameraInfo.daPingUIDis);
        }

        deltaXDirection = EditorGUILayout.Toggle("X轴镜像", CameraInfo.deltaXDirection == 1 ? false : true);
        deltaYDirection = EditorGUILayout.Toggle("Y轴镜像", CameraInfo.deltaYDirection == 1 ? false : true);
        deltaZDirection = EditorGUILayout.Toggle("Z轴镜像", CameraInfo.deltaZDirection == 1 ? false : true);
        deltaXRotDirection = EditorGUILayout.Toggle("X轴旋转镜像", CameraInfo.deltaXRotDirection == 1 ? false : true);
        deltaYRotDirection = EditorGUILayout.Toggle("Y轴旋转镜像", CameraInfo.deltaYRotDirection == 1 ? false : true);
        deltaZRotDirection = EditorGUILayout.Toggle("Z轴旋转镜像", CameraInfo.deltaZRotDirection == 1 ? false : true);

        CameraInfo.deltaXDirection = deltaXDirection ? -1 : 1;
        CameraInfo.deltaYDirection = deltaYDirection ? -1 : 1;
        CameraInfo.deltaZDirection = deltaZDirection ? -1 : 1;
        CameraInfo.deltaXRotDirection = deltaXRotDirection ? -1 : 1;
        CameraInfo.deltaYRotDirection = deltaYRotDirection ? -1 : 1;
        CameraInfo.deltaZRotDirection = deltaZRotDirection ? -1 : 1;

        CameraInfo.deltaX = EditorGUILayout.FloatField("X轴增量", CameraInfo.deltaX);
        CameraInfo.deltaY = EditorGUILayout.FloatField("Y轴增量", CameraInfo.deltaY);
        CameraInfo.deltaZ = EditorGUILayout.FloatField("Z轴增量", CameraInfo.deltaZ);
        CameraInfo.deltaRotX = EditorGUILayout.FloatField("X轴旋转增量", CameraInfo.deltaRotX);
        CameraInfo.deltaRotY = EditorGUILayout.FloatField("Y轴旋转增量", CameraInfo.deltaRotY);
        CameraInfo.deltaRotZ = EditorGUILayout.FloatField("Z轴旋转增量", CameraInfo.deltaRotZ);

        EditorGUILayout.Space();
        return base.DrawWizardGUI();
    }

    public JsonInformation LoadJsonInformation(string jsonPath)
    {
        try
        {
            string jsonContent = File.ReadAllText(jsonPath);

            // 解析JSON
            JObject jObject = JObject.Parse(jsonContent);

            // 手动映射字段，便于调试
            JsonInformation info = new JsonInformation
            {
                openMotionCapture = jObject.Value<bool>("openMotionCapture"),
                ipServer = jObject.Value<string>("ipServer"),
                port = jObject.Value<int>("port"),
                motionCaptureBrand = jObject.Value<string>("motionCaptureBrand"),
                cameraType = jObject.Value<string>("cameraType"),
                headTag = jObject.Value<string>("headTag"),
                handTag = jObject.Value<string>("handTag"),
                open3D = jObject.Value<bool>("open3D"),
                eyeStereoSeparation = jObject.Value<float>("eyeStereoSeparation"),
                allSceneHigh = jObject.Value<float>("allSceneHigh"),
                sideSceneLong = jObject.Value<float>("sideSceneLong"),
                frontSceneLong = jObject.Value<float>("frontSceneLong"),
                groundSceneWide = jObject.Value<float>("groundSceneWide"),
                deltaXDirection = jObject.Value<int>("deltaXDirection"),
                deltaYDirection = jObject.Value<int>("deltaYDirection"),
                deltaZDirection = jObject.Value<int>("deltaZDirection"),
                deltaXRotDirection = jObject.Value<int>("deltaXRotDirection"),
                deltaYRotDirection = jObject.Value<int>("deltaYRotDirection"),
                deltaZRotDirection = jObject.Value<int>("deltaZRotDirection"),
                deltaX = jObject.Value<float>("deltaX"),
                deltaY = jObject.Value<float>("deltaY"),
                deltaZ = jObject.Value<float>("deltaZ"),
                deltaRotX = jObject.Value<float>("deltaRotX"),
                deltaRotY = jObject.Value<float>("deltaRotY"),
                deltaRotZ = jObject.Value<float>("deltaRotZ"),
                daPingUIDis = jObject.Value<float>("daPingUIDis")
            };
            return info;
        }
        catch (Exception ex)
        {
            JsonInformation info = new JsonInformation();
            Debug.LogError($"JSON解析失败: {ex.Message}");
            return info;
        }
    }

    public void UpdateJsonInfo(Action endEvent = null)
    {
        InformatioCenter InformatioCenter = new InformatioCenter();
        JsonInformation t = LoadJsonInformation(jsonFilePath);

        infoConfig.CameraInfo.openMotionCapture = t.openMotionCapture;
        infoConfig.CameraInfo.ipServer = t.ipServer;
        infoConfig.CameraInfo.port = t.port;
        infoConfig.CameraInfo.motionCaptureBrand = (MotionCaptureBrand)System.Enum.Parse(typeof(MotionCaptureBrand), t.motionCaptureBrand);
        infoConfig.CameraInfo.cameraType = (CameraType)System.Enum.Parse(typeof(CameraType), t.cameraType);
        infoConfig.CameraInfo.headTag = t.headTag;
        infoConfig.CameraInfo.handTag = t.handTag;
        infoConfig.CameraInfo.open3D = t.open3D;
        infoConfig.CameraInfo.eyeStereoSeparation = t.eyeStereoSeparation;
        infoConfig.CameraInfo.allSceneHigh = t.allSceneHigh;
        infoConfig.CameraInfo.sideSceneLong = t.sideSceneLong;
        infoConfig.CameraInfo.frontSceneLong = t.frontSceneLong;
        infoConfig.CameraInfo.groundSceneWide = t.groundSceneWide;
        infoConfig.CameraInfo.deltaXDirection = t.deltaXDirection;
        infoConfig.CameraInfo.deltaYDirection = t.deltaYDirection;
        infoConfig.CameraInfo.deltaZDirection = t.deltaZDirection;
        infoConfig.CameraInfo.deltaXRotDirection = t.deltaXRotDirection;
        infoConfig.CameraInfo.deltaYRotDirection = t.deltaYRotDirection;
        infoConfig.CameraInfo.deltaZRotDirection = t.deltaZRotDirection;
        infoConfig.CameraInfo.deltaX = t.deltaX;
        infoConfig.CameraInfo.deltaY = t.deltaY;
        infoConfig.CameraInfo.deltaZ = t.deltaZ;
        infoConfig.CameraInfo.deltaRotX = t.deltaRotX;
        infoConfig.CameraInfo.deltaRotY = t.deltaRotY;
        infoConfig.CameraInfo.deltaRotZ = t.deltaRotZ;
        infoConfig.CameraInfo.daPingUIDis = t.daPingUIDis;

        endEvent?.Invoke();
    }

    public void CoverJson(Action endEvent)
    {
        JsonInformation jsonInfo = new JsonInformation();
        jsonInfo.openMotionCapture = CameraInfo.openMotionCapture;
        jsonInfo.ipServer = CameraInfo.ipServer;
        jsonInfo.port = CameraInfo.port;
        jsonInfo.motionCaptureBrand = CameraInfo.motionCaptureBrand.ToString();
        jsonInfo.cameraType = CameraInfo.cameraType.ToString();
        jsonInfo.headTag = CameraInfo.headTag;
        jsonInfo.handTag = CameraInfo.handTag;
        jsonInfo.open3D = CameraInfo.open3D;
        jsonInfo.eyeStereoSeparation = CameraInfo.eyeStereoSeparation;
        jsonInfo.allSceneHigh = CameraInfo.allSceneHigh;
        jsonInfo.sideSceneLong = CameraInfo.sideSceneLong;
        jsonInfo.frontSceneLong = CameraInfo.frontSceneLong;
        jsonInfo.groundSceneWide = CameraInfo.groundSceneWide;
        jsonInfo.deltaXDirection = CameraInfo.deltaXDirection;
        jsonInfo.deltaYDirection = CameraInfo.deltaYDirection;
        jsonInfo.deltaZDirection = CameraInfo.deltaZDirection;
        jsonInfo.deltaXRotDirection = CameraInfo.deltaXRotDirection;
        jsonInfo.deltaYRotDirection = CameraInfo.deltaYRotDirection;
        jsonInfo.deltaZRotDirection = CameraInfo.deltaZRotDirection;
        jsonInfo.deltaX = CameraInfo.deltaX;
        jsonInfo.deltaY = CameraInfo.deltaY;
        jsonInfo.deltaZ = CameraInfo.deltaZ;
        jsonInfo.deltaRotX = CameraInfo.deltaRotX;
        jsonInfo.deltaRotY = CameraInfo.deltaRotY;
        jsonInfo.deltaRotZ = CameraInfo.deltaRotZ;
        jsonInfo.daPingUIDis = CameraInfo.daPingUIDis;

        // 覆写JSON数据
        OverwriteJsonData(jsonInfo, endEvent);
    }

    public void OverwriteJsonData(JsonInformation data, Action endEvent)
    {
        try
        {
            // 确保目录存在
            string directory = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"创建目录: {directory}");
            }

            // 将数据序列化为JSON字符串
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);

            // 使用无BOM的UTF-8编码
            Encoding utf8WithoutBom = new UTF8Encoding(false);

            // 写入文件
            File.WriteAllText(jsonFilePath, jsonString, utf8WithoutBom);
            Debug.Log($"已覆写JSON数据到: {jsonFilePath}，编码: UTF-8 无BOM");

            // 刷新Unity资产数据库
            if (Application.isEditor)
            {
                AssetDatabase.Refresh();
            }

            endEvent?.Invoke();
        }
        catch (IOException e)
        {
            Debug.LogError($"写入JSON文件时发生IO错误: {e.Message}");
        }
        catch (JsonException e)
        {
            Debug.LogError($"JSON序列化错误: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"未知错误: {e.Message}");
        }
    }
}
