using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasFollowCave : MonoBehaviour
{
    private Canvas canvasObj;

    private CamManagerScr _camManager;
    private CamManagerScr camManager
    {
        get
        {
            if (_camManager == null)
            {
                _camManager = FindObjectOfType<CamManagerScr>();
            }
            return _camManager;
        }
    }

    void Start()
    {
        canvasObj = GetComponent<Canvas>();
    }

    void Update()
    {
        UpdateCavasePos();
    }

    public void UpdateCavasePos()
    {
        if (canvasObj == null)
            canvasObj = GetComponent<Canvas>();

        if (canvasObj.worldCamera == null)
        {
            canvasObj.worldCamera = Camera.main;
        }
        if (Camera.main != null)
        {
            if (camManager && !camManager.eyeLock)
            {
                if (_camManager)
                {
                    transform.position = Camera.main.transform.position - Camera.main.transform.right * Camera.main.transform.localPosition.x - Camera.main.transform.forward * Camera.main.transform.localPosition.z - Camera.main.transform.up * Camera.main.transform.localPosition.y + Camera.main.transform.forward * (camManager.sideSceneLong / 2.00f) + new Vector3(0, (camManager.allSceneHigh / 2.00f), 0);
                }
                else
                {
                    transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized * Camera.main.nearClipPlane;
                }
                transform.rotation = Camera.main.transform.rotation;
            }
            else
            {
                if (_camManager)
                {
                    transform.position = Camera.main.transform.position + Camera.main.transform.forward * (camManager.sideSceneLong / 2.00f);
                }
                else
                {
                    transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized * Camera.main.nearClipPlane;
                }
                transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}