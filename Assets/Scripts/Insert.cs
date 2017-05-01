using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insert : MonoBehaviour {

	public Animator rodAnimator;


	// Use this for initialization
	void Start () {
		//rodAnimator.SetBool("insert", false);
		//rodAnimator.Play ("RodIdle");
		rodAnimator = GetComponent<Animator>();
		rodAnimator.SetBool ("insert", false);
		rodAnimator.Play ("RodIdle");
	}

	void OnTriggerEnter (Collider target) {

		if (target.gameObject.tag == "Player") {
			rodAnimator.SetBool ("insert", true);
			rodAnimator.Play ("RodInsert");
		}

//
//		//if it comes into contact with the Generator's area
//		if (target.gameObject.tag == "Generator") {
//
//			Debug.Log("Inserting rod");
//
//			//detach rod from character
//			//GetComponent<PickUp>().dropObject ();
//			//insert rod
//			rodAnimator.SetBool("insert", true);
//			rodAnimator.Play ("RodInsert");
//		}
	}

	void OnTriggerExit(Collider target) {
		
		if (target.gameObject.tag == "Player") {
			DestroyObject (this.gameObject);
		}

	}

	// Update is called once per frame
	void Update () {

	}

}
