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
    private float _rotationCenter;
    private float _rotationEndTime;
    private float _rotationCurrentTime;
    private float _lastRotationAngle;


    // bobbing fields
    [Space(10)]
    [Header("Bobbing")]
    public float _hoverHeight;
    public float _bobbingDistance = 0.1f; // from _hoverHeight to top/bottom of bob
    public float _bobbingSpeed = 0.2f; // bobbing frequency

    private UpdateFunc _bobbingFunc;
    private float _bobbingCurrentTime = 0f;
    private float _bobbingPeriod;


    // idle rotation fields
    [Space(10)]
    [Header("Idle Rotation")]
    public float _idlingRotationRange = 90f; // angle from one side of the range to the other
    public float _idlingMinRotateStopTime = 1f;
    public float _idlingMaxRotateStopTime = 2f;
    public float _idleRotationSpeed = 15f;


    // attacking fields
    [Space(10)]
    [Header("Attacking")]
    public GameObject _bullet;
    public Animator _chargingShotAnimator;
    public float _secsBetweenShots = 2f; // assumes that the _chargingShotAnimator animation occurs over 1 second
    public float _attackingRotationSpeed = 30f;
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
    public float _seekingTimeBeforeIdle = 15f;
    public float _seekingRotationRange = 150f;
    public float _seekingMinRotateStopTime = 0.3f;
    public float _seekingMaxRotateStopTime = 0.8f;
    public float _seekingRotationSpeed = 30f;

    private UpdateFunc _seekingFunc;
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

    private void StartRotation(float rotationRange, float rotationSpeed)
    {
        Quaternion startQuaternion = transform.rotation;
        float rotationBounds = rotationRange / 2;
        float relativeNewAngle = UnityEngine.Random.Range(-rotationBounds, rotationBounds);
        float newAngle = _rotationCenter + relativeNewAngle;
        if (newAngle >= 360)
            newAngle -= 360;
        else if (newAngle < 0)
            newAngle += 360;
        Quaternion endQuaternion = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            newAngle,
            transform.rotation.eulerAngles.z);
        float angleDiff = Mathf.Abs(newAngle - _lastRotationAngle);
        if (angleDiff >= 180)
            angleDiff = 360 - angleDiff;
        _rotationCurrentTime = 0;
        _rotationEndTime = angleDiff / rotationSpeed;
        _lastRotationAngle = newAngle;
        // add rotation updater to FixedUpdate (see Mob.FixedUpdate())
        _rotateFunc = () =>
        {
            // based on https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

            _rotationCurrentTime += Time.deltaTime;
            if (_rotationCurrentTime > _rotationEndTime)
                _rotationCurrentTime = _rotationEndTime;

            float t = _rotationCurrentTime / _rotationEndTime;
            //float smoothedT = t * t * t * (t * (6f * t - 15f) + 10f);
            float smoothedT = t * t * (3f - 2f * t);
            transform.rotation = Quaternion.Lerp(startQuaternion, endQuaternion, smoothedT);
        };
        _updateFuncs += _rotateFunc;
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
                transform.rotation, rotationToPlayer, _attackingRotationSpeed * Time.deltaTime);

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

        _rotationCenter = transform.rotation.eulerAngles.y;
        _lastRotationAngle = _rotationCenter;
        _spotlight.SetActive(true);

        _updateFuncs += _bobbingFunc;
        _cleanupFuncs += CleanupIdling;
        StartCoroutine("UpdateIdle");
        StartCoroutine("CheckForStartAttacking");
    }
    
    private IEnumerator UpdateIdle()
    {
        while (true)
        {
            // pause rotation
            float pauseTime = UnityEngine.Random.Range(_idlingMinRotateStopTime, _idlingMaxRotateStopTime);
            yield return new WaitForSecondsRealtime(pauseTime);

            StartRotation(_idlingRotationRange, _idleRotationSpeed);

            // wait until the rotation lerp is complete
            yield return new WaitUntil(() => _rotationCurrentTime >= _rotationEndTime);

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

        _cleanupFuncs += CleanupRoaming;
        StartCoroutine("CheckForStartAttacking");
    }

    private void CleanupRoaming()
    {
        StopCoroutine("CheckForStartAttacking");

        _isCleaningUp = true;
    }

    protected override void Seek()
    {
        Debug.Log(this + " seeking plyaer at " + _lastKnownPlayerPosition);

        Quaternion angleToLastPlayerPosition = Quaternion.LookRotation(_lastKnownPlayerPosition, Vector3.up);
        _rotationCenter = angleToLastPlayerPosition.eulerAngles.y;
        _lastRotationAngle = _rotationCenter;
        _seekingCurrentTime = 0f;

        _updateFuncs += CheckForSeekingToIdling;
        _cleanupFuncs += CleanupSeeking;
        StartCoroutine("UpdateSeeking");
        StartCoroutine("CheckForStartAttacking");
    }

    private IEnumerator UpdateSeeking()
    {
        while (true)
        {
            if (_isMobile)
            {
                // TODO
            }
            else
            {
                // pause rotation
                float pauseTime = UnityEngine.Random.Range(_seekingMinRotateStopTime, _seekingMaxRotateStopTime);
                yield return new WaitForSecondsRealtime(pauseTime);

                StartRotation(_seekingRotationRange, _seekingRotationSpeed);

                // wait until the rotation lerp is complete
                yield return new WaitUntil(() => _rotationCurrentTime >= _rotationEndTime);

                _updateFuncs -= _rotateFunc;
            }
        }
    }

    private void CheckForSeekingToIdling()
    {
        _seekingCurrentTime += Time.deltaTime;
        if (_seekingCurrentTime >= _seekingTimeBeforeIdle)
            Behavior = Behavior.Idling;
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
        Destroy(gameObject);

        yield break;
    }


    public override string ToString()
    {
        return "Drone " + GetInstanceID();
    }

}
