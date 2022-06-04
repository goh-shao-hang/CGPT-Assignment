using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float attackDamage = 20f;
    public float detectDistance = 15f;
    public float attackRange = 1f;
    public float attackCD = 0.5f;

    private bool isAttacking;

    private Transform player;
    private Animator anim;
    private NavMeshAgent agent;
    public EnemyBehavior thisEnemy;
    public BoxCollider swordCollider;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
        thisEnemy = GetComponent<EnemyBehavior>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thisEnemy.dead)
            return;

        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                agent.velocity = Vector3.zero;
                int random = Random.Range(1, 3);
                if (random == 1)
                    anim.SetTrigger("Attack1");
                else
                    anim.SetTrigger("Attack2");
            }
        }

        if ((Vector3.Distance(transform.position, player.position) > detectDistance) || isAttacking)
        {
            agent.velocity = Vector3.zero;
            anim.SetBool("Moving", false);
        }
        else
        {
            anim.SetBool("Moving", true);
        }
    }

    public void ActivateSword()
    {
        swordCollider.enabled = true;
    }

    public void DeactivateSword()
    {
        swordCollider.enabled = false;
    }

    public void FinishAttack()
    {
        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("Attack2");
        anim.ResetTrigger("Hit");
        StartCoroutine(ResetAttack());
    }

    public IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCD);
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectDistance);
    }
}
