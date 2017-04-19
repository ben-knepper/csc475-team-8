using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Behavior
{
    Inactive,
    Idling,
    Roaming,
    Seeking,
    Attacking,
    Dying
}


public abstract class Enemy : Mob
{
    
    protected delegate void CleanupFunc();
    protected CleanupFunc _cleanupFuncs;


    [Space(10)]
    [Header("Enemy Base")]
    public Behavior _defaultBehavior = Behavior.Idling;
    public float _behaviorUpdateInterval = 0.1f;
    public GameObject[] _detectors;
    public float[] _detectorAngleSpans;
    public float[] _detectorRanges;
    public float _hearingRange = 30f;
    public PatrolNode _startingPatrolNode = null;

    protected static EnemyMaster _enemyMaster;
    protected static Player _player;
    protected Vector3 _lastKnownPlayerPosition;
    protected bool _isCleaningUp = false;
    protected PatrolNode _targetPatrolNode;


    public GameObject Target { get; protected set; }


    private Behavior _behavior;
    public Behavior Behavior
    {
        get { return _behavior; }
        set
        {
            if (_behavior != value)
            {
                _behavior = value;
                ChangeBehavior();
            }
        }
    }


    protected override void Awake()
    {
        base.Awake();

        if (_enemyMaster == null)
            _enemyMaster = GameObject.FindGameObjectWithTag("EnemyMaster").GetComponent<EnemyMaster>();
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        Health = _maxHealth;
        _targetPatrolNode = _startingPatrolNode;

        Behavior = _defaultBehavior;
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    // TODO: Remove
    bool canSeePlayer = false;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Debug.developerConsoleVisible && CanSeePlayer() != canSeePlayer)
        {
            canSeePlayer = !canSeePlayer;
            Debug.Log("CanSeePlayer() = " + canSeePlayer);
        }
    }

    
    public virtual void Initialize(Behavior behavior)
    {
        Behavior = behavior;
    }

    private void ChangeBehavior()
    {
        // clean up the states modified by behavior handlers
        if (_cleanupFuncs != null)
        {
            _cleanupFuncs.Invoke();
        }

        if (_isCleaningUp)
            StartCoroutine("CheckForStartBehavior");
        else
            StartBehavior();
    }

    private IEnumerator CheckForStartBehavior()
    {
        yield return new WaitUntil(IsNotCleaningUp);// () => !_isCleaningUp );

        StartBehavior();

        yield break;
    }
    bool IsNotCleaningUp()
    {
        return !_isCleaningUp;
    }

    private void StartBehavior()
    {
        switch (Behavior)
        {
            case Behavior.Inactive:
                Inactivate();
                break;
            case Behavior.Idling:
                Idle();
                break;
            case Behavior.Roaming:
                Roam();
                break;
            case Behavior.Seeking:
                Seek();
                break;
            case Behavior.Attacking:
                Attack();
                break;
            case Behavior.Dying:
                Die();
                break;
            default:
                // do nothing
                break;
        }
    }

    protected virtual void Inactivate()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Idle();

    protected abstract void Roam();

    protected abstract void Seek();

    protected abstract void Attack();

    protected abstract void Die();


    public bool CanSeePlayer()
    {
        if (!_player.IsAlive)
            return false;

        for (int i = 0; i < _detectors.Length; ++i)
        {
            Vector3 lineOfSight = _player._target.transform.position - _detectors[i].transform.position;

            if (lineOfSight.magnitude > _detectorRanges[i])
                continue; // player not in range of this detector

            float angleToPlayer = Vector3.Angle(_detectors[i].transform.forward, lineOfSight);
            if (angleToPlayer > _detectorAngleSpans[i] / 2)
                continue; // player not in the angle span of this detector

            RaycastHit hit;
            int layerMask = ~(1 << 8);
            bool didHit = Physics.Raycast(_detectors[i].transform.position, lineOfSight, out hit, _detectorRanges[i], layerMask);
            if (!didHit || hit.collider.gameObject.GetComponentInParent<Player>() == null)
                continue; // player behind an obstacle

            _lastKnownPlayerPosition = _player._target.transform.position;
            return true;
        }

        return false;
    }


    public override void Kill()
    {
        base.Kill();

        Behavior = Behavior.Dying;
    }

    public virtual void Alert()
    {
        _lastKnownPlayerPosition = _player._target.transform.position;
    }

    public virtual bool AlertIfInHearingRange()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= _hearingRange)
        {
            Alert();
            return true;
        }
        else
        {
            return false;
        }
    }


    public override string ToString()
    {
        return "Enemy " + GetInstanceID();
    }

}
