using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Parameters")]
    //Speed
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    //Stamina
    public float maxStamina;
    public float sprintStaminaConsumption;
    public float staminaRegenRate;
    public float staminaRegenDelay;
    //Jump
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool readyToJump = true;
    //Drag
    public float groundDrag;
    public float airDrag;
    public float groundCheckRadius;
    private bool grounded;
    public bool chargeAttacking;

    [Header("Assignables")]
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Transform orientation;
    public Camera mainCam;
    public SwordBehavior sword;
    private Rigidbody rb;
    private Collider dashTrigger;
    public PostProcessing postProcessing;

    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaRegenTimer;
    private float xInput, yInput;
    private Vector3 moveDirection;

    private void Start()
    {
        ResetJump();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        dashTrigger = GetComponent<BoxCollider>();
        mainCam = Camera.main;

        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (chargeAttacking)
            return;  

        GroundCheck();
        MyInput();
        SpeedControl();
        CalculateStamina();
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

        if (Input.GetKey(KeyCode.LeftShift) && (rb.velocity != Vector3.zero))
        {
            if (currentStamina > 0)
            {
                moveSpeed = sprintSpeed;
                currentStamina -= sprintStaminaConsumption * Time.deltaTime;
                staminaRegenTimer = 0f;
                postProcessing.AddWeight(0.25f); 
            }
            else
            {
                moveSpeed = walkSpeed;
                if (postProcessing.currentWeight > 0)
                    postProcessing.RestoreWeight();                    
            }
        }
        else
        {
            moveSpeed = walkSpeed;
            if (postProcessing.currentWeight > 0)
                postProcessing.RestoreWeight();
        }

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

    public void CalculateStamina()
    {
        if (currentStamina < maxStamina)
        {
            if (staminaRegenTimer > staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
            }
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
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
        Physics.IgnoreLayerCollision(8, 10, true); //Ignore enemy collision
        Physics.IgnoreLayerCollision(8, 9, true); //Ignore coin collision
        rb.drag = 0;
        rb.velocity = mainCam.transform.forward * chargeSpeed;
        yield return new WaitForSeconds(chargeDuration);
        chargeAttacking = false;
        rb.velocity = Vector3.zero;
        Physics.IgnoreLayerCollision(8, 10, false);
        Physics.IgnoreLayerCollision(8, 9, false);
    }

    private void OnTriggerEnter(Collider hitTarget)
    {
        if (chargeAttacking)
            if (hitTarget.gameObject.tag == "Enemy")
                sword.ChargeAttackHit(hitTarget);
    }
}
