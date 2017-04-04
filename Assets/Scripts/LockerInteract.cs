using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteract : MonoBehaviour {

	public float interactDistance = 5f;
    public GameObject hand;

    // Use this for initialization
    void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.F) || Input.GetButtonDown("Fire2") || SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.TRIGGER)) {
			Ray ray = new Ray (hand.transform.position, hand.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, interactDistance)) {
				if (hit.collider.CompareTag ("Locker")) {
					hit.collider.transform.GetComponent<LockerScript> ().changeLockerState ();

				}

			}

		}
		
	}
}
