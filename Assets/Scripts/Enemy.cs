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
                UpdateBehavior();
            }
        }
    }


    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        Behavior = Behavior.Idling;
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

    private void UpdateBehavior()
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

}
