using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    public GameObject playerObject;
    public GameObject LookAtMe;
    public Vector3 offset;

    private const float min = -10.0f; 
    private const float max = 10.0f;
    private Vector3 PlayerForward;
    

    private void Start()
    {
        offset = new Vector3(0, 1.7f, -4.0f);
        transform.LookAt(LookAtMe.transform);
        transform.position = playerObject.transform.position + offset;
        PlayerForward = playerObject.transform.forward;
    }

    // Update is called once per frame
    void LateUpdate () {
        RotateCamera();
        ZoomCamera();
        TranslateCamera();
    }

    private void ZoomCamera()
    {
        float translation = -Input.GetAxis("Mouse ScrollWheel");

        Vector3 zoom = Vector3.Normalize(transform.position - LookAtMe.transform.position) * translation;
        offset += zoom;
    }
   

    private void TranslateCamera()
    {
        transform.position = playerObject.transform.position + offset;
        transform.LookAt(LookAtMe.transform);
    }

    private float LimitAngles(float angle, float min, float max)
    {
        if (angle > max)
            return max;
        else if (angle < min)
            return min;
        else
            return angle;
    }

    private void RotateCamera()
    {
        if (Input.GetButton("Mouse X"))
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 2.0f, Vector3.up) * offset;
            offset.x = LimitAngles(offset.x, min, max);

            offset.y = (Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 3.0f, Vector3.right) * offset).y;
            offset.y = LimitAngles(offset.y, 0.4f, max);       
            
        } //else if (PlayerForward != playerObject.transform.forward) {
        //    float angle = Quaternion.Angle(Quaternion.Euler(offset), Quaternion.Euler(playerObject.transform.forward)) - cameraRotatingSpeed;
        //    //Debug.Log(angle);
        //    if (!playerObject.GetComponent<MainReactive>().side)
        //    {
        //        angle = -angle;
        //    } 
        //    offset = Quaternion.AngleAxis(angle, Vector3.up) * offset;
        //    PlayerForward = playerObject.transform.forward;
        //}
    }
}
