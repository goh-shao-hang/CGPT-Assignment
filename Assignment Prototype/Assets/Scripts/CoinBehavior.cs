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

    public PostProcessing coinVolume;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coinCollector = GameObject.FindGameObjectWithTag("CoinCollector").transform;
        coinVolume = GameObject.Find("Coin Effects").GetComponent<PostProcessing>();
    }

    public IEnumerator FollowDelay()
    {
        yield return new WaitForSeconds(1f);
        isFollowing = true;
        if (!isDestroyed)
            rb.isKinematic = false;    
    }

    private void FixedUpdate()
    {
        if (isFollowing)
            transform.position = Vector3.SmoothDamp(transform.position, coinCollector.position, ref coinVelocity, Time.fixedDeltaTime * followDamp + Random.Range(-0.1f, 0.1f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            isDestroyed = true;
            coinVolume.AddWeight(1);
            coinVolume.Invoke(nameof(coinVolume.RestoreWeight), 0.25f);
            Debug.Log("COIN + 1");
        }       
    }
}
