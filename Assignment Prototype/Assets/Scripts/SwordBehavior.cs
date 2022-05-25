using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    public float attackCD = 0.5f;
    public float swordDamage = 10f;
    public float chargeDamage = 20f;
    public float minChargeTime;
    public float maxChargeTime;
    public float chargeSpeed;
    public float chargeDuration;
    public Transform hitVfxLocation;
    [HideInInspector] public BoxCollider swordCollider;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool canAttack = true;
    public PlayerMovement playerMovement;
    public ParticleSystem swordTrail;
    public GameObject hitVFX;
    public CameraHandler camHandler;

    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool nextAttackReady = false;
    [SerializeField] private bool charging = false;
    [SerializeField] private float currentChargeTime = 0f;

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
            currentChargeTime = 0;
            anim.SetBool("Charging", false);
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
        else if (canAttack)
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
            isAttacking = false;
    }

    public void ChargingAttack()
    {
        //charging = true;
        if (currentChargeTime >= 0.3f)
        {
            Debug.Log("charging");
            anim.SetBool("Charging", true);
        }
        if (currentChargeTime <= maxChargeTime)
        {
            currentChargeTime += Time.deltaTime;
        }
            
    }

    public void ChargeAttack(float chargedTime)
    {
        swordTrail.Play();
        anim.SetBool("ChargeAttacking", true);
        StartCoroutine(playerMovement.ChargeAttackMovement(chargeSpeed, chargeDuration));
        Debug.Log("Charge Attacked!");
        Invoke(nameof(EndChargeAttack), chargeDuration);
        //charging = false;
    }

    public void EndChargeAttack()
    {
        anim.SetBool("ChargeAttacking", false);
    }

    public void ChargeAttackHit(Collider hitTarget)
    {
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

    IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(attackCD);
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
