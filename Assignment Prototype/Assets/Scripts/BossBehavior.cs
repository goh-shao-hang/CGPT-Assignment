using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public float wakeCD = 5f;
    public float fireballDamage;

    public Collider bossFightTrigger;
    public Animator anim;
    public Animator shieldAnim;

    public EnemyBehavior thisEnemy;
    public phases currentPhase;

    private Coroutine wakeCoroutine;
    private bool battleStarted = false;

    [Header("Fireball")]
    public float throwForce;

    public Transform bossHand;
    public Transform player;
    public GameObject fireballPrefab;
    private GameObject fireball;

    public enum phases
    {
        phase1,
        phase2,
        phase3
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = phases.phase1;
        thisEnemy = GetComponent<EnemyBehavior>();
        bossFightTrigger = GetComponent<BoxCollider>();
        bossFightTrigger.enabled = true;
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        if ((thisEnemy.currentHealth <= thisEnemy.maxHealth * 2 / 3) && (thisEnemy.currentHealth > thisEnemy.maxHealth * 1 / 3) && currentPhase == phases.phase1)
        {
            currentPhase = phases.phase2;
            StopCoroutine(wakeCoroutine);
            anim.SetTrigger("EnableShield");
            anim.SetFloat("Phase", 2);
        }
        else if ((thisEnemy.currentHealth <= thisEnemy.maxHealth * 1 / 3) && currentPhase == phases.phase2)
        {
            currentPhase = phases.phase3;
            StopCoroutine(wakeCoroutine);
            anim.SetTrigger("EnableShield");
            anim.SetFloat("Phase", 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (battleStarted)
            return;

        if (other.gameObject.tag == "Player")
        {
            battleStarted = true;
            anim.SetTrigger("EnableShield");
            bossFightTrigger.enabled = false;
        }
    }

    public void CreateShield()
    {
        shieldAnim.Play("Create");
    }

    public void RemoveShield()
    {
        anim.ResetTrigger("EnableShield");
        shieldAnim.Play("Dissolve");
        wakeCoroutine = StartCoroutine(WakeUp());
    }

    public void CreateFireball()
    {
        float random = 1;
        switch (currentPhase)
        {
            case phases.phase1:
                random = Random.Range(1, 2);
                break;
            case phases.phase2:
                random = Random.Range(2, 4);
                break;
            case phases.phase3:
                random = Random.Range(4, 7);
                break;
            default:
                break;
        }
        fireball = Instantiate(fireballPrefab, bossHand, false);
        fireball.GetComponent<Fireball>().fireballDamage = fireballDamage;
        fireball.transform.localScale *= random;
        foreach (Transform child in fireball.transform)
        {
            child.transform.localScale = new Vector3(random, random, random);
        }
            
    }

    public void ThrowFireball()
    {
        if (fireball == null)
            return;

        fireball.transform.parent = null;
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        Vector3 force = (player.transform.position - fireball.transform.position).normalized * throwForce;
        rb.AddForce(force, ForceMode.Impulse);
    }

    public IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(wakeCD);
        anim.SetTrigger("EnableShield");
    }
}
