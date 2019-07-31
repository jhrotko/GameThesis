using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Weapon
{
    private Rigidbody rb;
    private Animator anim;
    private const float DEATHTIME = 9.0f;
    private bool collided;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = character.GetComponent<Animator>();
        collided = false;
    }

    void Update()
    {
        AttackRotation();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.velocity != Vector3.zero)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void IncreaseDamage(float damage)
    {
        this.damage += damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Enemy"))
        {
            //Remove health or do smt here
            transform.parent = collision.transform;
            GetComponent<BoxCollider>().enabled = false;
            Destroy(gameObject, 3.0f);
        }

        if (!target.CompareTag("Player"))
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            collided = true;
        }
    }

    private void AttackRotation()
    {
        AnimatorStateInfo animatorState = anim.GetCurrentAnimatorStateInfo(0);

        if (animatorState.IsName("Attack"))
        {
            if (rb.velocity != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
}
