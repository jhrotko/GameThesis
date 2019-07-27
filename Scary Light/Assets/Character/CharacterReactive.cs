using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//public class CharacterReactive : NetworkBehaviour {
public class CharacterReactive : MonoBehaviour
{

        private Animator characterAnim;
    private GameObject mainCam;

    private float speed = 5.0f;
    private float rotationSpeed = 45.0f;

    public float life = 1.0f;
    public Image lifeBar;

    private void Start()
    {
        lifeBar.fillAmount = 1.0f;
        characterAnim = GetComponent<Animator>();
        //mainCam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update () {
        //if (!isLocalPlayer)
        //{
        //    return;
        //}

        lifeBar.fillAmount = life;
        characterMove();
	}

    private void characterMove()
    {
        //Get Keyboard Input
        float translation = Input.GetAxis("Vertical");
        float rotation = Input.GetAxis("Horizontal");

        Vector3 moveVector = new Vector3(translation, 0, rotation);
        //Blend Run and Idle
        characterAnim.SetFloat("moveSpeed", moveVector.magnitude);

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime * speed;
        rotation *= Time.deltaTime * rotationSpeed;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);


    }
}
