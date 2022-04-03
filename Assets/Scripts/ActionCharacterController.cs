using ECM.Common;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;


    /// <summary>
    /// 
    /// Example Character Controller
    /// 
    /// This example shows how to extend the 'BaseCharacterController' adding support for different
    /// character speeds (eg: walking, running, etc), plus how to handle custom input extending the
    /// HandleInput method and make the movement relative to camera view direction.
    /// 
    /// </summary>

    public sealed class ActionCharacterController : BaseCharacterController
    {
        #region EDITOR EXPOSED FIELDS

        [Header("CUSTOM CONTROLLER")]
        [Tooltip("The character's follow camera.")]
        public Transform playerCamera;

        [Tooltip("The character's run speed.")]
        [SerializeField]
        private float _runSpeed = 5.0f;

        [SerializeField] private GameObject visualsObject;
        
        [SerializeField]
        private bool rotateToMoveDirection = false;
        
        [SerializeField]
        private float movementLeanAmount = 15f;
        #endregion

        #region INPUT SYSTEM
        
        public PlayerInput input;
        #endregion

        #region PROPERTIES

        private bool _canControl = true;
        

        /// <summary>
        /// The character's run speed.
        /// </summary>

        public float runSpeed
        {
            get { return _runSpeed; }
            set { _runSpeed = Mathf.Max(0.0f, value); }
        }

        public bool canControl
        {
            get => _canControl;
            set => _canControl = value;
        }

        #endregion

        #region METHODS

        public void Knock(Vector3 direction)
        {
            movement.ApplyImpulse(direction);
        }

        /// <summary>
        /// Get target speed based on character state (eg: running, walking, etc).
        /// </summary>

        private float GetTargetSpeed()
        {
            return runSpeed;
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' CalcDesiredVelocity method to handle different speeds,
        /// eg: running, walking, etc.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            // Set 'BaseCharacterController' speed property based on this character state

            speed = GetTargetSpeed();

            // Return desired velocity vector

            return base.CalcDesiredVelocity();
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' Animate method.
        /// 
        /// This shows how to handle your characters' animation states using the Animate method.
        /// The use of this method is optional, for example you can use a separate script to manage your
        /// animations completely separate of movement controller.
        /// 
        /// </summary>

        protected override void Animate()
        {


            // Compute move vector in local space

            var move = transform.InverseTransformDirection(moveDirection);

            // Update the animator parameters

            // var forwardAmount = animator.applyRootMotion
            //     ? Mathf.InverseLerp(0.0f, runSpeed, move.z * speed)
            //     : Mathf.InverseLerp(0.0f, runSpeed, movement.forwardSpeed);
            var forwardAmount = Mathf.InverseLerp(0.0f, runSpeed, movement.forwardSpeed);

            float leanAmount = Mathf.Lerp(0, movementLeanAmount, forwardAmount);
            visualsObject.transform.localRotation = Quaternion.AngleAxis(leanAmount, Vector3.right);
            
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput,
        /// to perform custom controller input.
        /// </summary>

        protected override void HandleInput()
        {
            if (_canControl)
            {
                // Handle your custom input here...
                Vector2 movement = input.actions["Move"].ReadValue<Vector2>();

                moveDirection = new Vector3
                {
                    x = movement.x,
                    y = 0.0f,
                    z = movement.y
                };

                jump = input.actions["Jump"].ReadValue<float>() > 0;
            }
            else
            {
                moveDirection = new Vector3
                {
                    x = 0,
                    y = 0.0f,
                    z = 0
                };
            }
            

            // Transform moveDirection vector to be relative to camera view direction
            moveDirection = moveDirection.relativeTo(playerCamera ? playerCamera : Camera.main.transform);


        }

        protected override void UpdateRotation()
        {

            if (rotateToMoveDirection)
            {
                base.UpdateRotation();
            } else
            {
                RotateTowards(playerCamera ? playerCamera.forward : Camera.main.transform.forward, true);
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Overrides 'BaseCharacterController' OnValidate method,
        /// to perform this class editor exposed fields validation.
        /// </summary>
        private void Start()
        {
            if(input == null)
            {
                input = Camera.main.GetComponent<PlayerInput>();
            }
        }
        public override void OnValidate()
        {
            // Validate 'BaseCharacterController' editor exposed fields

            base.OnValidate();

            runSpeed = _runSpeed;
        }

        #endregion
    }

