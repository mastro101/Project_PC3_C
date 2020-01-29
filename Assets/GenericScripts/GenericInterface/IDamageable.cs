using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    GameObject gameObject { get; }
    Transform transform { get; }
    Rigidbody myRigidbody { get; }

    int currentHealth { get; }
    int maxHealth { get; set; }
    bool isDead { get; set; }
    bool invulnerable { get; set; }

    System.Action<int, CommandSequence> OnDamage { get; set; }
    System.Action<IDamageable> OnDeath { get; set; }

    void TakeDamage(int _damage, CommandSequence _command);
    void Stun(float _duration);
    void KnockBack(float _force, Vector3 _hitPoint);
    void TempInvulnerability(float _duration);
}