/*-----------------------------------------------------
    文件：CameraRotateAround.cs
	作者：Johnson
    日期：2023/8/20 18:50:2
	功能：摄像机围绕旋转
------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//摄像机操作   
//删减版，在实际的使用中可能会有限制的需求，比如最大远离多少，最近距离多少，不能旋转到地面以下等
public class CameraRotateAround : MonoBehaviour {
    public Transform CenObj;//围绕的物体
    public bool isMouseControlEnabled = true;
    public bool isWndOpen = false;


    void Update() {
/*        // 检测是否按住了Tab键
        if (Input.GetKey(KeyCode.Tab)) {
            isMouseControlEnabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            isMouseControlEnabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }*/

        if (CenObj != null && isMouseControlEnabled) {
            Ctrl_Cam_Move();
            Cam_Ctrl_Rotation();
            //transform.rotation = Quaternion.Lerp(transform.rotation, CenObj.rotation, Time.deltaTime);
            //transform.position = CenObj.position + new Vector3(-2,2,-2);
        }
    }
    //镜头的远离和接近
    public void Ctrl_Cam_Move() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            transform.Translate(Vector3.forward * 0.1f);//速度可调  自行调整
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            transform.Translate(Vector3.forward * -0.1f);//速度可调  自行调整
        }
    }
    //摄像机的旋转
    public void Cam_Ctrl_Rotation() {
        var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
        var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动
        transform.RotateAround(CenObj.position, Vector3.up, mouse_x * Constants.ViewSensitivity);
        transform.RotateAround(CenObj.position, transform.right, mouse_y * Constants.ViewSensitivity);
        transform.LookAt(CenObj);
    }
}