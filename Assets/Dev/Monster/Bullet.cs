using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 5f;

        Invoke("DestorySelf", 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            GameObject.Destroy(gameObject);
    }

    private void DestorySelf()
    {
        GameObject.Destroy(gameObject);
    }
}
