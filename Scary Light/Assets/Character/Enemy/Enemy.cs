using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingBeing {

    private enum State {HIT, ATTACK, IDLE, RUN, STUN};
    
    private const float NEARANGLE = 0.8f; //between -1 and 1 
    private const float NEARDISTANCE = 15.0f;
    private const float CLOSE = 4.0f;
    public float damage = 5.0f;

    public GameObject player;
    private MainReactive playerScript;
    private Animator anim;
    private NavMeshAgent nav;
    private Rigidbody rb;

    public bool attacked;
    private bool hit;
    private bool dieing;
    private bool stuned;


	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerScript = player.GetComponent<MainReactive>();
        attacked = false;
        dieing = false;
        stuned = false;
        hit = false;

        InitializeLife();
        ChangeState(State.IDLE);
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDead() && !dieing)
        {
            Die(anim);
            Death();
        }
        else {
            bool near = IsPlayerClose(NEARANGLE, NEARDISTANCE);
            bool close = IsPlayerClose(NEARANGLE, CLOSE);
            
            if(!IsPlayerDead())
            {
                if (close)
                {
                    //Debug.Log("ITS NEAR ME");
                    ChangeState(State.ATTACK);
                }
                else if ((near && !attacked) || (!close && attacked))
                {
                    //Debug.Log("ITS FAR");
                    ChangeState(State.RUN);
                }
                else if (hit)
                {
                    //Debug.Log("AUCH");
                    ChangeState(State.HIT);
                }
                if(stuned)
                {
                    Debug.Log("STUNED");
                    ChangeState(State.STUN);
                }
            }
            else
            {
                ChangeState(State.IDLE);
            }
        } 
    }

    private bool IsPlayerDead()
    {
        return player.GetComponent<LivingBeing>().dead;
    }

    private void Death()
    {
        nav.enabled = false;
        Destroy(gameObject, 5.0f);
        dieing = true;
    }

    private void ChangeState(State stateName)
    {
        if(!dieing)
        {
            switch (stateName)
            {
                case State.IDLE:
                    anim.SetTrigger("IdleState");
                    break;
                case State.ATTACK:
                    anim.SetTrigger("AttackState");
                    nav.enabled = false;
                    rb.isKinematic = false;
                    break;
                case State.HIT:
                    anim.SetTrigger("Hit");
                    attacked = true;
                    hit = false;
                    break;
                case State.RUN:
                    anim.SetTrigger("Run");
                    nav.enabled = true;
                    rb.isKinematic = true;
                    nav.SetDestination(player.transform.position);
                    break;
                case State.STUN:
                    anim.Play("Stuned");
                    StartCoroutine(Stuned());
                    break;
            }
        }
    }
    
    private IEnumerator Stuned()
    {
        nav.enabled = false;
        yield return new WaitForSeconds(5);
        stuned = false;
        nav.enabled = true;
        anim.Play("Run");
        ChangeState(State.RUN);
        yield break;
    }

    private bool IsPlayerClose(float angle, float distance)
    {
        Vector3 playerRelativePos = Vector3.Normalize(player.transform.position - transform.position);
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        //Debug.Log("distance "+playerDistance);
        return Vector3.Dot(transform.forward, playerRelativePos) >= angle && playerDistance <= distance;
    }

    public bool IsClose()
    {
        return IsPlayerClose(NEARANGLE, NEARDISTANCE);
    }

    public void GetStuned()
    {
        stuned = true;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if(!dieing)
        {
            GameObject GOCollided = collision.gameObject;

            if (GOCollided.CompareTag("Weapon"))
            {
                Weapon weapon = GOCollided.GetComponent<Weapon>();
                UpdateLife(weapon.damage);
                weapon.damage = 0.0f;
                hit = true;
            }
        }
    }

    public void TakeDamage()
    {
        if (IsPlayerClose(NEARANGLE, CLOSE))
            playerScript.UpdateLife(damage);
    }
}