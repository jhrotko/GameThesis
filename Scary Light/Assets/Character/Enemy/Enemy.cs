using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingBeing {

    private enum State {HIT, ATTACK, IDLE, RUN, STUN};
    
    private const float NEARANGLE = -1.0f; //between -1 and 1 
    private const float NEARDISTANCE = 15.0f;
    private const float CLOSEDISTANCE = 5.0f;
    private const float CLOSEANGLE = 0.8f;
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

    [SerializeField] private GameObject StunedStars;
    [SerializeField] private GameObject CrystalHeart;


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
            bool near = IsOtherClose(NEARANGLE, NEARDISTANCE, player);
            bool close = IsOtherClose(CLOSEANGLE, CLOSEDISTANCE, player);
            bool withinRange = IsOtherClose(-1.0f, CLOSEDISTANCE + 15.0f, player);

            if (!IsPlayerDead())
            {
                if (stuned)
                {
                    ChangeState(State.STUN);
                }
                else if (close)
                {
                    ChangeState(State.ATTACK);
                }
                else if ((near && !attacked) || (withinRange && attacked))
                {
                    ChangeState(State.RUN);
                }
                else if (hit)
                {
                    ChangeState(State.HIT);
                }
                else
                {
                    ChangeState(State.IDLE);
                }
            } else
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
        StartCoroutine(CrystalSpawn());
    }

    private IEnumerator CrystalSpawn()
    {
        yield return new WaitForSeconds(4.9f);

        CrystalHeart.transform.Translate(0.0f, 4.0f, 0.0f);
        CrystalHeart.transform.parent = null;
        CrystalHeart.SetActive(true);
        CrystalHeart.GetComponent<CrystaAnim>().BounceForAWhile();
    }


    private void ActivateTrigger(string triggerName)
    {
        string[] triggers = { "IdleState", "AttackState", "Hit", "Run"};

        for(int i = 0; i < triggers.Length; i++)
        {
            if(!triggers[i].Equals(triggerName))
                anim.ResetTrigger(triggers[i]);
        }
        anim.SetTrigger(triggerName);
    }

    private void ChangeState(State stateName)
    {
        if(!dieing)
        {
            switch (stateName)
            {
                case State.IDLE:
                    ActivateTrigger("IdleState");
                    nav.isStopped = true;
                    break;
                case State.ATTACK:
                    ActivateTrigger("AttackState");
                    nav.isStopped = true;
                    rb.isKinematic = false;
                    break;
                case State.HIT:
                    ActivateTrigger("Hit");
                    attacked = true;
                    hit = false;
                    nav.isStopped = true;
                    break;
                case State.RUN:
                    ActivateTrigger("Run");
                    nav.isStopped = false;
                    nav.SetDestination(player.transform.position);
                    break;
                case State.STUN:
                    anim.Play("Stuned");
                    nav.isStopped = true;
                    StartCoroutine(Stuned());
                    break;
            }
        }
    }
    
    private IEnumerator Stuned()
    {
        StunedStars.SetActive(true);
        yield return new WaitForSeconds(5);
        StunedStars.SetActive(false);
        stuned = false;
        ChangeState(State.RUN);
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
                Debug.Log("damage received " + weapon.damage);
                UpdateLife(weapon.damage);
                weapon.damage = 0.0f;
                hit = true;
            }
        }
    }

    public void TakeDamage()
    {
        if (IsOtherClose(NEARANGLE, CLOSEDISTANCE, player))
            playerScript.UpdateLife(damage);
    }

    public void ReceiveDamage(float damage)
    {
        hit = true;
        UpdateLife(damage);
    }
}