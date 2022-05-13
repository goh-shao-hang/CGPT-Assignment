using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    public float attackCD = 0.5f;
    public float swordDamage = 10f;
    public Transform hitVfxLocation;
    [HideInInspector] public BoxCollider swordCollider;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool canAttack = true;
    public ParticleSystem swordTrail;
    public GameObject hitVFX;
    public CameraHandler camHandler;

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
            if (canAttack)
            {
                StartAttack();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopAttack();
        }
    }

    public void StartAttack()
    {
        anim.SetBool("Attacking", true);
        canAttack = false;
    }

    public void StopAttack()
    {
        anim.SetBool("Attacking", false);
        canAttack = true;
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
            Instantiate(hitVFX, vfxPos, Quaternion.identity);
            StartCoroutine(camHandler.CameraShake(0.15f, 1f));
        }
    }
}
