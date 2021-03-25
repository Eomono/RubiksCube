using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{
    public float ZoomDistance { get; private set; }

    private Transform anchor;
    private Quaternion targetRotation;
    private Vector3 camRotFocus, camLookFocus, dampVelAnchor, dampVelCamPos;
    private float xRotation, yRotation;

    private float rotationSensitivity = 1250f;
    private float zoomSensitivity = 1250f;
    private const float anchorRotSmooth = 12f;
    private const float zoomSmoothTime = 0.08f; 

    private Vector2 zoomLimits;

    private void Awake()
    {
        //Creating a pivot to rotate around
        anchor = new GameObject("CameraControlAnchor").transform;
        anchor.position = Vector3.zero;
        anchor.rotation = Quaternion.identity;
        transform.SetParent(anchor);
    }

    public void SetUpInitialTargets(int size)
    {
        zoomLimits = new Vector2(size + (size * 0.5f), size * 5f);
        ZoomDistance = zoomLimits.x + (size * 1.6f);
        
        Vector3 angles = transform.eulerAngles;
        xRotation = angles.y - 45f;
        yRotation = angles.x + 24f;
        targetRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }

    public void OverrideToPosition(float zoomDistance, float xRotAdd, float yRotSet)
    {
        ZoomDistance = zoomLimits.x + zoomDistance;
        
        Vector3 angles = transform.eulerAngles;
        xRotation = angles.y + xRotAdd;
        yRotation = yRotSet;
        targetRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }
    
    private void LateUpdate()
    {
        anchor.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * anchorRotSmooth);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0f, 0f, -ZoomDistance), ref dampVelCamPos, zoomSmoothTime, float.MaxValue, Time.deltaTime);
    }

    public void RotateAroundCenter(Vector2 amount)
    {
        xRotation += amount.x * rotationSensitivity * Time.deltaTime * ((transform.up.y < 0f)?-1f:1f);
        yRotation -= amount.y * rotationSensitivity * Time.deltaTime;

        targetRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }

    public void Zoom(float amount)
    {
        ZoomDistance -= (amount * zoomSensitivity * Time.deltaTime);

        ZoomDistance = Mathf.Clamp(ZoomDistance, zoomLimits.x, zoomLimits.y);
    }
}