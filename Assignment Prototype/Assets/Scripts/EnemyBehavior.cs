using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private float health = 50;
    [SerializeField] private bool dead = false;
    public int minCoin;
    public int maxCoin;
    public GameObject coin;
    public Transform coinSpawner;
    public float coinLaunchForce;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CapsuleCollider parentCollider;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Rigidbody[] rigidbodies;

    private void Start()
    {
        anim = GetComponent<Animator>();
        parentCollider = GetComponent<CapsuleCollider>();
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        SetCollidersState(false);
        SetRigidbodiesState(true);
    }

    public void TakeDamage(float damage)
    {
        if (dead)
            return;

        health -= damage;
        if (health <= 0)
            EnemyDeath();
        else
        {
            anim.SetTrigger("Hit");
        }
    }

    public void EnemyDeath()
    {
        dead = true;
        anim.enabled = false;
        SetCollidersState(true);
        SetRigidbodiesState(false);
        DropCoins();
    }

    public void DropCoins()
    {
        int coinAmount = Random.Range(minCoin, maxCoin);
        for (int count = 1; count <= coinAmount; count++)
        {
            Vector3 coinLaunch = new Vector3(Random.Range(-coinLaunchForce, coinLaunchForce), Random.Range(-coinLaunchForce, coinLaunchForce), Random.Range(-coinLaunchForce, coinLaunchForce));

            GameObject newCoin = Instantiate(coin, coinSpawner.position, Quaternion.identity);
            Rigidbody coinRB = newCoin.GetComponent<Rigidbody>();
            coinRB.AddForceAtPosition(coinLaunch, coinSpawner.position, ForceMode.Impulse);
            StartCoroutine(newCoin.GetComponent<CoinBehavior>().FollowDelay());
        }
    }


    public void SetRigidbodiesState(bool state)
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
    }

    public void SetCollidersState(bool state)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        parentCollider.enabled = !state;
    }
}
