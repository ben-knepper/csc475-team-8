using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public int rodCount;

	// Use this for initialization
	void Start () {
		rodCount = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		//check if all rods have been inserted
		if (rodCount == 4) {
			//open exit door
		}
		
	}

	void OnTriggerEnter (Collider target) {

		//if a rod has entered the generator's area
		if (target.gameObject.tag == "Energy") {

			Debug.Log("Adding rod count");
			//add one to the rod count
			rodCount += 1;
		}
	}
}
