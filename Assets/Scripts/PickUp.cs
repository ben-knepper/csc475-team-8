using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	GameObject mainCamera;
	bool carrying;
	GameObject carriedObject;
	public float distance;
	public float smooth;
	public float maxReach;


	// Use this for initialization
	void Start () {

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		
	}
	
	// Update is called once per frame
	void Update () {

		if (carrying) {
			carry (carriedObject);
			checkDrop ();

		} else {
			pickObjectUp ();

		}
	}

	void carry(GameObject obj) {

		obj.transform.position = Vector3.Lerp(obj.transform.position, mainCamera.transform.position + mainCamera.transform.forward * distance, Time.deltaTime * smooth);
//		obj.transform.rotation = Quaternion.identity;
			
	}

	void pickObjectUp() {

		if (Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("Fire1") || SixenseInput.Controllers[1].GetButton(SixenseButtons.TRIGGER)) {

			int x = Screen.width / 2;
			int y = Screen.height / 2;

			Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay (new Vector3 (x, y));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, maxReach)) {

				PickUpObject p = hit.collider.GetComponent<PickUpObject> ();
				if (p != null) {
					carrying = true;
					carriedObject = p.gameObject;
					p.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
				}
			}
		}
	}

	void checkDrop() {

		if (Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("Fire1") || SixenseInput.Controllers[1].GetButton(SixenseButtons.TRIGGER)) {
			dropObject ();

		}
	}

	void dropObject() {

		carrying = false;
		carriedObject.gameObject.GetComponent<Rigidbody> ().useGravity = true;
		carriedObject = null;
	}
}
