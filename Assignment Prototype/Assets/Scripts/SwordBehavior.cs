using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SwordBehavior : MonoBehaviour
{
    [Header("Parameters")]
    public float swordDamage;
    public float minChargeTime;
    public float maxChargeTime;
    public float baseChargeDamage;
    public float extraChargeDamage;
    public float baseChargeSpeed;
    public float extraChargeSpeed;
    public float chargeDuration;
    public float blockingStaminaConsumption;
    public float maxPostProcessWeight;

    [Header("Assignables")]
    public Transform hitVfxLocation;
    public Transform chargeVFXLocation;
    [HideInInspector] public BoxCollider swordCollider;
    [HideInInspector] public Animator anim;
    [HideInInspector] public MeshRenderer mr;
    public PlayerMovement playerMovement;
    public VisualEffect chargingVFX;
    public Material swordMat;
    public Material glowingMat;
    public PostProcessing speedVolume;
    public ParticleSystem swordTrail;
    public GameObject hitVFX;
    public GameObject fullyChargedVFX;
    public CameraHandler camHandler;

    private bool canAttack = true;
    private bool isAttacking = false;
    private bool nextAttackReady = false;
    private bool chargingVFXPlaying = false;
    private bool fullyChargedVFXPlayed = false;
    private float currentChargeTime = 0f;
    private float storedChargeTime = 0f;
    [HideInInspector] public bool isBlocking;

    private void Start()
    {
        swordCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        mr = GetComponent<MeshRenderer>();
        swordCollider.enabled = false;
    }

    private void Update()
    {
        /* uncomment to test player getting hit
        if (Input.GetMouseButtonDown(2))
            anim.SetTrigger("Blocked"); */

        if (Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }
        if (Input.GetMouseButton(0))
        {
            ChargingAttack();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (currentChargeTime >= minChargeTime)
                ChargeAttack(currentChargeTime);
            else
            {
                anim.SetBool("Charging", false);
            }     

            currentChargeTime = 0;
            chargingVFX.Stop();
            chargingVFXPlaying = false;

            if (fullyChargedVFXPlayed)
                fullyChargedVFXPlayed = false;
        }
        if (Input.GetMouseButton(1))
        {
            StartBlocking();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopBlocking();
        }
    }

    public void StartAttack()
    {
        if (!isAttacking)
        {
            anim.SetTrigger("Attack");
            canAttack = false;
            isAttacking = true;
        }
        if (canAttack)
            nextAttackReady = true;
    }

    public void AllowNextAttackInput()
    {
        canAttack = true;
    }

    public void CheckForContinueAttack()
    {
        if (nextAttackReady)
        {
            anim.SetTrigger("Attack");
            canAttack = false;
            nextAttackReady = false;
        }
        else
        {
            isAttacking = false;
            anim.SetTrigger("StopAttack");
        }
    }

    public void ChargingAttack()
    {    
        anim.ResetTrigger("StopAttack");
        if (currentChargeTime >= 0.3f)
        {
            anim.SetBool("Charging", true);
        }
        if (currentChargeTime > minChargeTime)
        {
            if (!chargingVFXPlaying)
            {
                chargingVFX.Play();
                chargingVFXPlaying = true;
            }
        }
        if (currentChargeTime < maxChargeTime)
        {
            currentChargeTime += Time.deltaTime;
        }
        else
        {
            anim.SetTrigger("FullyCharged");
            chargingVFX.Stop();
            chargingVFXPlaying = false;
            mr.material = glowingMat;
            if (!fullyChargedVFXPlayed)
            {
                fullyChargedVFXPlayed = true;
                Instantiate(fullyChargedVFX, chargeVFXLocation);
            }
                fullyChargedVFXPlayed = true;
        }
            
    }

    public void ChargeAttack(float chargedTime)
    {
        swordTrail.Play();
        anim.SetBool("ChargeAttacking", true);
        anim.SetBool("Charging", false);
        anim.ResetTrigger("FullyCharged");
        storedChargeTime = chargedTime;

        float targetWeight = 1 * ((storedChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
        speedVolume.AddWeight(targetWeight);

        float chargeSpeed = baseChargeSpeed + extraChargeSpeed * ((storedChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
        chargeSpeed = Mathf.Round(chargeSpeed);

        StartCoroutine(playerMovement.ChargeAttackMovement(chargeSpeed, chargedTime, chargeDuration));
        Invoke(nameof(EndChargeAttack), chargeDuration);
    }

    public void StopCharging()
    {
        currentChargeTime = 0;
        chargingVFX.Stop();
        if (mr.material != swordMat)
            mr.material = swordMat;
        fullyChargedVFXPlayed = false;
        chargingVFXPlaying = false;
    }

    public void EndChargeAttack()
    {
        anim.SetBool("ChargeAttacking", false);
        speedVolume.RestoreWeight();
        storedChargeTime = 0f;
        if (mr.material != swordMat)
            mr.material = swordMat;
    }

    public void ChargeAttackHit(Collider hitTarget)
    {
        float chargeDamage = baseChargeDamage + extraChargeDamage * ((storedChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
        chargeDamage = Mathf.Round(chargeDamage);
        Vector3 vfxPos = hitVfxLocation.position;
        hitTarget.GetComponent<EnemyBehavior>().TakeDamage(chargeDamage);
        GameObject hitEffect = Instantiate(hitVFX, vfxPos, Quaternion.identity);
        Destroy(hitEffect, 2);
        StartCoroutine(camHandler.CameraShake(0.15f, 1f));
    }

    public void StartBlocking()
    {
        if (playerMovement.currentStamina >= blockingStaminaConsumption)
        {
            isBlocking = true;
            anim.SetBool("Blocking", true);
            anim.ResetTrigger("FullyCharged");
            StopCharging();
        }
        else
            StopBlocking();
    }

    public void StopBlocking()
    {
        anim.SetBool("Blocking", false);
        anim.ResetTrigger("Blocked");
        isBlocking = false;
    }

    public void BlockSuccess()
    {
        playerMovement.currentStamina -= blockingStaminaConsumption;
        anim.SetTrigger("Blocked");
    }

    public void ActivateCollider()
    {
        swordCollider.enabled = true;
        swordTrail.Play();
    }

    public void DeactivateCollider()
    {
        swordCollider.enabled = false;
        swordTrail.Stop();
    }   

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Boss")
        {
            Vector3 vfxPos = hitVfxLocation.position;
            other.GetComponent<EnemyBehavior>().TakeDamage(swordDamage);
            GameObject hitEffect = Instantiate(hitVFX, vfxPos, Quaternion.identity);
            Destroy(hitEffect, 2);
            StartCoroutine(camHandler.CameraShake(0.15f, 1f));
        }
    }
}
