using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowArrow : StateMachineBehaviour {
    public GameObject prefabArrow;
    private GameObject newArrow;
    private float prepareArrow;
    

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        newArrow = Instantiate(prefabArrow, prefabArrow.transform.position, prefabArrow.transform.rotation, prefabArrow.transform.parent);
        newArrow.SetActive(true);
        prepareArrow = 0.0f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        prepareArrow += Time.deltaTime;

        Debug.Log(prepareArrow);
        if (prepareArrow >= 0.05f)
        {
            newArrow.transform.parent = null;
            Rigidbody arrowBody = newArrow.GetComponent<Rigidbody>();
            arrowBody.useGravity = true;
            arrowBody.AddForce(-newArrow.transform.forward * 10);
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        prepareArrow = 0.0f;
        //newArrow = null;
    }
}
