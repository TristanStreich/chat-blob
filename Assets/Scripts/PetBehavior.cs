using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;
using Unity.Mathematics;

public class PetBehavior : MonoBehaviour
{
    public static PetBehavior PetBehav = null;

    // Movement Variables
    [Header("Movement")]
    public float minSpeed = 1f; // Minimum speed
    public float maxSpeed = 5f; // Maximum speed
    public float sitStillFrequency = 0.5f; // How frequently it sits still (0 = always, 1 = never)
    private float sitStillFreqOG; //saves the sit still timer so i can be used again
    public float minMoveTime = 1f; //min of how much it will move in one move action
    public float maxMoveTime = 3f; //max of how much it will move in one move action
    public AnimationCurve MoveCurve; //organic movement made from this
    private float currentMoveTime;
    private float currentSitTime;
    private float currentSpeed;
    private int randomDirection;
    private float EnergyLevel = 1f;

    // Mouse Interaction Variables
    [Header("Mouse Interaction")]
    public float jumpAtMouseChance = 0.5f;
    public float jumpAtMouseForce = 7f;
    public float reachForMouseChance;
    public float reachForMouseSpeed;
    public float jumpAtMouseCooldown = 3f;
    private bool mouseNearby;
    [HideInInspector]
    public bool canMove, isHeld, sitting, isLate;

    // Mouse Interaction Variables
    [Header("Idle Action")]
    private float idleTime = 0f;
    private float minIdleDuration = 5f;  // Minimum idle time before triggering an event (in seconds).

    // Jump Variables
    [Header("Jumping")]
    public float minJumpInterval = 2f;
    public float maxJumpInterval = 5f;
    public float minJumpPower = 5f;
    public float maxJumpPower = 10f;
    public float minJumpAngle, maxJumpAngle;
    private float jumpTimer;
    private bool grounded;
    public float jumpAngle = 0;
    private float jumpMagnitude = 0;
    public Transform groundCheck;
    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public float raycastDistance = 0.5f;
    public float wallJumpGroundDistance;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    RaycastHit2D leftHit;
    RaycastHit2D rightHit;
    bool naturalJump = true;
    bool CanWallBounce = true;

    [Header("Particles")]
    public GameObject ParticleBlob;

    //Other stuff oranize later
    private PetScaleScript _PetScale;
    
    private enum movementStates
    {
        Awake,
        Tired,
        Asleep,
        DeepAsleep,
    }
    private movementStates readState = movementStates.Awake;

    public Rigidbody2D[] rb;

    private void Awake()
    {
        if (PetBehav == null)
        {
            PetBehav = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    void Start()
    {
        //sets a bunch of private vars to avoid bugs
        currentMoveTime = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
        currentSpeed = 0;
        randomDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        sitStillFreqOG = sitStillFrequency;

        _PetScale = gameObject.GetComponent<PetScaleScript>();

        canMove = true; isHeld = false; sitting = false; isLate = false;
    }

    void Update()
    {

        //check for ground
        GroundCheck();
        //check for walls
        WallCheck();
        //check for walljump 
        WallJumpCheck();
        //timers and such for moving and other idle activities
        IdleMoveMovement();
        
        
        
    }


    #region movement
    void IdleMoveMovement()
    {
        
        if (canMove && !isHeld && grounded && !sitting)
        {
            #region Random Walking
            
            currentMoveTime -= Time.deltaTime;
            if (currentMoveTime >= 0f) //while moving timer is active: move
            {
                currentSpeed = MoveCurve.Evaluate(currentMoveTime) * UnityEngine.Random.Range(minSpeed, maxSpeed); ;
                foreach (Rigidbody2D rb in rb)
                {
                    rb.velocity = new Vector2(randomDirection * currentSpeed, rb.velocity.y) * EnergyLevel;
                }
            }
            else //otherwise sit still for a period and then pick a new movement amount
            {
                if (UnityEngine.Random.value < sitStillFrequency)
                {
                    sitStill();
                }
                else
                {
                    // Randomly choose -1 (left) or 1 (right)
                    sitStillFrequency = sitStillFreqOG;
                    randomDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                    currentMoveTime = Mathf.Floor(UnityEngine.Random.Range(minMoveTime, maxMoveTime + 1));
                }
            }

            #endregion

            #region Random Jumps
            jumpTimer -= Time.deltaTime;

            if (jumpTimer <= 0f && grounded)
            {
                naturalJump = true;
                jumpAngle = UnityEngine.Random.Range(minJumpAngle, maxJumpAngle);
                jumpMagnitude = UnityEngine.Random.Range(minJumpPower, maxJumpPower) * EnergyLevel;
                Jump();
                ResetJumpTimer();
                canMove = false;
                StartCoroutine(EnableMovementAfterCooldown(3));
                
            }
            #endregion
            
            #region Mouse Interaction
            // Mouse Interaction
            if (mouseNearby && UnityEngine.Random.value < jumpAtMouseChance)
            {
                JumpAtMouse();
                canMove = false;
                StartCoroutine(EnableMovementAfterCooldown(jumpAtMouseCooldown));
            }
            #endregion

            #region Idle Action Timer
            // Increment idle time if the player is not interacting.
            idleTime += Time.deltaTime;

            // Check if the idle time exceeds the minimum threshold.
            if (idleTime >= minIdleDuration && readState != movementStates.DeepAsleep) //will not proc more Idle actions if blobby is deep asleep
            {
                StartCoroutine(TriggerRandomEvent());

                idleTime = 0f;
            }
            #endregion
        }
        if (isHeld)
        {
            naturalJump = false;
            
            idleTime = 0f; // Reset idle time when the player interacts with the pet.
            if (readState != movementStates.Awake && readState != movementStates.DeepAsleep) //will not set to awake if already awake or Deep asleep
            { 
                ChangeMovementState(movementStates.Awake); 
            }

        }
    }

    void sitStill()
    {
        sitStillFrequency -= 0.1f;
        canMove = false;
        StartCoroutine(EnableMovementAfterCooldown(1));
    }
    void JumpAtMouse()
    {
        Debug.Log("Jumping at mouse");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 jumpDirection = (mousePosition - (Vector2)transform.position).normalized;
        Vector2 jumpVelocity = jumpDirection * jumpAtMouseForce * EnergyLevel ;
        foreach (Rigidbody2D rb in rb)
            rb.velocity = jumpVelocity;
        mouseNearby = false;

        StartCoroutine(SlowDownVelocityAfterJump(mousePosition));
    }

    IEnumerator SlowDownVelocityAfterJump(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            yield return null;
        }

        foreach (Rigidbody2D rb in rb)
            rb.velocity *= 0.1f; // Adjust the factor to control the slowdown speed
    }
   

    void Jump()
    {
        Vector2 jumpVector = Quaternion.Euler(0f, 0f, jumpAngle) * Vector2.up * jumpMagnitude;
        
        foreach (Rigidbody2D rb in rb)
        {
            rb.velocity = jumpVector;
        }
    }

    void ResetJumpTimer()
    {
        jumpTimer = UnityEngine.Random.Range(minJumpInterval, maxJumpInterval);
    }

    IEnumerator EnableMovementAfterCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canMove = true;
    }
    IEnumerator WallBounceCooldown(float cooldownTime)
    {
        CanWallBounce = false;
        yield return new WaitForSeconds(cooldownTime);
        CanWallBounce = true;
    }

