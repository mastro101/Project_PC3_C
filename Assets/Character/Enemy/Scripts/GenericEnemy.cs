using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : CharacterBase , IShooter
{
    [SerializeField] int exp;
    [SerializeField] BulletBase bullet;
    [SerializeField] float viewRadious;
    [SerializeField] float fireRate;
    [SerializeField] Transform _shootPosition;

    CommandSequence lastHitCommand;

    Transform targetTransform;
    float timer;

    Collider[] colliders;

    public Vector3 shootPosition { get { return _shootPosition.position; } }
    public Vector3 aimDirection { get; set; }

    public Action OnDestroy { get; set; }

    protected override void Awake()
    {
        base.Awake();
        OnDamage += Damage;
        OnDeath += Death;
    }

    private void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, viewRadious);

        foreach (var item in colliders)
        {
            PlayerData player = item.GetComponentInParent<PlayerData>();
            if (player != null)
            {
                aimDirection = player.transform.position - transform.position;
                timer += Time.deltaTime;
                if (timer >= fireRate)
                    Attack(player.transform);
            }
        }
    }

    void Attack(Transform target)
    {
        timer = 0f;
        BulletPoolManager.instance.Shoot(bullet, shootPosition, aimDirection, this, null);
    }

    void Death(IDamageable _damageable)
    {
        if (lastHitCommand != null)
            lastHitCommand.AddExp(exp);
        OnDeath -= Death;
        Destroy(gameObject);
    }

    void Damage(int _damage, CommandSequence _command)
    {
        lastHitCommand = _command;
        if (lastHitCommand != null)
            lastHitCommand.AddExp(1);
        timer = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, viewRadious);
    }
}