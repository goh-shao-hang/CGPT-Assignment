using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float fireballDamage;
    public PlayerHealth player;
    private bool dealtDamage = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dealtDamage)
            return;

        if (other.gameObject.tag == "Player")
        {
            dealtDamage = true;
            player.GetComponent<PlayerHealth>().TakeDamage(fireballDamage);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Pillars")
        {
            Destroy(gameObject);
        }
    }
}
