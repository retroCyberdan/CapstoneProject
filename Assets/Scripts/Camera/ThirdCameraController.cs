using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdCameraController : MonoBehaviour
{
    public AxisState xAxis;
    public AxisState yAxis;

    public Transform target;

    void Start()
    {
        // inizializza la camera guardando in avanti (forward del target)
        float initialYRotation = target.eulerAngles.y;
        xAxis.Value = initialYRotation;
        yAxis.Value = 0; // <- angolo orizzontale

        HideCursor(); // <- nasconde e blocca il cursore all'avvio
    }

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

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}