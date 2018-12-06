using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Camera cam;
    private Transform thisTransform;
    private float camSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        thisTransform = GetComponent<Transform>();
    }

    public void SetCamSize(float xSize, float ySize, float cellD)
    {
        camSize = (xSize > ySize ? xSize * 0.5f : ySize * 0.5f) + cellD;
        cam.orthographicSize = camSize++;
        thisTransform.position += new Vector3(cellD * 0.5f, 0, cellD * 0.5f);
    }
}
