﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public Animation DoorAnimations;

	// Use this for initialization
	void Start () {
		//Door Idle
		DoorAnimations.Play("DoorIdle");
	}

	void OnTriggerEnter (Collider target) {

		if (target.gameObject.tag == "Player") {

			//Door Open
			DoorAnimations.Play ("DoorOpen");
		}
	}

	void OnTriggerExit (Collider target) {

		if (target.gameObject.tag == "Player") {

			//Door Closed
			DoorAnimations.Play ("DoorClosed");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}