using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{

    // base
    [Space(10)]
    [Header("Drone Base")]
    public GameObject _gunEnd;
    public GameObject _spotlight;
    public bool _isMobile;

    private UpdateFunc _rotateFunc;
    private float _rotateEndTime;
    private float _rotateCurrentTime;
    private UpdateFunc _moveFunc;
    private float _moveEndTime;
    private float _moveCurrentTime;


    // bobbing fields
    [Space(10)]
    [Header("Bobbing")]
    public float _hoverHeight;
    public float _bobbingDistance = 0.1f; // from _hoverHeight to top/bottom of bob
    public float _bobbingSpeed = 0.2f; // bobbing frequency

    private UpdateFunc _bobbingFunc;
    private float _bobbingCurrentTime = 0f;
    private float _bobbingPeriod;


    // idling fields
    [Space(10)]
    [Header("Idling")]
    public float _idlingRotateRange = 90f; // angle from one side of the range to the other
    public float _idlingMinRotateStopTime = 1f;
    public float _idlingMaxRotateStopTime = 2f;
    public float _idleRotateSpeed = 10f;


    // roaming fields
    [Space(10)]
    [Header("Roaming")]
    public float _roamingTurningToMovingChance = 0.2f; // evaluated after every rotation while idling
    public float _roamingMoveSpeed = 5f;
    public float _roamingRotateRange = 360f;
    public float _roamingRotateSpeed = 15f;
    public float _roamingMinRotateStopTime = 1f;
    public float _roamingMaxRotateStopTime = 2f;


    // attacking fields
    [Space(10)]
    [Header("Attacking")]
    public GameObject _bullet;
    public Animator _chargingShotAnimator;
    public float _secsBetweenShots = 2f; // assumes that the _chargingShotAnimator animation occurs over 1 second
    public float _attackingRotateSpeed = 30f;
    public float _shotAlignedMarginOfError = 1f;
    public float _pauseAfterShotTime = 0.5f;
    public float _attackingTimeToWaitUntilSeek = 3f;

    private float _attackingCurrentTime = 0f;
    private float _chargingShotAnimatorSpeed;
    private bool _isPausedAfterShot = false;
    private bool _isShooting = false;
    private float _attackingWaitTimeBeforeSeeking = 0f;


    // seeking fields
    [Space(10)]
    [Header("Seeking")]
    public float _seekingTimeBeforeDefault = 15f;
    public float _seekingRotateRange = 150f;
    public float _seekingMinRotateStopTime = 0.3f;
    public float _seekingMaxRotateStopTime = 0.8f;
    public float _seekingRotateSpeed = 30f;

    private float _seekingCurrentTime;

    // death fields
    [Space(10)]
    [Header("Death")]
    public float _minDespawnTime = 30f;
    public float _minDespawnDistance = 20f;
    public float _minDespawnPlayerAngle = 120f;
    public GameObject _deathSparkParticles;


    private Collider _collider;


    protected override void Awake()
    {
        base.Awake();

        _collider = GetComponent<Collider>();
        //_collider.enabled = false;

        if (_hoverHeight <= 0)
            _hoverHeight = transform.position.y;

        _bobbingPeriod = 1 / _bobbingSpeed;
        _bobbingFunc = Bob;

        if (_chargingShotAnimator == null)
            _chargingShotAnimator = _gunEnd.GetComponent<Animator>();
        _chargingShotAnimatorSpeed = 1 / _secsBetweenShots;
        _chargingShotAnimator.speed = _chargingShotAnimatorSpeed;

        _gunEnd.SetActive(false);
        _spotlight.SetActive(false);
        _deathSparkParticles.SetActive(false);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        StartCoroutine("CheckForStartAttacking");
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

    void LateUpdate()
    {
        if (_isShooting)
        {
            Shoot();
            _isShooting = false;
        }
    }


    protected IEnumerator CheckForStartAttacking()
    {
        while (true)
        {
            if (CanSeePlayer())
                Behavior = Behavior.Attacking;

            yield return new WaitForSeconds(_behaviorUpdateInterval);
        }
    }

    protected void Bob()
    {
        _bobbingCurrentTime += Time.deltaTime;

        float newHeight = _hoverHeight + _bobbingDistance * Mathf.Sin(
            (2 * Mathf.PI / _bobbingPeriod) * _bobbingCurrentTime);
        transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
    }

    private UpdateFunc MakeRandomRotateFunc(float rotateCenter, float rotateRange, float rotateSpeed)
    {
        float rotationBounds = rotateRange / 2;
        float relativeNewAngle = UnityEngine.Random.Range(-rotationBounds, rotationBounds);
        float newAngle = rotateCenter + relativeNewAngle;
        if (newAngle >= 360)
            newAngle -= 360;
        else if (newAngle < 0)
            newAngle += 360;
        Quaternion newRotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            newAngle,
            transform.rotation.eulerAngles.z);
        UpdateFunc rotateFunc = MakeRotateFunc(newRotation, rotateSpeed);
        return rotateFunc;
    }

    private UpdateFunc MakeRotateFunc(Quaternion endRotation, float rotationSpeed)
    {
        Quaternion startRotation = transform.rotation;
        _rotateCurrentTime = 0;
        _rotateEndTime = Mathf.Sqrt(Quaternion.Angle(startRotation, endRotation)) / rotationSpeed; // sqrt makes the speed more consistent for short and long rotations
        UpdateFunc rotateFunc = () =>
        {
            // based on https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

            _rotateCurrentTime += Time.deltaTime;
            if (_rotateCurrentTime > _rotateEndTime)
                _rotateCurrentTime = _rotateEndTime;

            float t = _rotateCurrentTime / _rotateEndTime;
            //float smoothedT = t * t * t * (t * (6f * t - 15f) + 10f);
            float smoothedT = t * t * (3f - 2f * t);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, smoothedT);
        };
        return rotateFunc;
    }

    private bool RotateIsDone()
    {
        return _rotateCurrentTime >= _rotateEndTime;
    }

    private UpdateFunc MakeMoveFunc(Vector3 endPosition, float moveSpeed, bool useY = false)
    {
        Vector3 startPosition = transform.position;
        _moveCurrentTime = 0;
        _moveEndTime = Vector3.Distance(startPosition, endPosition) / moveSpeed;
        UpdateFunc moveFunc = () =>
        {
            _moveCurrentTime += Time.deltaTime;
            if (_moveCurrentTime > _moveEndTime)
                _moveCurrentTime = _moveEndTime;

            float t = _moveCurrentTime / _moveEndTime;
            float smoothT = t * t * t * (t * (6f * t - 15f) + 10f);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
        };
        return moveFunc;
    }

    private bool MoveIsDone()
    {
        return _moveCurrentTime >= _moveEndTime;
    }

    private void Shoot()
    {
        Vector3 shotDirection = transform.forward;
        GameObject bullet = Instantiate(_bullet, _gunEnd.transform.position, Quaternion.LookRotation(shotDirection));

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), _gunEnd.GetComponent<Collider>());
    }


    protected override void Attack()
    {
        Debug.Log(this + " attacking");

        _gunEnd.SetActive(true);
        _attackingCurrentTime = 0f;
        _chargingShotAnimator.speed = _chargingShotAnimatorSpeed;
        _isPausedAfterShot = false;
        _attackingWaitTimeBeforeSeeking = 0f;

        _updateFuncs += UpdateAttacking;
        _cleanupFuncs += CleanupAttacking;
    }

    private void UpdateAttacking()
    {
        _attackingCurrentTime += Time.deltaTime;

        if (_isPausedAfterShot)
        {
            if (_attackingCurrentTime >= _pauseAfterShotTime)
            {
                _isPausedAfterShot = false;
                _chargingShotAnimator.speed = _chargingShotAnimatorSpeed;
                _attackingCurrentTime = 0f;
            }
        }
        else
        {
            bool canSeePlayer = CanSeePlayer();

            // rotate toward the player
            Quaternion rotationToPlayer = Quaternion.LookRotation(
                _lastKnownPlayerPosition - transform.position, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, rotationToPlayer, _attackingRotateSpeed * Time.deltaTime);

            float angleAwayFromPlayer = Quaternion.Angle(transform.rotation, rotationToPlayer);

            if (!canSeePlayer)
            {
                if (angleAwayFromPlayer < _shotAlignedMarginOfError)
                    _attackingWaitTimeBeforeSeeking += Time.deltaTime;

                if (_attackingCurrentTime >= _secsBetweenShots)
                {
                    _chargingShotAnimator.speed = 0f;
                }

                if (_attackingWaitTimeBeforeSeeking > _attackingTimeToWaitUntilSeek)
                {
                    Behavior = Behavior.Seeking;
                }
            }
            else
            {
                if (_attackingCurrentTime >= _secsBetweenShots)
                {
                    if (angleAwayFromPlayer < _shotAlignedMarginOfError)
                    {
                        _isShooting = true;
                        //Shoot();
                        _attackingCurrentTime = 0f;

                        _chargingShotAnimator.speed = 0f;
                        _isPausedAfterShot = true;
                    }
                    else
                    {
                        //_chargingShotAnimator.Play("Drone Gun Charge", 0, 0.9f);
                        _chargingShotAnimator.speed = 0f;
                    }
                }

                _attackingWaitTimeBeforeSeeking = 0f;
            }
        }
    }

    private void CleanupAttacking()
    {
        _updateFuncs -= UpdateAttacking;

        _cleanupFuncs -= CleanupAttacking;

        _isCleaningUp = true;

        StartCoroutine("UpdateCleanupAttacking");
    }

    private IEnumerator UpdateCleanupAttacking()
    {

        _gunEnd.SetActive(false);

        _isCleaningUp = false;

        yield break;
    }

    protected override void Idle()
    {
        Debug.Log(this + " idling");

        _spotlight.SetActive(true);

        _updateFuncs += _bobbingFunc;
        _cleanupFuncs += CleanupIdling;
        StartCoroutine("UpdateIdle");
        StartCoroutine("CheckForStartAttacking");
    }
    
    private IEnumerator UpdateIdle()
    {
        float rotationCenter = transform.rotation.eulerAngles.y;

        while (true) // will stop when the coroutine is stopped
        {
            // pause rotation
            float pauseTime = UnityEngine.Random.Range(_idlingMinRotateStopTime, _idlingMaxRotateStopTime);
            yield return new WaitForSecondsRealtime(pauseTime);

            _rotateFunc = MakeRandomRotateFunc(rotationCenter, _idlingRotateRange, _idleRotateSpeed);
            _updateFuncs += _rotateFunc;

            // wait until the rotation lerp is complete
            yield return new WaitUntil(RotateIsDone);

            _updateFuncs -= _rotateFunc;
        }
    }

    protected void CleanupIdling()
    {
        StopCoroutine("UpdateIdle");
        StopCoroutine("CheckForStartAttacking");

        _updateFuncs -= _rotateFunc;
        _updateFuncs -= _bobbingFunc;

        _spotlight.SetActive(false);

        _cleanupFuncs -= CleanupIdling;
    }

    protected override void Roam()
    {
        Debug.Log(this + " roaming");

        _spotlight.SetActive(true);

        _cleanupFuncs += CleanupRoaming;
        StartCoroutine("CheckForStartAttacking");
        StartCoroutine("UpdateRoaming");
    }

    protected IEnumerator UpdateRoaming()
    {
        while (true) // will stop when the coroutine is stopped
        {
            // if the drone isn't mobile, it isn't on a patrol track, or there aren't adjacent PatrolNodes, just idle
            if (!_isMobile
                || _targetPatrolNode == null
                || _targetPatrolNode._adjacentNodes.Length == 0)
            {
                Debug.Log(this + " has no patrol track");
                Behavior = Behavior.Idling;
                break;
            }

            // select a new PatrolNode
            var nodes = _targetPatrolNode._adjacentNodes;
            _targetPatrolNode = nodes[UnityEngine.Random.Range(0, nodes.Length)];
            Vector3 nodePosition = _targetPatrolNode.transform.position;
            if (!_targetPatrolNode._useY)
                nodePosition.y = transform.position.y;

            // face the node
            _rotateFunc = MakeRotateFunc(
                Quaternion.LookRotation(nodePosition - transform.position, Vector3.up),
                _roamingRotateSpeed);
            _updateFuncs += _rotateFunc;
            yield return new WaitUntil(RotateIsDone);
            _updateFuncs -= _rotateFunc;

            // move to the node
            _moveFunc = MakeMoveFunc(nodePosition, _roamingMoveSpeed);
            _updateFuncs += _moveFunc;
            yield return new WaitUntil(MoveIsDone);
            _updateFuncs -= _moveFunc;

            if (_targetPatrolNode._isStoppingNode)
            {
                float rand = 1f;
                while (rand > _roamingTurningToMovingChance)
                {
                    // pause rotation
                    float pauseTime = UnityEngine.Random.Range(_roamingMinRotateStopTime, _roamingMaxRotateStopTime);
                    yield return new WaitForSecondsRealtime(pauseTime);

                    // rotate
                    _rotateFunc = MakeRandomRotateFunc(transform.rotation.eulerAngles.y, _roamingRotateRange, _roamingRotateSpeed);
                    _updateFuncs += _rotateFunc;

                    yield return new WaitUntil(RotateIsDone);

                    _updateFuncs -= _rotateFunc;

                    rand = UnityEngine.Random.value;
                }
            }
        }
    }

    private void CleanupRoaming()
    {
        StopCoroutine("UpdateRoaming");
        StopCoroutine("CheckForStartAttacking");

        _updateFuncs -= _moveFunc;
        _updateFuncs -= _rotateFunc;

        _spotlight.SetActive(false);

        //_isCleaningUp = true;
    }

    protected override void Seek()
    {
        Debug.Log(this + " seeking player at " + _lastKnownPlayerPosition);

        _updateFuncs += CheckForSeekingToIdling;
        _cleanupFuncs += CleanupSeeking;
        StartCoroutine("UpdateSeeking");
        StartCoroutine("CheckForStartAttacking");
    }

    private IEnumerator UpdateSeeking()
    {
        _seekingCurrentTime = 0;

        // first rotate to the last known player position
        _rotateFunc = MakeRotateFunc(
            Quaternion.LookRotation(_lastKnownPlayerPosition - transform.position, Vector3.up),
            _seekingRotateSpeed);
        _updateFuncs += _rotateFunc;
        yield return new WaitUntil(RotateIsDone);
        _updateFuncs -= _rotateFunc;

        float rotationCenter = transform.rotation.eulerAngles.y;

        while (true) // will stop when coroutine is stopped
        {
            //if (_isMobile)
            //{
            //    // TODO
            //}
            //else
            {
                // pause rotation
                float pauseTime = UnityEngine.Random.Range(_seekingMinRotateStopTime, _seekingMaxRotateStopTime);
                yield return new WaitForSecondsRealtime(pauseTime);

                _rotateFunc = MakeRandomRotateFunc(rotationCenter, _seekingRotateRange, _seekingRotateSpeed);
                _updateFuncs += _rotateFunc;

                // wait until the rotation lerp is complete
                yield return new WaitUntil(RotateIsDone);

                _updateFuncs -= _rotateFunc;
            }
        }
    }

    private void CheckForSeekingToIdling()
    {
        _seekingCurrentTime += Time.deltaTime;
        if (_seekingCurrentTime >= _seekingTimeBeforeDefault)
            Behavior = _defaultBehavior;
    }

    private void CleanupSeeking()
    {
        StopCoroutine("UpdateSeeking");
        StopCoroutine("CheckForStartAttacking");

        _updateFuncs -= CheckForSeekingToIdling;
        _updateFuncs -= _rotateFunc;

        //_isCleaningUp = true;
    }

    protected override void Die()
    {
        Debug.Log(this + " dying");

        _rb.isKinematic = false;
        _rb.useGravity = true;
        //_collider.enabled = true;

        _deathSparkParticles.SetActive(true);

        StartCoroutine("UpdateDying");
    }

    private IEnumerator UpdateDying()
    {
        // wait to start detecting whether to despawn
        yield return new WaitForSecondsRealtime(_minDespawnTime);

        Debug.Log(this + " ready to despawn");

        // then wait until the player is far enough away and not looking at the drone
        yield return new WaitUntil(() =>
        {
            Vector3 playerToDrone = transform.position - _player._target.transform.position;
            float distanceToPlayer = playerToDrone.magnitude;
            float angleToPlayer = Mathf.Abs(Vector3.Angle(_player._target.transform.forward, playerToDrone));

            return distanceToPlayer >= _minDespawnDistance && angleToPlayer > _minDespawnPlayerAngle;
        });

        // finally despawn the drone
        Debug.Log(this + " being destroyed");
        _enemyMaster.enemies.Remove(this);
        Destroy(gameObject);

        yield break;
    }


    public override void Alert()
    {
        base.Alert();

        if (Behavior == Behavior.Roaming || Behavior == Behavior.Idling)
        {
            Behavior = Behavior.Seeking;
        }
        else if (Behavior == Behavior.Seeking)
        {
            StopCoroutine("UpdateSeeking");
            _updateFuncs -= _rotateFunc;

            StartCoroutine("UpdateSeeking");

            _seekingCurrentTime = 0f;
        }
    }


    public override string ToString()
    {
        return "Drone " + GetInstanceID();
    }

}
