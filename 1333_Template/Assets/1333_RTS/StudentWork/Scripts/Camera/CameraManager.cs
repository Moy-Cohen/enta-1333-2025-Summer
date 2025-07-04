using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public float PanSpeed = 10f;
    public float PanBorderThickness = 10f;
    public Vector2 PanLimit;
    public float ScrollSpeed = 10f;
    public float MinY = 1f;
    public float MaxY = 20f;
    public float RotateSpeed = 20f;
    public float RotationX;
    public float RotationY;
  
    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = transform.position;

        // WASD Camera Movement
        if (Input.GetKey(KeyCode.W))
        {
            cameraPos.z += PanSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraPos.z -= PanSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraPos.x += PanSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraPos.x -= PanSpeed * Time.deltaTime;
        }

        // Mouse Wheel Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraPos.y -= scroll * ScrollSpeed * 100f * Time.deltaTime;

        // Clamp Position
        cameraPos.x  = Mathf.Clamp(cameraPos.x, -PanLimit.x, PanLimit.x);
        cameraPos.y = Mathf.Clamp(cameraPos.y, MinY, MaxY);
        cameraPos.z = Mathf.Clamp(cameraPos.z, -PanLimit.y, PanLimit.y);

        transform.position = cameraPos;

        // Q and E Camera Rotation
        float rotationAmount = RotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            transform.RotateAround(transform.position, Vector3.up, -rotationAmount);
        if (Input.GetKey(KeyCode.E))
            transform.RotateAround(transform.position, Vector3.up, rotationAmount);
    }
}
