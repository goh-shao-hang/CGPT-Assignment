using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{

    public Transform coinCollector;
    public float followDamp = 30f;

    private Rigidbody rb;
    private Vector3 coinVelocity = Vector3.zero;
    private bool isFollowing;
    private bool isDestroyed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coinCollector = GameObject.FindGameObjectWithTag("CoinCollector").transform;
    }

    public IEnumerator FollowDelay()
    {
        yield return new WaitForSeconds(1f);
        isFollowing = true;
        if (!isDestroyed)
            rb.isKinematic = false;    
    }

    private void Update()
    {
        if (isFollowing)
            transform.position = Vector3.SmoothDamp(transform.position, coinCollector.position, ref coinVelocity, Time.deltaTime * followDamp);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            isDestroyed = true;
            Debug.Log("COIN + 1");
        }       
    }
}
