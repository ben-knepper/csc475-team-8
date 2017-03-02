using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mob : MonoBehaviour
{

    protected delegate void UpdateFunc();


    protected Rigidbody _rb;
    protected UpdateFunc _updateFuncs;


    [Header("Basic Mob Fields")]
    [Space(5)]
    public int _maxHealth = 2;
    public float _moveSpeed = 5f;


    public int Health { get; protected set; }


    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
		
	}
	
	// Update is called once per frame
	protected virtual void Update()
    {
		
	}

    protected virtual void FixedUpdate()
    {
        if (_updateFuncs != null)
            _updateFuncs.Invoke();
    }

}
