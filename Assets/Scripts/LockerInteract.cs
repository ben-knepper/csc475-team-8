using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteract : MonoBehaviour {

	public float interactDistance = 5f;

	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.F) || Input.GetButtonDown("Fire2") || SixenseInput.Controllers[1].GetButton(SixenseButtons.TRIGGER)) {
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, interactDistance)) {
				if (hit.collider.CompareTag ("Locker")) {
					hit.collider.transform.GetComponent<LockerScript> ().changeDoorState ();

				}

			}

		}
		
	}
}
