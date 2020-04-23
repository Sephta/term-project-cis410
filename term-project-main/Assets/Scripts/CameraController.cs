using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Public Vars
    public GameObject player;
    public bool cameraMode; // false = Top Down, true = Iso
    public Camera topDown;
    public Camera isometric;

    public Vector3 cameraOffset;
    public float offsetSmoothing;

    // public float speedH;
    // public float speedV;

    //Private Vars
    Camera mainCamera;
    // float yaw = 0f;
    // float pitch = 0f;
    // float currentX;
    // float currentY;
    // Quaternion rotation;
    // Vector3 offset = new Vector3(0f, 0f, -5.0f);

    void Start()
    {
        if (cameraMode) {
            isometric.depth = -1;
            topDown.depth = -2;
            mainCamera = isometric;
        } else {
            isometric.depth = -2;
            topDown.depth = -1;
            mainCamera = topDown;
        }
    }

    void Update()
    {

        // currentX += Input.GetAxis("Mouse X");
        // currentY -= Input.GetAxis("Mouse Y");
        
        // yaw += speedH * Input.GetAxis("Mouse X");
        // pitch -= speedV * Input.GetAxis("Mouse Y");

        // yaw = Mathf.Clamp(yaw, -35f, 35f);
        // pitch = Mathf.Clamp(pitch, 25f, 45f);

        
        // mainCamera.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    void FixedUpdate()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, player.transform.position + cameraOffset - new Vector3(0f, 1f, 0f), offsetSmoothing * Time.deltaTime);
    }

    // void LateUpdate()
    // {
    //     rotation = Quaternion.Euler(currentY, currentX, 0f);
    //     mainCamera.transform.position = player.transform.position + rotation * offset;
    //     mainCamera.transform.LookAt(player.transform.position);
    // }

}
