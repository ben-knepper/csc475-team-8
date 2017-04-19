using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Mob
{

    public Camera _camera;
    public GameObject _target;
	public Rigidbody _rb;
    public Animator _fadeAnimator;
    public GameObject _canvas;


    public bool IsAlive { get; set; }


	//public AudioSource hurtSound;
	public AudioSource[] _sounds;


	private AudioSource _deathSound;
	private AudioSource _shot1Sound;
	private AudioSource _shot2Sound;

    
    private EnemyMaster _enemyMaster;


    protected override void Awake()
    {
        base.Awake();

        if (_camera == null)
            _camera = gameObject.GetComponentInChildren<Camera>();
        _enemyMaster = GameObject.FindGameObjectWithTag("EnemyMaster").GetComponent<EnemyMaster>();
        _rb = GetComponent<Rigidbody>();

        Health = _maxHealth;
        IsAlive = true;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        _sounds = GetComponents<AudioSource> ();
		_shot1Sound = _sounds [0];
		_shot2Sound = _sounds [1];
		_deathSound = _sounds [2];

        StartCoroutine("StartNewLevel");
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

    private IEnumerator StartNewLevel()
    {
        _fadeAnimator.Play("FadeIn");

        yield return new WaitForSeconds(2);

        _canvas.SetActive(false);

        yield break;
    }

	private IEnumerator StartDeath()
    {
        //_rb.isKinematic = true;

        _deathSound.Play ();

        _canvas.SetActive(true);
        _fadeAnimator.Play("FadeOut");
		yield return new WaitForSeconds(2);

		Application.LoadLevel (Application.loadedLevel);
        yield break;
	}

	public override void Shot1()
	{
		base.Shot1 ();
		_shot1Sound.Play ();

	}

	public override void Shot2()
	{
		base.Shot2 ();
		_shot2Sound.Play ();

	}

    public override void Kill()
    {
        base.Kill();

        IsAlive = false;

		StartCoroutine("StartDeath");
    }


    public override string ToString()
    {
        return "Player " + GetInstanceID();
    }

}
