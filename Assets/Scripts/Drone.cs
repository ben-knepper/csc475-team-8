using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{

    
    public GameObject _bobbingTracker;

    public float _bobbingAnimatorSpeed = 1f;
    public float _rotationTorque = 50f;
    public float _rotationRange = 45f; // distance from _idleRotateCenter
    public float _minIdleRotateStopTime = 1f;
    public float _maxIdleRotateStopTime = 5f;


    private Animator _bobbingAnimator;

    private bool _isRotating;
    private bool _isRotatingClockwise;
    private float _idleRotateCenter;


    protected override void Awake()
    {
        base.Awake();

        _bobbingAnimator = _bobbingTracker.GetComponent<Animator>();
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

    protected void FixedUpdate()
    {
        if (_bobbingAnimator.speed > 0)
            transform.position = new Vector3(
                transform.position.x,
                _bobbingTracker.transform.localPosition.y,
                transform.position.z);
    }


    protected override void Attack()
    {
        _bobbingAnimator.speed = 0f;

        //throw new NotImplementedException();
    }

    protected override void Die()
    {
        _bobbingAnimator.speed = 0f;

        //throw new NotImplementedException();
    }

    protected override void Idle()
    {
        _bobbingAnimator.speed = _bobbingAnimatorSpeed;
        _isRotating = false;
        _isRotatingClockwise = UnityEngine.Random.value < 0.5f;
        _idleRotateCenter = transform.rotation.eulerAngles.y;

        StartCoroutine("UpdateIdle");
    }
    
    private IEnumerator UpdateIdle()
    {
        while (Behavior == Behavior.Idling)
        {
            if (_isRotating)
            {

                // stop rotating by "pushing" in the opposite direction
                _rb.AddTorque(transform.up * (_isRotatingClockwise ? _rotationTorque : -_rotationTorque));

                float angleDiff = Mathf.Abs(transform.rotation.eulerAngles.y - _idleRotateCenter);
                if (angleDiff > _rotationRange || (360 - angleDiff) > _rotationRange)
                {
                    Debug.Log("Stopping");

                    _isRotating = false;
                    _isRotatingClockwise = !_isRotatingClockwise;

                    float stopTime = UnityEngine.Random.Range(_minIdleRotateStopTime, _maxIdleRotateStopTime);
                    yield return new WaitForSecondsRealtime(stopTime);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                _rb.AddTorque(transform.up * (_isRotatingClockwise ? -_rotationTorque : _rotationTorque));

                _isRotating = true;

                Debug.Log("Rotating " + (_isRotatingClockwise ? "clockwise" : "counterclockwise"));
            }
        }

        yield break;
    }

    protected override void Roam()
    {
        _bobbingAnimator.speed = _bobbingAnimatorSpeed;

        //throw new NotImplementedException();
    }

    protected override void Seek()
    {
        _bobbingAnimator.speed = _bobbingAnimatorSpeed;

        //throw new NotImplementedException();
    }
}
