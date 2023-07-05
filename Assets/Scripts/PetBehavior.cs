using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PetBehavior : MonoBehaviour
{
    public static PetBehavior PetBehav = null;

    // Movement Variables
    [Header("Movement")]
    public float minSpeed = 1f; // Minimum speed
    public float maxSpeed = 5f; // Maximum speed
    public float sitStillFrequency = 0.5f; // How frequently it sits still (0 = always, 1 = never)
    private float sitStillFreqOG;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;
    public float minSitTime = 1f;
    public float maxSitTime = 3f;
    private float currentMoveTime;
    private float currentSitTime;
    private float currentSpeed;
    private int randomDirection;
    private float EnergyLevel = 1f;

    // Mouse Interaction Variables
    [Header("Mouse Interaction")]
    public float jumpAtMouseChance = 0.5f;
    public float jumpAtMouseForce = 7f;
    public float jumpAtMouseCooldown = 3f;
    private bool mouseNearby;
    [HideInInspector]
    public bool canMove = true, isHeld = false;


    // Jump Variables
    [Header("Jumping")]
    public float minJumpInterval = 2f;
    public float maxJumpInterval = 5f;
    public float minJumpPower = 5f;
    public float maxJumpPower = 10f;
    private float jumpTimer;
    private float jumpPower;

    
    private enum movementStates
    {
        Awake,
        Tired,
        Asleep,
        DeepAsleep
    }
    private movementStates currentState = movementStates.Awake;

    private Rigidbody2D rb;

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
        rb = GetComponent<Rigidbody2D>();
        currentMoveTime = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
        currentSitTime = UnityEngine.Random.Range(minSitTime,maxSitTime);
        currentSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        randomDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        sitStillFreqOG = sitStillFrequency;
}

    void Update()
    {

        ReadWorlTime();

        // Random Movement
        if (canMove && !isHeld)
        {
            currentMoveTime -= Time.deltaTime;
            if (currentMoveTime >= 0f)
            {
                MoveRandom();
            }
            else
            {
                if (UnityEngine.Random.value < sitStillFrequency)
                {
                    // Pet sits still
                    sitStill();
                }
                else
                {
                    // Randomly choose -1 (left) or 1 (right)
                    sitStillFrequency = sitStillFreqOG;
                    randomDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                    currentMoveTime = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                    currentSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
                }
            }

            // Random Jumps
            jumpTimer -= Time.deltaTime;

            if (jumpTimer <= 0f)
            {
                Jump();
                ResetJumpTimer();
                canMove = false;
                StartCoroutine(EnableMovementAfterCooldown(jumpAtMouseCooldown));
            }

            // Mouse Interaction
            if (mouseNearby && UnityEngine.Random.value < jumpAtMouseChance)
            {
                JumpAtMouse();
                canMove = false;
                StartCoroutine(EnableMovementAfterCooldown(jumpAtMouseCooldown));
            }
        }

        
    }
#region movement
    void MoveRandom()
    {
        rb.velocity = new Vector2(randomDirection * currentSpeed, rb.velocity.y) * EnergyLevel ;
    }
    void sitStill()
    {
        sitStillFrequency -= 0.1f;
        canMove = false;
        StartCoroutine(EnableMovementAfterCooldown(1));
        currentSitTime = UnityEngine.Random.Range(minSitTime, maxSitTime);
    }
    void JumpAtMouse()
    {
        Debug.Log("Jumping at mouse");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 jumpDirection = (mousePosition - (Vector2)transform.position).normalized;
        Vector2 jumpVelocity = jumpDirection * jumpAtMouseForce * EnergyLevel ;
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

        rb.velocity *= 0.1f; // Adjust the factor to control the slowdown speed
    }

    void Jump()
    {
        jumpPower = UnityEngine.Random.Range(minJumpPower, maxJumpPower) * EnergyLevel ;
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
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

    void OnMouseEnter()
    {
        mouseNearby = true;
    }

    void OnMouseExit()
    {
        mouseNearby = false;
    }

    public void CanMove()
    {
        canMove = true;
    }

    #endregion

    private void ReadWorlTime()
    {
        DateTime currentTime = DateTime.Now;

        // Check if the current time is past 8:00 PM
        if (currentTime.Hour >= 20)
        {
            if (currentState == movementStates.Awake)
            {
                currentState = movementStates.Tired;
                ChangeMovementState();
            }
        }
        else 
        { if (currentState != movementStates.Awake)
            {
                currentState = movementStates.Awake;
                ChangeMovementState();
            }
        }
    }

    private void ChangeMovementState()
    {
        switch (currentState)
        {
            case movementStates.Awake:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Happy);
                EnergyLevel = 1f;
                break;
            case movementStates.Tired:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Tired);
                EnergyLevel = 0.5f;
                float timerElapsed = 0f;
                timerElapsed += Time.deltaTime;
                Debug.Log(timerElapsed);
                if (timerElapsed >= 10)
                {
                    currentState = movementStates.Asleep;
                }
                break;
            case movementStates.Asleep:
                FaceController.FaceManager.ChangeFace(FaceController.Expressions.Asleep);
                EnergyLevel = 0f;
                float timer2Elapsed = 0f;
                timer2Elapsed += Time.deltaTime;
                if (timer2Elapsed >= 60)
                {
                    currentState = movementStates.DeepAsleep;
                }
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

}

    

