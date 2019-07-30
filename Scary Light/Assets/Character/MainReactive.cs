using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainReactive : LivingBeing
{
    [SerializeField] private bool side;
    // ***************** CONSTANTS **********************
    // 
    private const float SPEED = 6.0f;
    [SerializeField] private float ROTATIONSPEED = 55.0f;
    [SerializeField] private float FORCE = 30.0f;
    [SerializeField] private float BASICDAMAGE = 2.0f;

    // 
    //***************************************************

    // ************* ANMIMATOR Variables ******************
    //
    private Animator characterAnim;
    private bool isAttacking;
    private float speed;
    //
    // ****************************************************

    [SerializeField] private GameObject prefabArrow;
    private GameObject newArrow = null;
    private bool prepareArrow;


    private Transform selectedTarget;
    private GameObject target;
    private Vector3 attackDirection;

    private GameObject head;
    private GameObject[] AllEnemies;

    [SerializeField] private GameObject Smoke;
    private const float NEARDISTANCE = 7.0f;
    private const float NEARANGLE = -1.0f;

    private bool jumping;

    private void Start()
    {
        InitializeLife();
        AllEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        characterAnim = GetComponent<Animator>();
        isAttacking = false;
        speed = SPEED;

        prepareArrow = true;
        attackDirection = transform.forward;

        head = GameObject.Find("VirtualHead").transform.Find("Head").gameObject;
        side = true;

        jumping = false;
    }

    void Update () {
        if (IsDead())
        {
            Die(characterAnim);
        } else
        {
            CharacterMove();
            SelectEnemy();
            MoveHead();
            SmokeBomb();
            Attack();
            DestroyArrowHand();
        }
    }

    private void MoveHead()
    {
        if (selectedTarget != null)
        {
            Vector3 pointingToEnemy = selectedTarget.position - transform.position;
            float withinFieldOfView = Vector3.Dot(transform.forward, pointingToEnemy);

            if(withinFieldOfView > 0.0f)
            {
                head.transform.forward = Vector3.Normalize(pointingToEnemy);
            }
        } else
        {
            head.transform.forward = transform.forward;
        }
    }

    private void SelectEnemy()
    {
        if (Input.GetMouseButtonDown(0))
        { // when button clicked... 

            RaycastHit hit; // cast a ray from mouse pointer: 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // if enemy hit... 

            
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Enemy"))
            {
                SelectTarget(hit); // and select it
            }
            else
            {
                DeselectTarget();
            }
        } else
        {
            if(selectedTarget == null )
            {
                attackDirection = transform.forward;
            }
        }
    }

    private void SelectTarget(RaycastHit hit)
    {
        selectedTarget = hit.transform;
        target = selectedTarget.gameObject;
        attackDirection = Vector3.Normalize(target.transform.position - transform.position);
    }

    private void DeselectTarget()
    { 	
        selectedTarget = null;
        attackDirection = transform.forward;
    }

    private void CharacterMove()
    {
        
        //Get Keyboard Input
        float translation = Input.GetAxis("Vertical");
        float rotation = Input.GetAxis("Horizontal");

        side = rotation >= 0;
        if (!isAttacking)
            Translate(translation);

        Rotate(rotation);
        characterAnim.SetFloat("moveSpeed", translation);
        Roll();
    }

    private void Translate(float translation)
    {
        //Sets Front or Back Animation
        IsFront(translation);

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime * speed;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);
    }

    private void SmokeBomb()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            characterAnim.Play("Smoke Bomb");
        }

        if(characterAnim.GetCurrentAnimatorStateInfo(0).IsName("Back Flip"))
        {
            // Move translation along the object's z-axis
            if(jumping)
                transform.Translate(0, 0, -Time.deltaTime * 10.0f);
        }
    }

    private void StartSmoke()
    {
        Smoke.SetActive(true);
        foreach(GameObject enem in AllEnemies)
        {
            if(IsOtherClose(NEARANGLE, NEARDISTANCE, enem))
            {
                enem.GetComponent<Enemy>().GetStuned();
            }
        }
    }

    private void StopSmoke()
    {
        Smoke.SetActive(false);
    }

    private void Roll()
    {
        if(Input.GetButtonDown("Jump") && !characterAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            characterAnim.Play("Roll");
        }
    }

    private void Rotate(float rotation)
    {       
        //Rotate character
        rotation *= Time.deltaTime * ROTATIONSPEED;
        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            characterAnim.Play("Basic Attack");
        }
        else if(Input.GetButtonDown("Fire1") || (Input.GetAxis("Vertical") != 0 && isAttacking))
        {
            isAttacking = !isAttacking;
        }

        //Turns to target
        if (selectedTarget != null && isAttacking)
            transform.forward = Vector3.Normalize(selectedTarget.position - transform.position);

        characterAnim.SetBool("isAttacking", isAttacking);

        ThrowArrow();
    }

    private void IsFront(float translation)
    {
        bool front = false;
        speed = SPEED - 2.5f;
        if (translation >= 0.0f)
        {
            front = true;
            speed = SPEED;
        }
        characterAnim.SetBool("front", front);
    }

    private void ThrowArrow()
    {
        AnimatorStateInfo animatorState = characterAnim.GetCurrentAnimatorStateInfo(0);

        if(animatorState.IsName("Start Attack"))
        {
            if(prepareArrow)
            {
                if(newArrow != null)
                    newArrow.GetComponent<BoxCollider>().enabled = true;
                    
                newArrow = Instantiate(prefabArrow, prefabArrow.transform.position, prefabArrow.transform.rotation, prefabArrow.transform.parent);
                newArrow.SetActive(true);
                prepareArrow = false;
            }
        }
        else if (animatorState.IsName("Attack"))
        {
            Rigidbody arrowRb = newArrow.GetComponent<Rigidbody>();
            arrowRb.useGravity = true;
            arrowRb.velocity = attackDirection * FORCE;
            newArrow.transform.parent = null;
            prepareArrow = true;
        } else
        {
            prepareArrow = true;
        } 
    }

    private void DestroyArrowHand()
        //Destroys arrow that wasnt thrown
    {
        AnimatorStateInfo animatorState = characterAnim.GetCurrentAnimatorStateInfo(0);

        if (!(animatorState.IsName("Start Attack") || animatorState.IsName("Attack")))
        {
            if (prepareArrow && newArrow != null)
            {
                Destroy(newArrow.gameObject, 0.9f);
            }
        }
    }

    private void BasicAttack()
    {
        foreach (GameObject enem in AllEnemies)
        {
            if (IsOtherClose(NEARANGLE, NEARDISTANCE, enem))
            {
                enem.GetComponent<Enemy>().ReceiveDamage(BASICDAMAGE);
            }
        }
    }

    private void JumpFlip()
    {
        jumping = !jumping;
    }
    
}
