using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mob
{

    public Camera _camera;
    public GameObject _target;


    protected override void Awake()
    {
        base.Awake();

        if (_camera == null)
            _camera = gameObject.GetComponentInChildren<Camera>();

        Health = _maxHealth;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
	}

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Kill()
    {
        base.Kill();

        // TODO
    }


    public override string ToString()
    {
        return "Player " + GetInstanceID();
    }

}
