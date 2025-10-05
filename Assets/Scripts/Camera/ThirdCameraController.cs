using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdCameraController : MonoBehaviour
{
    public AxisState xAxis;
    public AxisState yAxis;

    public Transform target;

    // Update is called once per frame
    void Update()
    {
        OnThirdCameraRotation();
    }

    private void OnThirdCameraRotation()
    {
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);

        target.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, xAxis.Value, 0), Time.deltaTime * 5);
    }
}
