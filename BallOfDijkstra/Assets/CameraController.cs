using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform cameraPivot;
    float pitch;
    float yaw;
    [SerializeField] Vector3 cameraFollowOffset;
    [SerializeField] Vector3 cameraTopView;
    // Start is called before the first frame update
    void Start()
    {
        //The pivot of the camera is basicly the same as its parent
        cameraPivot = transform.parent;
        cameraFollowOffset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            pitch -= Input.GetAxis("Mouse Y");
            yaw += Input.GetAxis("Mouse X");
            pitch = Mathf.Clamp(pitch, -60, 60);
            cameraPivot.transform.rotation = Quaternion.Euler(new Vector3(pitch, yaw, 0));
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.SetParent(null);
            transform.position = cameraTopView;
            transform.rotation = Quaternion.Euler(new Vector3(80, 0, 0));
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            transform.SetParent(cameraPivot);
            transform.localPosition = cameraFollowOffset;
            transform.localRotation = Quaternion.Euler(new Vector3(40, 0, 0));
        }
    }
}
