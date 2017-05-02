using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public int rodCount;
    public Animator rodAnimator;

    private PickUp player;
	private Door powerlessDoor;

	// Use this for initialization
	void Start () {
		rodCount = 0;
        if (player == null) 
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUp>();
			powerlessDoor = GameObject.FindGameObjectWithTag ("PowerlessDoor").GetComponent<Door> ();
		powerlessDoor.DoorAnimations.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		//check if all rods have been inserted
		if (rodCount == 4) {
			//open exit door
			powerlessDoor.DoorAnimations.enabled = true;
		}
		
	}

	void OnTriggerEnter (Collider target) {

        //if a rod has entered the generator's area
		if (target.tag == "Energy") {

            Debug.Log("Rod being inserted");

            player.dropObject();
            Destroy(target.gameObject);

            if (rodAnimator.speed > 0)
                rodAnimator.Play("RodInsert");
            else
                rodAnimator.speed = 1f;

            StartCoroutine("PauseAnimation");

			//add one to the rod count
			rodCount += 1;
		}
	}

    IEnumerator PauseAnimation()
    {
        yield return new WaitForSeconds(0.99f);

        rodAnimator.speed = 0;

        yield break;
    }
}
