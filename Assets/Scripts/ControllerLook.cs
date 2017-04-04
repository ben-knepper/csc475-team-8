using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLook : MonoBehaviour {

    public float lookSpeed = 50f;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float yRotation = SixenseInput.Controllers[1].JoystickX;
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + yRotation * Time.deltaTime * lookSpeed,
            transform.rotation.eulerAngles.z);
	}
}
