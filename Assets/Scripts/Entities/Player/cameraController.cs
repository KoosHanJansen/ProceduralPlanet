using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public float sensitivity;
    public float smoothing;

    private Transform _Parent;

    Vector2 smoothMouse = Vector2.zero;
    Vector2 smooth = Vector2.zero;

    private void Start()
    {
        _Parent = transform.parent;
    }
    
    void Update()
	{
        Vector2 mouseDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseDir = Vector2.Scale(mouseDir, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smooth.x = Mathf.Lerp(smooth.x, mouseDir.x, 1f / smoothing);
        smooth.y = Mathf.Lerp(smooth.y, mouseDir.y, 1f / smoothing);

        smoothMouse += smooth;
        smoothMouse.y = Mathf.Clamp(smoothMouse.y, -90f, 90f);

        this.transform.localRotation = Quaternion.AngleAxis(-smoothMouse.y, Vector3.right);
        _Parent.localRotation = Quaternion.AngleAxis(smoothMouse.x, _Parent.up);
    }
}
