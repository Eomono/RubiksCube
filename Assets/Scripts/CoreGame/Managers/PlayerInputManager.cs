using UnityEngine;
using UnityEngine.EventSystems;
using static CubeUtils;

public class PlayerInputManager : MonoBehaviour
{
    private enum EngagedState { Idle, OnCube, RotatingCam }

    [SerializeField] private Transform catcher;
    
    private Camera cam;
    private CameraMovementManager cameraMovementManager;
    private CubeManager cubeManager;
    private GameManager gameManager;

    private EngagedState currentState;
    private LayerMask cubeletLayer;
    private LayerMask actionCatcherLayer;
    
    private Transform cubeletFromAction;
    private Vector3 cubeActionPoint;
    private Vector2 screenActionPoint;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cameraMovementManager = gameObject.GetComponent<CameraMovementManager>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        cubeletLayer = LayerMask.GetMask("Cubelet");
        actionCatcherLayer = LayerMask.GetMask("ActionCatcher");
    }

    public void SetCubeManager(CubeManager cm)
    {
        cubeManager = cm;
    }

    private void Update()
    {
        if(gameManager.PlayerCanRotateCube && (currentState == EngagedState.Idle || currentState == EngagedState.OnCube))
            CheckActionOnCube();

        if(!gameManager.PlayerCanMoveCamera) return;
        
        if(currentState == EngagedState.Idle || currentState == EngagedState.RotatingCam)
            CheckCameraRotation();
        
        CheckZoom();
    }

    private void CheckActionOnCube()
    {
        if (currentState == EngagedState.Idle)
        {
            if(CheckMainInputBegan)
            {
                Ray ray = cam.ScreenPointToRay(GetMainInputPosition);
                if ( Physics.Raycast(ray, out var hit, 100f, cubeletLayer))
                {
                    currentState = EngagedState.OnCube;
                    
                    cubeletFromAction = hit.transform;
                    cubeActionPoint = hit.point;
                    screenActionPoint = cam.ScreenToViewportPoint(GetMainInputPosition);
                    
                    catcher.position = hit.point;
                    catcher.forward = hit.normal;
                }
            }
        }

        if (currentState != EngagedState.OnCube) return;
        
        if (CheckMainInputOn)
        {
            if (!(Vector2.Distance(screenActionPoint, cam.ScreenToViewportPoint(GetMainInputPosition)) > (0.3f / cameraMovementManager.ZoomDistance))) return;

            Ray ray = cam.ScreenPointToRay(GetMainInputPosition);
            if (Physics.Raycast(ray, out var hit, 100f, actionCatcherLayer))
            {
                Vector3 localDir = catcher.InverseTransformDirection((hit.point - cubeActionPoint).normalized);
                    
                Vector3 chosenDir;
                if (Mathf.Abs(localDir.x) > Mathf.Abs(localDir.y))
                    chosenDir = catcher.up * ((localDir.x > 0f) ? 1f : -1f);
                else
                    chosenDir = catcher.right * ((localDir.y > 0f) ? -1f : 1f);

                EvaluateDirectionToRotAxis(chosenDir, out Axis rotAxis, out bool clockwise);

                cubeManager.SetRotation(cubeletFromAction, rotAxis, clockwise);
            }
                
            currentState = EngagedState.Idle;
        }
        else
            currentState = EngagedState.Idle;
    }

    private void CheckCameraRotation()
    {
        if (CheckMainInputOn)
        {
            if (CheckMainInputBegan && InputNotOverMenu)
                currentState = EngagedState.RotatingCam;
        }
        else
            currentState = EngagedState.Idle;

        if (currentState != EngagedState.RotatingCam) return;
        
        cameraMovementManager.RotateAroundCenter(GetMainInputMoveDelta);
    }

    private void CheckZoom()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        if (!screenRect.Contains(GetMainInputPosition) && InputNotOverMenu) return;

        float scroll = GetZoomInput;
        if (IsBetweenRange(scroll, -0.01f, 0.01f) || !InputNotOverMenu) return;
        
        cameraMovementManager.Zoom(scroll);
    }

    private bool CheckMainInputBegan
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount < 1) return false;
            Touch touch = Input.GetTouch(0);
            return (touch.phase == TouchPhase.Began);
#endif
        }
    }

    private bool CheckMainInputOn
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Input.GetMouseButton(0);
#elif UNITY_ANDROID || UNITY_IOS
            return (Input.touchCount > 0);
#endif
        }
    }

    private Vector3 GetMainInputPosition
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
            Vector3 pos = Vector3.zero;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                pos = touch.position;
            }
            return pos;
#endif
        }
    }

    private Vector2 GetMainInputMoveDelta
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#elif UNITY_ANDROID || UNITY_IOS
            Vector2 delta = Vector2.zero;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                delta = touch.deltaPosition;
                delta.x /= Screen.width;
                delta.y /= Screen.height;
                delta *= 7f;
            }
            return delta;
#endif
        }
    }

    private float GetZoomInput
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Input.GetAxis("Mouse ScrollWheel");
#elif UNITY_ANDROID || UNITY_IOS
            float zoom = 0f;
            if (Input.touchCount > 1)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                touchZeroPrevPos.x /= Screen.width;
                touchZeroPrevPos.y /= Screen.height;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                touchOnePrevPos.x /= Screen.width;
                touchOnePrevPos.y /= Screen.height;
                float prevDist = Vector2.Distance (touchZeroPrevPos, touchOnePrevPos);

                Vector2 touchZeroCurrentPos = touchZero.position;
                touchZeroCurrentPos.x /= Screen.width;
                touchZeroCurrentPos.y /= Screen.height;
                Vector2 touchOneCurrentPos = touchOne.position;
                touchOneCurrentPos.x /= Screen.width;
                touchOneCurrentPos.y /= Screen.height;
                
                float currentDist = Vector2.Distance (touchZeroCurrentPos, touchOneCurrentPos);
                zoom = currentDist - prevDist;
                zoom *= 2f;
            }
            return zoom;
#endif
        }
    }

#if UNITY_STANDALONE || UNITY_EDITOR
    private static bool InputNotOverMenu => !EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
    private static bool InputNotOverMenu => !EventSystem.current.IsPointerOverGameObject(Input.touchCount>0?Input.GetTouch(0).fingerId:-1);
#endif

    private void EvaluateDirectionToRotAxis(Vector3 dir, out Axis rotAxis, out bool cw)
    {
        float angleTreshold = 5f;
        
        if (Vector3.Angle(dir, cubeManager.transform.up) < angleTreshold || Vector3.Angle(dir, -cubeManager.transform.up) < angleTreshold)
        {
            rotAxis = Axis.Y;
            cw = (Vector3.Angle(dir, cubeManager.transform.up) < angleTreshold);
        }
        else if (Vector3.Angle(dir, cubeManager.transform.right) < angleTreshold || Vector3.Angle(dir, -cubeManager.transform.right) < angleTreshold)
        {
            rotAxis = Axis.X;
            cw = (Vector3.Angle(dir, cubeManager.transform.right) < angleTreshold);
        }
        else
        {
            rotAxis = Axis.Z;
            cw = (Vector3.Angle(dir, cubeManager.transform.forward) < angleTreshold);
        }
    }
    
    private static bool IsBetweenRange(float value, float min, float max)
    {
        return (value > min && value < max);
    }
}