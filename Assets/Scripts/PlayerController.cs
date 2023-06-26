using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 9f;
    public float jumpVelocity = 140f;
    public float jumpCooldown = 0.4f;
    public bool isJumping = false;
    private float jumpCooldownTimer = 0f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);

        // Jumping
        if (Input.GetButtonDown("Jump") && !isJumping && jumpCooldownTimer <= 0f)
        {
            jump();

        }

        // Jump cooldown timer
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }
    
    public void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        isJumping = true;
        jumpCooldownTimer = jumpCooldown;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            isJumping = false;
        }
    }

}
