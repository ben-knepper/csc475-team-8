using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerScript : MonoBehaviour {

	public bool open = false;
	public float doorOpenAngle = 90f;
	public float doorCloseAngle = 0f;
	public float smooth = 2;
	private AudioSource lockerAudio;

	// Use this for initialization
	void Start () {

		lockerAudio = GetComponent<AudioSource> ();
		
	}

	public void changeLockerState() {
		open = !open;
		lockerAudio.Play ();

	}
	
	// Update is called once per frame
	void Update () {

		if (open) {
			Quaternion targetRotation = Quaternion.Euler (0, doorOpenAngle, 0);
			transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, smooth * Time.deltaTime);

		} else {
			Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0);
			transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation2, smooth * Time.deltaTime);

		}
		
	}
}
