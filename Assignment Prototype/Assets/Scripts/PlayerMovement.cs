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
    public float lowStaminaWarningThreshold;
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
    public PostProcessing speedVolume;
    public PostProcessing lowStaminaVolume;

    public float currentStamina;
    private bool maxReached = false;
    [SerializeField] private float staminaRegenTimer;
    private float xInput, yInput;
    private Vector3 moveDirection;

    private void Start()
    {
        ResetJump();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        dashTrigger = GetComponent<BoxCollider>();
        dashTrigger.enabled = false;
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
                speedVolume.AddWeight(0.25f); 
            }
            else
            {
                moveSpeed = walkSpeed;
                if (speedVolume.currentWeight > 0)
                    speedVolume.RestoreWeight();                    
            }
        }
        else
        {
            moveSpeed = walkSpeed;
            if (speedVolume.currentWeight > 0)
                speedVolume.RestoreWeight();
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

        if (currentStamina <= lowStaminaWarningThreshold)
        {
            lowStaminaVolume.defaultWeight = 0.5f;
            if (lowStaminaVolume.currentWeight < 0.95 && !maxReached)
            {
                lowStaminaVolume.AddWeight(1f);
            }
            else if (lowStaminaVolume.currentWeight >= 0.95 && !maxReached)
            {
                maxReached = true;
                lowStaminaVolume.RestoreWeight();
            }
            else if (lowStaminaVolume.currentWeight < 0.55 && maxReached)
            {
                maxReached = false;
            }
        }
        else
        {
            maxReached = false;
            lowStaminaVolume.defaultWeight = 0f;
            if (lowStaminaVolume.currentWeight > 0f)
                lowStaminaVolume.RestoreWeight();
        }
    }

    public void ConsumeStamina(float staminaConsumed)
    {
        currentStamina -= staminaConsumed;
        staminaRegenTimer = 0f;
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
        dashTrigger.enabled = true;
        Physics.IgnoreLayerCollision(8, 10, true); //Ignore enemy collision
        Physics.IgnoreLayerCollision(8, 9, true); //Ignore coin collision
        rb.drag = 0;
        rb.velocity = mainCam.transform.forward * chargeSpeed;
        yield return new WaitForSeconds(chargeDuration);
        chargeAttacking = false;
        dashTrigger.enabled = false;
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
