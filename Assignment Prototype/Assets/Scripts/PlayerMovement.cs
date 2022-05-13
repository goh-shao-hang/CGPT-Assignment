using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool readyToJump = true;
    public float playerHeight;
    public float groundDrag;
    public float groundCheckDistance;
    private bool grounded;
    
    public LayerMask whatIsGround;
    public Transform orientation;
    private Rigidbody rb;

    float xInput, yInput;

    Vector3 moveDirection;

    private void Start()
    {
        ResetJump();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GroundCheck();
        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void MyInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Jump") && readyToJump && grounded)
        {
            Jump();
            Invoke(nameof(ResetJump), jumpCD);
        }
    }

    private void Move()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10f);

        moveDirection = orientation.forward * yInput + orientation.right * xInput;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundCheckDistance, whatIsGround);
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 velCap = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(velCap.x, rb.velocity.y, velCap.z);
        }
    }

    private void Jump()
    {
        readyToJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
