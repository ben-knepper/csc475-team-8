using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{

    // bobbing fields
    public float _hoverHeight = 3.2f;
    public float _bobbingDistance = 0.1f; // from _hoverHeight to top/bottom of bob
    public float _bobbingSpeed = 0.2f; // bobbing frequency

    private UpdateFunc _bobbingFunc;
    private float _bobbingCurrentTime = 0f;
    private float _bobbingPeriod;


    // idle rotation fields
    public float _idleRotationRange = 90f; // angle from one side of the range to the other
    public float _minIdleRotateStopTime = 1f;
    public float _maxIdleRotateStopTime = 2f;
    public float _rotationSpeed = 10f;
    
    private float _idleRotationBounds;
    private float _idleRotationCenter;
    private float _idleRotationDestTime;
    private float _idleRotationCurrentTime;
    private float _idleLastRotationAngle;


    protected override void Awake()
    {
        base.Awake();

        _idleRotationBounds = _idleRotationRange / 2;

        _bobbingPeriod = 1 / _bobbingSpeed;
        _bobbingFunc = () =>
        {
            _bobbingCurrentTime += Time.deltaTime;

            float newHeight = _hoverHeight + _bobbingDistance * Mathf.Sin(
                (2 * Mathf.PI / _bobbingPeriod) * _bobbingCurrentTime);
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        };
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


    protected override void Attack()
    {
        throw new NotImplementedException();
    }

    protected override void Die()
    {
        throw new NotImplementedException();
    }

    protected override void Idle()
    {
        Debug.Log("Idling");

        _idleRotationCenter = transform.rotation.eulerAngles.y;
        _idleLastRotationAngle = _idleRotationCenter;

        _updateFuncs += _bobbingFunc;
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
            _idleRotationDestTime = angleDiff / _rotationSpeed;
            _idleLastRotationAngle = newAngle;
            // add rotation updater to FixedUpdate (see Mob.FixedUpdate())
            UpdateFunc rotateFunc = () =>
            {
                // based on https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

                _idleRotationCurrentTime += Time.deltaTime;
                if (_idleRotationCurrentTime > _idleRotationDestTime)
                    _idleRotationCurrentTime = _idleRotationDestTime;

                float t = _idleRotationCurrentTime / _idleRotationDestTime;
                float smoothedT = t * t * t * (t * (6f * t - 15f) + 10f);
                transform.rotation = Quaternion.Lerp(startQuaternion, endQuaternion, smoothedT);
            };
            _updateFuncs += rotateFunc;

            // wait until the rotation lerp is complete
            yield return new WaitUntil(() => _idleRotationCurrentTime >= _idleRotationDestTime);

            _updateFuncs -= rotateFunc;
        }

        yield break;
    }

    protected override void Roam()
    {

    }

    protected override void Seek()
    {

    }

}
