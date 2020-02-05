using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public int ID;
    [SerializeField] protected ParticleSystem vfx;
    [SerializeField] float _duration;
    //TODO: Da considerare temporaneo fino ulteriori informazioni
    [SerializeField] int damage;
    //
    [SerializeField] bool friendlyFire;

    public float duration { get { return _duration; } }

    public State state { get; set; }
    public bool created { get; set; }
    float returnTime;
    [HideInInspector] public IShooter shooter;
    [HideInInspector] public CommandSequence command;

    public Action OnPreShoot;
    public Action OnShoot;
    public Action<IDamageable> OnDamage;
    public Action OnPostDamage;
    public Action OnReturn;

    protected virtual void Tick()
    {
        if (Time.time > returnTime)
            Return();
    }

    #region API
    public virtual void Shoot(Vector3 shootPosition, Vector3 direction, IShooter _shooter, CommandSequence _command)
    {
        shooter = _shooter;
        transform.position = shootPosition;
        transform.rotation = Quaternion.LookRotation(direction);
        state = State.Shooted;
        command = _command;
        returnTime = Time.time + _duration;
        OnPreShoot?.Invoke();
        OnShoot?.Invoke();
        if (vfx != null)
            vfx.Play();
    }

    public virtual void Return()
    {
        if (vfx != null)
        {
            vfx.Stop();
            vfx.Clear();
        }
        OnReturn?.Invoke();
        BulletPoolManager.instance.ReturnBullet(this);
    }
    #endregion

    public virtual void OnDamageableCollide(IDamageable damageable)
    {
        OnDamage?.Invoke(damageable);
        damageable.TakeDamage(damage, command);
        OnPostDamage?.Invoke();
    }

    private void OnEnable()
    {
        if (!created)
            created = true;
    }

    private void Update()
    {
        if (state == State.Shooted)
            Tick();
    }

    public enum State
    {
        Pooled,
        Shooted,
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEnter(other);
    }

    public void OnEnter(Collider other)
    {
        if (state == State.Shooted)
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                if (!friendlyFire && ((damageable is GenericEnemy && shooter is GenericEnemy) || (damageable is PlayerData && shooter is PlayerControllerInput)))
                {

                }
                else if (damageable.gameObject != shooter.gameObject && damageable.invulnerable == false)
                {
                    OnDamageableCollide(damageable);
                }
            }
        }
    }
}