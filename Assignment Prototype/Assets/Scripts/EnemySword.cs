using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    public PlayerHealth player;
    public EnemyBehavior thisEnemy;
    public EnemyAI thisAI;
    private float swordDamage;
    private bool dealtDamage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerHealth>();
        thisEnemy = GetComponentInParent<EnemyBehavior>();
        thisAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (thisEnemy.dead)
            return;

        if (other.gameObject.tag == "Player")
        {
            if (!dealtDamage)
            {
                player.GetComponent<PlayerHealth>().TakeDamage(swordDamage);
                dealtDamage = true;
                Invoke(nameof(CanDealDamage), 0.5f);
            }
            
        }
    }

    public void CanDealDamage()
    {
        dealtDamage = false;
    }
}
