using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{

    // bobbing fields
    [Space(10)]
    [Header("_bobbingFuncbing")]
    public float _hoverHeight = 3.2f;
    public float _bobbingDistance = 0.1f; // from _hoverHeight to top/bottom of bob
    public float _bobbingSpeed = 0.2f; // bobbing frequency

    private UpdateFunc _bobbingFunc;
    private float _bobbingCurrentTime = 0f;
    private float _bobbingPeriod;

    // idle rotation fields
    [Space(10)]
    [Header("Idle Rotation")]
    public float _idleRotationRange = 90f; // angle from one side of the range to the other
    public float _minIdleRotateStopTime = 1f;
    public float _maxIdleRotateStopTime = 2f;
    public float _rotationSpeed = 10f;

    private UpdateFunc _rotateFunc;
    private float _idleRotationBounds;
    private float _idleRotationCenter;
    private float _idleRotationEndTime;
    private float _idleRotationCurrentTime;
    private float _idleLastRotationAngle;


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
        _collider.enabled = false;

        _idleRotationBounds = _idleRotationRange / 2;

        _bobbingPeriod = 1 / _bobbingSpeed;
        _bobbingFunc = Bob;
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

        if (Input.GetButton("Fire1"))
            Behavior = Behavior.Dying;
    }


    protected void Bob()
    {
        _bobbingCurrentTime += Time.deltaTime;

        float newHeight = _hoverHeight + _bobbingDistance * Mathf.Sin(
            (2 * Mathf.PI / _bobbingPeriod) * _bobbingCurrentTime);
        transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
    }

    protected override void Attack()
    {
        throw new NotImplementedException();
    }

    protected override void Die()
    {
        Debug.Log("Drone " + GetInstanceID() + " dying");

        _rb.isKinematic = false;
        _rb.useGravity = true;
        _collider.enabled = true;

        _deathSparkParticles.SetActive(true);

        StartCoroutine(UpdateDying());
    }

    private IEnumerator UpdateDying()
    {
        // wait to start detecting whether to despawn
        yield return new WaitForSecondsRealtime(_minDespawnTime);

        // then wait until the player is far enough away and not looking at the drone
        yield return new WaitUntil(() =>
        {
            Vector3 playerToDrone = transform.position - playerCamera.transform.position;
            float distanceToPlayer = playerToDrone.magnitude;
            float angleToPlayer = Mathf.Abs(Vector3.Angle(playerCamera.transform.forward, playerToDrone));

            return distanceToPlayer >= _minDespawnDistance && angleToPlayer > _minDespawnPlayerAngle;
        });
        
        // finally despawn the drone
        Debug.Log("Drone " + GetInstanceID() + " being destroyed");
        Destroy(gameObject);

        yield break;
    }

    protected override void Idle()
    {
        Debug.Log("Drone " + GetInstanceID() + " idling");

        _idleRotationCenter = transform.rotation.eulerAngles.y;
        _idleLastRotationAngle = _idleRotationCenter;

        _updateFuncs += _bobbingFunc;
        _cleanupFuncs += CleanupIdling;
        StartCoroutine("UpdateIdle");
    }
    
    private IEnumerator UpdateIdle()
    {
        while (Behavior == Behavior.Idling)
        {
            // pause rotation
            float pauseTime = UnityEngine.Random.Range(_minIdleRotateStopTime, _maxIdleRotateStopTime);
            yield return new WaitForSecondsRealtime(pauseTime);

            // rotate
            Quaternion startQuaternion = transform.rotation;
            float relativeNewAngle = UnityEngine.Random.Range(-_idleRotationBounds, _idleRotationBounds);
            float newAngle = _idleRotationCenter + relativeNewAngle;
            if (newAngle >= 360)
                newAngle -= 360;
            else if (newAngle < 0)
                newAngle += 360;
            Quaternion endQuaternion = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                newAngle,
                transform.rotation.eulerAngles.z);
            float angleDiff = Mathf.Abs(newAngle - _idleLastRotationAngle);
            if (angleDiff >= 180)
                angleDiff = 180 - angleDiff;
            _idleRotationCurrentTime = 0;
            _idleRotationEndTime = angleDiff / _rotationSpeed;
            _idleLastRotationAngle = newAngle;
            // add rotation updater to FixedUpdate (see Mob.FixedUpdate())
            _rotateFunc = () =>
            {
                // based on https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

                _idleRotationCurrentTime += Time.deltaTime;
                if (_idleRotationCurrentTime > _idleRotationEndTime)
                    _idleRotationCurrentTime = _idleRotationEndTime;

                float t = _idleRotationCurrentTime / _idleRotationEndTime;
                float smoothedT = t * t * t * (t * (6f * t - 15f) + 10f);
                transform.rotation = Quaternion.Lerp(startQuaternion, endQuaternion, smoothedT);
            };
            _updateFuncs += _rotateFunc;

            // wait until the rotation lerp is complete
            yield return new WaitUntil(() => _idleRotationCurrentTime >= _idleRotationEndTime);

            _updateFuncs -= _rotateFunc;
        }

        yield break;
    }

    protected void CleanupIdling()
    {
        StopCoroutine("UpdateIdle");
        _updateFuncs -= _rotateFunc;
        _updateFuncs -= _bobbingFunc;
    }

    protected override void Roam()
    {

    }

    protected override void Seek()
    {

    }

}
