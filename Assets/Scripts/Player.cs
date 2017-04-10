using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Mob
{

    public Camera _camera;
    public GameObject _target;
	public Rigidbody rb;
	//public AudioSource hurtSound;
	public AudioSource[] sounds;
	private AudioSource deathSound;
	private AudioSource shot1Sound;
	private AudioSource shot2Sound;



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
		rb = GetComponent<Rigidbody> ();
		sounds = GetComponents<AudioSource> ();
		shot1Sound = sounds [0];
		shot2Sound = sounds [1];
		deathSound = sounds [2];
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

	IEnumerator death ()
	{
		deathSound.Play ();
		rb.isKinematic = true;
		yield return new WaitForSeconds(1);
		Application.LoadLevel (Application.loadedLevel);

	}

	public override void Shot1()
	{
		base.Shot1 ();
		shot1Sound.Play ();

	}

	public override void Shot2()
	{
		base.Shot2 ();
		shot2Sound.Play ();

	}

    public override void Kill()
    {
        base.Kill();

		StartCoroutine (death ());

        // TODO

    }


    public override string ToString()
    {
        return "Player " + GetInstanceID();
    }

}
