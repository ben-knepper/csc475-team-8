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

	public GameObject _shot1Blood;
	public GameObject _shot2Blood;
	public GameObject _shot3Blood;


    public bool IsAlive { get; set; }


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
		_shot1Blood.SetActive (false);
		_shot2Blood.SetActive (false);
		_shot3Blood.SetActive (false);

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
		_shot1Blood.SetActive (true);
		_shot1Sound.Play ();

	}

	public override void Shot2()
	{
		base.Shot2 ();
		_shot1Blood.SetActive (false);
		_shot2Blood.SetActive (true);
		_shot2Sound.Play ();

	}

    public override void Kill()
    {
        base.Kill();
		_shot2Blood.SetActive (false);
		_shot3Blood.SetActive (true);

        IsAlive = false;

		StartCoroutine("StartDeath");
    }


    public override string ToString()
    {
        return "Player " + GetInstanceID();
    }

}
