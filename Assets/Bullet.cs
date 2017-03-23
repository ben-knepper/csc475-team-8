using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage = 1;
    public float speed = 10f;
    public float lifespan = 15f;

    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifespan);
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        Mob mob = collider.gameObject.GetComponent<Mob>();
        if (mob != null)
            mob.AddDamage(damage);

        Destroy(gameObject);
    }
}
