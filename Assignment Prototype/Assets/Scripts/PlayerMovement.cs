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
    public float groundDrag;
    public float airDrag;
    public float groundCheckRadius;
    public bool chargeAttacking;
    private bool grounded;
    
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Transform orientation;
    public SwordBehavior sword;
    private Rigidbody rb;
    private Collider dashTrigger;

    float xInput, yInput;

    Vector3 moveDirection;

    private void Start()
    {
        ResetJump();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        dashTrigger = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (chargeAttacking)
            return;

        GroundCheck();
        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        if (chargeAttacking)
            return;
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
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
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

    public IEnumerator ChargeAttackMovement(float chargeSpeed, float chargedTime, float chargeDuration)
    {
        chargeAttacking = true;
        Physics.IgnoreLayerCollision(8, 10, true);
        rb.drag = 0;
        rb.velocity = Camera.main.transform.forward * chargeSpeed;
        yield return new WaitForSeconds(chargeDuration);
        chargeAttacking = false;
        rb.velocity = Vector3.zero;
        Physics.IgnoreLayerCollision(8, 10, false);
    }

    private void OnTriggerEnter(Collider hitTarget)
    {
        if (chargeAttacking)
            if (hitTarget.gameObject.tag == "Enemy")
                sword.ChargeAttackHit(hitTarget);
    }
}
