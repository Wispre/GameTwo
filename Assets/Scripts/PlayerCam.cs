using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float Xsens;
    public float Ysens;

    public Transform orientation;

    float RotationX;
    float RotationY;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        float mouseX = Input.GetAxisRaw("Mouse X")* Time.deltaTime*Xsens;
        float mouseY = Input.GetAxisRaw("Mouse Y")* Time.deltaTime*Ysens;

        RotationX =RotationX -mouseY;

        RotationY = RotationY + mouseX;

        RotationX = Mathf.Clamp(RotationX,-90f,90f);

        //rotate cam and orientation
        transform.rotation = Quaternion.Euler(RotationX,RotationY,0);
        orientation.rotation = Quaternion.Euler(0,RotationY,0);




    }

}
