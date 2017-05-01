using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerRod : MonoBehaviour {

	public Animator rodAnimator;

    private static PickUp player;


	// Use this for initialization
	void Start () {
		//rodAnimator.SetBool("insert", false);
		//rodAnimator.Play ("RodIdle");
		rodAnimator = GetComponent<Animator>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUp>();
        //rodAnimator.SetBool ("insert", false);
        //rodAnimator.Play ("RodIdle");
    }

	//void OnTriggerEnter (Collider target) {

	//	if (target.gameObject.tag == "Player") {
	//		rodAnimator.SetBool ("insert", true);
	//		rodAnimator.Play ("RodInsert");
	//	}


 //       //if it comes into contact with the Generator's area
 //       if (target.gameObject.tag == "Generator")
 //       {

 //           Debug.Log("Inserting rod");

 //           //detach rod from character
 //           //GetComponent<PickUp>().dropObject ();
 //           //insert rod
 //           rodAnimator.SetBool("insert", true);
 //           rodAnimator.Play("RodInsert");
 //       }
 //   }

	//void OnTriggerExit(Collider target) {
		
	//	if (target.gameObject.tag == "Player") {
	//		DestroyObject (this.gameObject);
	//	}

	//}

	// Update is called once per frame
	void Update () {

	}

    //public void Insert()
    //{
    //    Debug.Log("Inserting rod");

    //    // detach rod from character
    //    player.dropObject();
    //    // insert rod
    //    //rodAnimator.SetBool("insert", true);
    //    GetComponent<Rigidbody>().isKinematic = true;
    //    rodAnimator.Play("RodInsert");

    //    // destroy the rod after a delay
    //    Destroy(gameObject, 10);
    //}

}
