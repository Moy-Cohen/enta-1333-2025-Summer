using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public float PanSpeed = 20f;
    public float PanBorderThickness = 10f;
    public Vector2 PanLimit;
    public float ScrollSpeed = 20f;
    public float MinY = 1f;
    public float MaxY = 20f;
  
    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - PanBorderThickness)
        {
            cameraPos.z += PanSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= PanBorderThickness)
        {
            cameraPos.z -= PanSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - PanBorderThickness)
        {
            cameraPos.x += PanSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= PanBorderThickness)
        {
            cameraPos.x -= PanSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraPos.y -= scroll * ScrollSpeed * 100f * Time.deltaTime;

        cameraPos.x  = Mathf.Clamp(cameraPos.x, -PanLimit.x, PanLimit.x);
        cameraPos.y = Mathf.Clamp(cameraPos.y, MinY, MaxY);
        cameraPos.z = Mathf.Clamp(cameraPos.z, -PanLimit.y, PanLimit.y);

        transform.position = cameraPos;
    }
}
