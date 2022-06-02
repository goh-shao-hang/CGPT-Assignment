using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth;
    public float blockingStaminaConsumption;

    public SwordBehavior sword;
    public PlayerMovement playerMovement;

    [SerializeField] private float currentHealth;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            TakeDamage(20);
        }
    }

    public void PlayerHeal()
    {

    }

    public void TakeDamage(float damage)
    {
        if (sword.isBlocking)
        {
            Debug.Log("BLOCKED");
            playerMovement.ConsumeStamina(blockingStaminaConsumption);
            return;
        }
        else
        {
            Debug.Log("TOOK " + damage + " DAMAGE");
            currentHealth -= damage;
            if (currentHealth <= 0)
                Debug.Log("GAME OVER");
        }  
    }
}
