using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth;

    public SwordBehavior sword;
    public Transform hitVfxLocation;
    public GameObject hitVFX;
    public PlayerMovement playerMovement;
    public CameraHandler camHandler;
    public PostProcessing healthVolume;

    [SerializeField] public float currentHealth;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Uncomment to test taking damage
        /*if (Input.GetMouseButtonDown(2))
        {
            TakeDamage(20);
        }*/
    }

    public void PlayerHeal()
    {

    }

    public void TakeDamage(float damage)
    {
        if ((sword.anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking") || sword.anim.GetCurrentAnimatorStateInfo(0).IsName("Blocked")))
        {
            Debug.Log("BLOCKED");
            sword.BlockSuccess();
            StartCoroutine(camHandler.CameraShake(0.15f, 1f));
            Vector3 vfxPos = hitVfxLocation.position;
            GameObject hitEffect = Instantiate(hitVFX, vfxPos, Quaternion.identity);
            Destroy(hitEffect, 2);
            return;
        }
        else
        {
            Debug.Log("TOOK " + damage + " DAMAGE");
            currentHealth -= damage;
            healthVolume.AddWeight(1);
            healthVolume.Invoke(nameof(healthVolume.RestoreWeight), 0.5f);
            StartCoroutine(camHandler.CameraShake(0.5f, 1f));

            if (currentHealth <= 0)
                Debug.Log("GAME OVER");
        }  
    }
}
