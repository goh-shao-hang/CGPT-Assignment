using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    public float swordDamage;
    public float minChargeTime;
    public float maxChargeTime;
    public float baseChargeDamage;
    public float extraChargeDamage;
    public float baseChargeSpeed;
    public float extraChargeSpeed;
    public float chargeDuration;
    public float maxLensDistortAmount;

    public Transform hitVfxLocation;
    [HideInInspector] public BoxCollider swordCollider;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool canAttack = true;
    public PlayerMovement playerMovement;
    public PostProcessing postProcessing;
    public ParticleSystem swordTrail;
    public GameObject hitVFX;
    public CameraHandler camHandler;

    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool nextAttackReady = false;
    [SerializeField] private bool charging = false;
    [SerializeField] private float currentChargeTime = 0f;
    private float storedChargeTime = 0f;

    private void Start()
    {
        swordCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        swordCollider.enabled = false;
    }

    private void Update()
    {
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
                anim.ResetTrigger("StopAttack");
            }     
            currentChargeTime = 0;
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
        if (currentChargeTime < maxChargeTime)
        {
            currentChargeTime += Time.deltaTime;
        }
        else
        {
            anim.SetTrigger("FullyCharged");
        }
            
    }

    public void ChargeAttack(float chargedTime)
    {
        swordTrail.Play();
        anim.SetBool("ChargeAttacking", true);
        anim.SetBool("Charging", false);
        anim.ResetTrigger("FullyCharged");
        storedChargeTime = chargedTime;

        float lensDistortAmount = maxLensDistortAmount * ((storedChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
        postProcessing.startLensDistort(lensDistortAmount);

        float chargeSpeed = baseChargeSpeed + extraChargeSpeed * ((storedChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
        chargeSpeed = Mathf.Round(chargeSpeed);

        StartCoroutine(playerMovement.ChargeAttackMovement(chargeSpeed, chargedTime, chargeDuration));
        Invoke(nameof(EndChargeAttack), chargeDuration);
    }

    public void EndChargeAttack()
    {
        anim.SetBool("ChargeAttacking", false);
        postProcessing.EndLensDistort();
        storedChargeTime = 0f;
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
        if (other.tag == "Enemy")
        {
            Vector3 vfxPos = hitVfxLocation.position;
            other.GetComponent<EnemyBehavior>().TakeDamage(swordDamage);
            GameObject hitEffect = Instantiate(hitVFX, vfxPos, Quaternion.identity);
            Destroy(hitEffect, 2);
            StartCoroutine(camHandler.CameraShake(0.15f, 1f));
        }
    }
}
