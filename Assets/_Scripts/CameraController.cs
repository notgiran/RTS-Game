using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float panSpeed;
    [SerializeField] float scrollSpeed;
    [SerializeField] float panBorderThickness;
    [SerializeField] Vector3 limits;

    private void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos.z += panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
            pos.z -= panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
            pos.x += panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
            pos.x -= panSpeed * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 1000f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, -limits.y, limits.y);

        pos.x = Mathf.Clamp(pos.x, -limits.x, limits.x);
        pos.z = Mathf.Clamp(pos.z, -limits.z, limits.z);

        transform.position = pos;
    }
}
