using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    // declare reference variables
    CharacterController characterController;
    CharacterInputController playerInput;
    Player childrenPlayer_;

    // variables to store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    bool isGrounded_; 
    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;

    string[] buffer = new string[1];

    // gravity variables
    float gravity = -9.8f;
    float groundedGravity = -.05f;

    // jumping variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    [SerializeField] float maxJumpHeight = 2.0f;
    [SerializeField] float maxJumpTime = 0.75f;
    [SerializeField] float runSpeed = 3f;
    bool isJumping = false;
    int isJumpingHash;
    int jumpCountHash;
    bool isJumpAnimating = false;
    int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();
    Coroutine currentJumpResetRoutine = null;
    Coroutine jumpBuffer = null;
    // Awake is called earlier than Start in Unity's event life cycle
    void Awake()
    {
        // initially set reference variables
        playerInput = new CharacterInputController();
        characterController = GetComponent<CharacterController>();
        childrenPlayer_ = GetComponentInChildren<Player>();

        // set the player input callbacks
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;

        gravity  = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
        initialJumpVelocity = (2 * (maxJumpHeight)) / (timeToApex * 1.25f);

        float secondJumpGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        float secondJumpInitialVelocity = (2 * (maxJumpHeight)) / timeToApex;


        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);

        jumpGravities.Add(0, gravity);
        jumpGravities.Add(1, gravity);
        jumpGravities.Add(2, secondJumpGravity);
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed) 
            //|| !isJumping && characterController.isGrounded && buffer[0] == "space")
        {
            if (jumpCount < 2 && currentJumpResetRoutine != null)
            {
                StopCoroutine(currentJumpResetRoutine);
                //StopCoroutine(jumpBuffer);
            }
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1;
            try
            {
                //buffer[0] = "";
                currentMovement.y = initialJumpVelocities[jumpCount];
                appliedMovement.y = initialJumpVelocities[jumpCount];
            }
            catch
            {
                jumpCount = 0;
            }
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    IEnumerator jumpResetRoutine()
    {
        yield return new WaitForSeconds(.5f);
        jumpCount = 0;
    }

    IEnumerator BufferJump()
    {
        yield return new WaitForSeconds(.1f);
        buffer[0] = "";
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        //buffer[0] = "space";
        //if(jumpBuffer == null)
        //{
        //    jumpBuffer = StartCoroutine(BufferJump());
        //}       
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    // handler function to set the player input values
    void onMovementInput(InputAction.CallbackContext context) // To do buffer jump
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        // apply proper gravity if the player is grounded or not
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                isJumpAnimating = false;
                currentJumpResetRoutine = StartCoroutine(jumpResetRoutine());
                if (jumpCount == 2)
                {
                    jumpCount = 0;
                }
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;

            // additional gravity applied after reaching apex of jump
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f, -20.0f);

            // applied when character is not grounded
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * .5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        appliedMovement.x = currentMovement.x * runSpeed;
        appliedMovement.z = currentMovement.z * runSpeed;


        characterController.Move(appliedMovement * Time.deltaTime);
        handleJump();

   
        childrenPlayer_.setPos(this.transform.position.x, this.transform.position.y, this.transform.position.z + this.transform.position.y);
  
        
    }

    void FixedUpdate()
    {
        handleGravity();
    }

    void OnEnable()
    {
        // enable the character controls action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // disable the character controls action map
        playerInput.CharacterControls.Disable();
    }

    private void isGroundedCheck()
    {
        Vector3 raycastOffset = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        RaycastHit hit;
        isGrounded_ = false;
        if (Physics.Raycast(raycastOffset, new Vector3(0f, -1f, 0f), out hit, 1.001f))
        {
            isGrounded_ = true;
        }
    }
}
