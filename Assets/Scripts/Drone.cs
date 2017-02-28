using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{

    public Animator _bobbingAnimator;
    public float _bobbingAnimatorSpeed = 1f;
    public float _rotationTorque = 10f;
    public float _idleChangeRotatingChance = 0.1f;


    private bool _isRotating;
    private bool _isRotatingClockwise;


    protected override void Awake()
    {
        base.Awake();
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

        StartCoroutine("UpdateIdle");
    }
    
    private IEnumerator UpdateIdle()
    {
        while (Behavior == Behavior.Idling)
        {
            bool changingRotating = UnityEngine.Random.value < _idleChangeRotatingChance;

            if (changingRotating)
            {
                if (_isRotating)
                {
                    // stop rotating by "pushing" in the opposite direction
                    _rb.AddTorque(transform.up * (_isRotatingClockwise ? _rotationTorque : -_rotationTorque));
                }
                else
                {
                    _isRotatingClockwise = UnityEngine.Random.value < 0.5f;
                    _rb.AddTorque(transform.up * (_isRotatingClockwise ? -_rotationTorque : _rotationTorque));
                }
            }

            yield return new WaitForSeconds(0.1f);
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
