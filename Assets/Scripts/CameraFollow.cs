using System;
using UnityEngine;
using Cinemachine;
using mactinite.ToolboxCommons;
using UnityEngine.InputSystem;

public class CameraFollow : SingletonMonobehavior<CameraFollow>
{
    public Transform followTarget;
    public LayerMask targetLockLayers;
    public float targetLockDistance = 15f;
    public CinemachineTargetGroup TargetGroup;
    private Transform _lockOnTarget;
    public float lateralSensitivity = 5;
    public float verticalSensitivity = 5;

    private Transform _cameraTrackingTransform;

    public CinemachineVirtualCamera followCam;
    public float smoothTime = 1f;

    private float _lastYPosition = 0;
    private Vector3 _vel;


    private Vector2 _look = Vector2.zero;
    private float rotationPower = 3f;
    private bool _lockCursor;

    public PlayerInput input;
    private Vector2 _mousePos = Vector2.zero;

    private bool targetLock = false;
    private Transform lockedOn = null;
    Collider[] targetableObjects = new Collider[10];


    private void Awake()
    {
        if (_cameraTrackingTransform == null)
        {
            _cameraTrackingTransform = new GameObject("Camera Target").transform;
            followCam.Follow = _cameraTrackingTransform;
        }

        SetCursorState(true);
        input.actions["Release Mouse"].performed += ReleaseCursor;
        input.actions["Punch"].performed += LockCursor;
        input.actions["Toggle Lock On"].performed += TargetLock;
    }

    private void OnDisable()
    {
        input.actions["Release Mouse"].performed -= ReleaseCursor;
        input.actions["Punch"].performed -= LockCursor;
        input.actions["Toggle Lock On"].performed -= TargetLock;
    }

    private void TargetLock(InputAction.CallbackContext obj)
    {
        // toggle target lock
        if (targetLock)
        {
            targetLock = false;
            TargetGroup.RemoveMember(lockedOn);
        }
        else
        {
            // get target lock tagged objects and set target to closest;

            var size = Physics.OverlapSphereNonAlloc(transform.position, targetLockDistance, targetableObjects, targetLockLayers);
            if (size > 0)
            {
                targetLock = true;
                lockedOn = targetableObjects[0].transform;
                TargetGroup.AddMember(lockedOn, 1, 5);
            }
        }
    }

    private void ReleaseCursor(InputAction.CallbackContext obj)
    {
        SetCursorState(false);
    }

    private void LockCursor(InputAction.CallbackContext obj)
    {
        if(!_lockCursor)
            SetCursorState(true);
    }

    private void Update()
    {
        if (lockedOn == null)
        {
            targetLock = false;
        }
        
        if (_cameraTrackingTransform == null)
        {
            _cameraTrackingTransform = new GameObject("Camera Target").transform;
            followCam.Follow = _cameraTrackingTransform;
            followCam.LookAt = _cameraTrackingTransform;
        }

        if (followTarget != null && _lockCursor)
        {
            var desiredPosition = followTarget.position;
            _cameraTrackingTransform.position = Vector3.SmoothDamp(_cameraTrackingTransform.position, desiredPosition,
                ref _vel, smoothTime);
            Rotation();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(true);
    }

    private void SetCursorState(bool newState)
    {
        _lockCursor = newState;
        if (newState)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    public void LockCursor()
    {
        _lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        _lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Rotation()
    {
        if (!targetLock)
        {
            // we get mouse position from player input as either mouse delta or gamepad joystick
            // "Look" binding is set up for both gamepad and mouse delta
            _mousePos = input.actions["Look"].ReadValue<Vector2>() * Time.deltaTime;
            _look.x = _mousePos.x * lateralSensitivity;
            _look.y = -(_mousePos.y * verticalSensitivity);


            var rotation = _cameraTrackingTransform.transform.rotation;
            rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
            rotation *= Quaternion.AngleAxis(_look.y * rotationPower, Vector3.right);
            _cameraTrackingTransform.transform.rotation = rotation;
        }
        else if (lockedOn != null)
        {
            var dirToTarget = lockedOn.transform.position - followTarget.transform.position;
            dirToTarget.y = 0;
            var rotation = Quaternion.LookRotation(dirToTarget.normalized);
            _cameraTrackingTransform.transform.rotation = rotation;
            
        }
        

        var angles = _cameraTrackingTransform.localEulerAngles;
            angles.z = 0;
            

            var angle = _cameraTrackingTransform.localEulerAngles.x;

            //Clamp the Up/Down rotation
            if (angle > 180 && angle < 340)
            {
                angles.x = 340;
            }
            else if (angle < 180 && angle > 40)
            {
                angles.x = 40;
            }


            _cameraTrackingTransform.localEulerAngles = angles;

    }

    private void OnDrawGizmos()
    {
        if (Application.IsPlaying(this))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_cameraTrackingTransform.position, 0.25f);
        }
    }
}