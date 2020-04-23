using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Camera mainCamera;

    public float yaw;
    public float speedH;

    [SerializeField] Vector3 cameraOffset = new Vector3(0.0f, 5.0f, -5.0f);
    public float offsetSmoothing = 0.0f;

    void Start() {}

    void Update()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position + cameraOffset - new Vector3(0f, 1f, 0f), offsetSmoothing * Time.deltaTime);
        
        yaw += speedH * Input.GetAxis("Mouse X");
        mainCamera.transform.eulerAngles = new Vector3(35f, yaw, 0f);
    }
}
