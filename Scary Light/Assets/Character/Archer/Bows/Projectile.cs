﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Weapon
{
    private Rigidbody rb;
    private Animator anim;
    private const float DEATHTIME = 9.0f;
    private bool collided;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = character.GetComponent<Animator>();
        collided = false;
        damage = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        AttackRotation();
        DestroyAfterTime(9);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.velocity != Vector3.zero)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Enemy"))
        {
            //Remove health or do smt here
            Debug.Log("I HITED AN ENEMY");
            transform.parent = collision.transform;
            GetComponent<BoxCollider>().enabled = false;
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

    private void DestroyAfterTime(float time)
    {
        if (collided)
            Destroy(this.gameObject, time);
    }
}