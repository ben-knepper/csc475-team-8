using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mob : MonoBehaviour
{

    protected delegate void UpdateFunc();


    protected Rigidbody _rb;
    protected UpdateFunc _updateFuncs;


    [Header("Mob Base")]
    [Space(5)]
    public int _maxHealth = 3;


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


    public void AddDamage(int damage)
    {
        if (Health <= 0)
            return;

        Health -= damage;

		if (Health == 2)
			Shot1 ();
		if (Health == 1)
			Shot2 ();
        if (Health < 0)
            Health = 0;
        else if (Health > _maxHealth)
            Health = _maxHealth;

        Debug.Log(this + " hit for " + damage + " damage (Health = " + Health + ")");

        if (Health <= 0)
            Kill();
    }

	public virtual void Shot1()
	{

	}

	public virtual void Shot2()
	{

	}

    public virtual void Kill()
    {
        Debug.Log(this + " killed");
    }


    public override string ToString()
    {
        return "Mob " + GetInstanceID();
    }

}
