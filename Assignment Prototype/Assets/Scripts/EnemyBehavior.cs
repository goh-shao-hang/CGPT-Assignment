using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyBehavior : MonoBehaviour
{
    public float maxHealth = 40f;
    public float currentHealth;
    public bool dead = false;
    public int minCoin;
    public int maxCoin;
    public GameObject coin;
    public Transform player;
    public Transform coinSpawner;
    public float coinLaunchForce;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Collider[] parentColliders;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Rigidbody[] rigidbodies;

    private void Start()
    {
        anim = GetComponent<Animator>();
        parentColliders = GetComponents<Collider>();
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        SetCollidersState(false);
        SetRigidbodiesState(true);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!dead)
        {
            Vector3 targetDir = player.position - transform.position;
            targetDir.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (dead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
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
        if (GetComponent<NavMeshAgent>() != null)
            Destroy(GetComponent<NavMeshAgent>());
        SetCollidersState(true);
        SetRigidbodiesState(false);
        DropCoins();

        if (gameObject.name == "Boss")
            Invoke(nameof(BossDeath), 3f);
    }

    public void BossDeath()
    {
        Debug.Log("loaded");
        SceneManager.LoadScene(3);
    }

    public void DropCoins()
    {
        int coinAmount = Random.Range(minCoin, maxCoin + 1);
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

        foreach (Collider collider in parentColliders)
        {
            collider.enabled = !state;
        }
    }
}
