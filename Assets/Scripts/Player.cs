using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mob
{

    public Camera _camera;
    public GameObject _target;


    private EnemyMaster _enemyMaster;


    protected override void Awake()
    {
        base.Awake();

        if (_camera == null)
            _camera = gameObject.GetComponentInChildren<Camera>();
        _enemyMaster = GameObject.FindGameObjectWithTag("EnemyMaster").GetComponent<EnemyMaster>();

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
		//yield return new WaitForSecondsRealtime(3);
		//Application.LoadLevel (Application.loadedLevel);
    }


    public override string ToString()
    {
        return "Player " + GetInstanceID();
    }

}
