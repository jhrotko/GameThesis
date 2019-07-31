using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystaAnim : MonoBehaviour
{
    private float angle = 1.0f;
    public float HPGain = 10.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.RotateAround(transform.position, transform.up, angle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject, 0.5f);
        }
    }

    public void BounceForAWhile()
    {
        StartCoroutine(DisableRigidBody());
    }

    private IEnumerator DisableRigidBody()
    {
        yield return new WaitForSeconds(2.0f);
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}