    public void CanMove()
    {
        canMove = true;
    }

    private void GroundCheck()
    {
        //Scan for ground under body
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, raycastDistance, groundLayer);

        Vector2 verticalVelocity = new Vector2(rb[0].velocity.y, 0f);
        float verticalMagnitude = verticalVelocity.magnitude;

        // Check if the raycast hit a ground object
        if (hit.collider != null)
        {
            // GameObject is close to the ground
            grounded = true;
            
            if (rb[0].velocity.magnitude > 22 && !isHeld )
            {
                //spawns particles on fast contact with obstacles, more particles when the speed is faster.
                Instantiate(ParticleBlob, transform.position, Quaternion.identity);

                if (!naturalJump && CanWallBounce) //read for bouncing off floor when thrown
                {
                    jumpAngle = 45;
                    if (rb[0].velocity.x > 0) // right 
                    {
                        jumpAngle *= -1; //invert jump angle              
                        jumpMagnitude = rb[0].velocity.magnitude * 0.75f;

                        Jump();
                        canMove = false;
                        StartCoroutine(EnableMovementAfterCooldown(3));                        
                    }
                    if (rb[0].velocity.x < 0) // left
                    {
                        jumpMagnitude = rb[0].velocity.magnitude * 0.75f;

                        Jump();
                        canMove = false;
                        StartCoroutine(EnableMovementAfterCooldown(3));
                        
                    }
                    StartCoroutine(WallBounceCooldown(0.2f));
                }
                if (readState == movementStates.DeepAsleep) //Will wake blob from deep sleep if slammed down
                {
                    ChangeMovementState(movementStates.Awake);
                }
            } 
        }
        else
        {
            // GameObject is not close to the ground
            grounded = false;
        }
        Debug.DrawRay(groundCheck.position, Vector2.down * raycastDistance, Color.red);
    }

    private void WallCheck()
    {
        //Scan for right wall near body
        rightHit = Physics2D.Raycast(rightWallCheck.position, Vector2.right, raycastDistance, wallLayer);

        // Check if the raycast hit a ground object
        if (rightHit.collider != null)
        {
            // alter directional input to turn away
            randomDirection = -1;         
        }
        Debug.DrawRay(rightWallCheck.position, Vector2.right * raycastDistance, Color.blue);

        //Scan for left wall near body
        leftHit = Physics2D.Raycast(leftWallCheck.position, Vector2.left, raycastDistance, wallLayer);

        // Check if the raycast hit a ground object
        if (leftHit.collider != null)
        {
            // alter directional input to turn away
            randomDirection = 1;
            
        }
        Debug.DrawRay(leftWallCheck.position, Vector2.left * raycastDistance, Color.green);
    }

    private void WallJumpCheck()
    {
        int wallBounceMag;
        if (readState == movementStates.DeepAsleep) //requries the magnitude for wallbounce to be higher when sleeping deeply
        {
            wallBounceMag = 10;
        }
        else wallBounceMag = 3;

            // Check if the raycast is away from ground and touching right wall;
            if (rightHit.collider != null && !isHeld && rb[0].velocity.magnitude > wallBounceMag && CanWallBounce)
        {
            // GameObject is close to wall
            jumpAngle = 45;
            jumpMagnitude = rb[0].velocity.magnitude * 0.9f;           
            Jump();
            StartCoroutine(WallBounceCooldown(0.2f));
            Instantiate(ParticleBlob, transform.position, Quaternion.identity);
            if (readState == movementStates.DeepAsleep) //Will wake blob from deep sleep if slammed down
            {
                ChangeMovementState(movementStates.Awake);
            }
        }
        // Check if the raycast is away from ground and touching right wall;
        if (leftHit.collider != null && !isHeld && rb[0].velocity.magnitude > wallBounceMag && CanWallBounce)
        {
            // GameObject is close to  wall
            jumpAngle = -45;
            jumpMagnitude = rb[0].velocity.magnitude * 0.9f;          
            Jump();
            StartCoroutine(WallBounceCooldown(0.2f));
            Instantiate(ParticleBlob, transform.position, Quaternion.identity);
            if (readState == movementStates.DeepAsleep) //Will wake blob from deep sleep if slammed down
            {
                ChangeMovementState(movementStates.Awake);
            }
        }

    }

    #endregion

    #region IdleActions
    private void DoRandomEvent()
    {
        int randomEvent = 0;

        //check if sleepy
        ReadWorlTime();
        //if its late make 50% of all idle actions be "get tired / sleep"
        if (isLate || readState == movementStates.Tired) 
        {
            int coinFlipSleepyAction = UnityEngine.Random.Range(0, 2);
            if (coinFlipSleepyAction == 0)
            {
                randomEvent = 2; //sleepy event
            }
            else
            {
                randomEvent = UnityEngine.Random.Range(1, 6); // ranomly pick an event
            }
        }
        else
        {
            randomEvent = UnityEngine.Random.Range(1, 6); // ranomly pick an event
        }
        switch (randomEvent)
        {
            case 1:
                Debug.Log("Blob is dancing!");
                // Add dancing logic here.
                break;
            case 2:
                Debug.Log("Blob is sleepy");

                if (readState == movementStates.Awake)
                {
                    ChangeMovementState(movementStates.Tired);
                    
                }
                
                else if (readState == movementStates.Tired)
                {
                    ChangeMovementState(movementStates.Asleep);
                    
                }

                else if (readState == movementStates.Asleep)
                {
                    ChangeMovementState(movementStates.DeepAsleep);
                    
                }
                break;
            case 3:
                Debug.Log("Blob is playing!");
                // Add playing logic here.
                break;
            case 4:
                Debug.Log("Blob is curious");
                // Add curious logic here.
                break;
            case 5:
                Debug.Log("Blob is imitating shapes");
                // Add imitating logic here.
                break;

        }
    }

    private IEnumerator TriggerRandomEvent()
    {
        float randomIdleDuration = UnityEngine.Random.Range(0, minIdleDuration); // wait between 0 and 60 sec after after being idle for a minute
        yield return new WaitForSeconds(randomIdleDuration);

        DoRandomEvent();

    }
    #endregion

    private void ReadWorlTime()
    {
        DateTime currentTime = DateTime.Now;

        // Check if the current time is past 8:00 PM
        if (currentTime.Hour >= 20)
        {
            isLate = true;
        }
        else 
        { 
            isLate = false;
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FoodCheck(collision);
    }

    private void FoodCheck(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {

            _PetScale.Grow();

            // You can destroy the food object if needed.
            Destroy(collision.gameObject);
            Cursor.visible = true;
        }
        if (collision.gameObject.CompareTag("ShrinkFood"))
        {

            _PetScale.Shrink();

            // You can destroy the food object if needed.
            Destroy(collision.gameObject);
            Cursor.visible = true;
        }
    }

    private void ChangeMovementState(movementStates currentState)
    {
        readState = currentState;
        switch (currentState)
        {
            case movementStates.Awake:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Happy);
                EnergyLevel = 1f;
                break;
            case movementStates.Tired:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Tired);
                EnergyLevel = 0.5f;
                break;
            case movementStates.Asleep:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Asleep);
                EnergyLevel = 0f;              
                break;
            case movementStates.DeepAsleep:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Asleep);
                EnergyLevel = 0f;
                break;


        }

    }

    private void GptEventListener(GptEvent e) {
        switch (e) {
            case GptEvent.RequestSent _:
                canMove = false;
                break;
            case GptEvent.ResponseRecieved _:
                canMove = true;
                break;
        }
    }
    public void _ToggleSit()
    {
        if (sitting)
        {
            sitting = false;
        }   
        else sitting = true;
    }
}

    

