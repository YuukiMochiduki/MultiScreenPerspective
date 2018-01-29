using UnityEngine;
using System.Collections;

//  Attatch main camera
public class MultiScreenCameraManager : MonoBehaviour {

    public static MultiScreenCameraManager instance;

    public enum ScreenLayoutType { Wide, Corner, Surround };

    public ScreenLayoutType scrLayoutType = ScreenLayoutType.Wide;

    public GameObject mainCamera;

    public int cameraNum;

    [HideInInspector]
    public GameObject[] goMultiCameras;

    private MultiScreenCamera[] multiScreenCameras;

    public ScreenPlane[] screenPlanes;

    private Vector3[] startPosScreenPlanes;

    public float bezel = 0.0f;

    void Awake()
    {
        instance = this;

        bezel = PlayerPrefs.GetFloat("Bezel" + scrLayoutType.ToString(), 0.0f);
    }

    // Use this for initialization
    void Start()
    {
        // layout center of screen align when odd number
        if (scrLayoutType == ScreenLayoutType.Wide)
        {
            if (cameraNum % 2 == 1)
            {
                Vector3 deff = (screenPlanes[0].transform.localPosition - screenPlanes[1].transform.localPosition) * 0.5f;

                foreach (ScreenPlane scrP in screenPlanes)
                {
                    scrP.transform.localPosition -= deff;
                }
            }
        }

        startPosScreenPlanes = new Vector3[screenPlanes.Length];

        for (int i = 0; i < startPosScreenPlanes.Length; i++)
        {
            startPosScreenPlanes[i] = screenPlanes[i].transform.localPosition;
        }

        goMultiCameras = new GameObject[cameraNum];

        multiScreenCameras = new MultiScreenCamera[cameraNum];

        for (int i = 0; i < cameraNum; i++)
        {
            goMultiCameras[i] = (GameObject)Instantiate(mainCamera);

            goMultiCameras[i].name = "MultiScreenCamera" + (i+1).ToString("00");

            goMultiCameras[i].GetComponent<Camera>().depth = - i - 1;

            multiScreenCameras[i] = goMultiCameras[i].AddComponent<MultiScreenCamera>();

            multiScreenCameras[i].multiScreenCamera = goMultiCameras[i].GetComponent<Camera>();

            multiScreenCameras[i].screenPlane = this.screenPlanes[i];

            multiScreenCameras[i].multiScreenCamera.targetDisplay = i;
        }

        mainCamera.GetComponent<Camera>().enabled = false;

        for (int i = 0; i < cameraNum; i++)
        {
            goMultiCameras[i].transform.parent = this.transform;
        }
	}

    void LateUpdate()
    {
        //  ベゼル処理
        if (scrLayoutType == ScreenLayoutType.Wide)
        {
            for (int i = 0; i < screenPlanes.Length; i++)
            {
                screenPlanes[i].transform.localPosition = startPosScreenPlanes[i] + (i - screenPlanes.Length * 0.5f + ((cameraNum % 2 == 1) ? 1.0f : 0.5f)) * bezel * Vector3.left;
            }
        }
        else if (scrLayoutType == ScreenLayoutType.Corner)
        {
            for (int i = 0; i < 3; i++)
            {
                screenPlanes[i].transform.localPosition = startPosScreenPlanes[i] - (3 - i) * bezel * screenPlanes[i].transform.right;
            }
            for (int i = 3; i < 6; i++)
            {
                screenPlanes[i].transform.localPosition = startPosScreenPlanes[i] + (i - 2) * bezel * screenPlanes[i].transform.right;
            }
        }
    }

    // Update is called once per frame
    void OnGUI()
    {
	    //  ベゼル処理
        if (Input.GetKey(KeyCode.B))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                bezel += 0.001f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                bezel -= 0.001f;
            }

            GUI.Box(new Rect(0, Screen.height - 20, 200, 20), "Bezel : " + bezel.ToString("0.000"));
        }
	}

    void OnDisable()
    {
        PlayerPrefs.SetFloat("Bezel" + scrLayoutType.ToString(), bezel);
    }	
}
