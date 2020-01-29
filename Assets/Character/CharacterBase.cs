using System;
using System.Collections;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IDamageable
{
    const float KNOCKBACK_RATE = 1f;

    #region Serialized
    [Header("Health")]
    [SerializeField] int _currentHealth;
    [SerializeField] int _maxHealth;
    [SerializeField] Material damageMaterial;
    #endregion

    #region Interface_Damageable
    public int currentHealth { get => _currentHealth; set { _currentHealth = value; } }
    public int maxHealth { get => _maxHealth; set => _maxHealth = value; }
    public bool isDead { get; set; }
    public bool invulnerable { get; set; } 
    #endregion

    public Action<int, CommandSequence> OnDamage { get; set; }
    public Action<IDamageable> OnDeath { get; set; }

    public Rigidbody myRigidbody { get; private set; }

    public bool knockbackState { get; protected set; }
    [HideInInspector] public bool canMove;

    public Renderer renderer { get; protected set; }
    public Material originalMaterial { get; protected set; }

    public virtual void TakeDamage(int _damage, CommandSequence _command)
    {
        if (!invulnerable)
        {
            currentHealth -= _damage;
            if (renderer && damageMaterial)
            {
                if (damageFeedbackCorutine != null)
                    StopCoroutine(damageFeedbackCorutine);
                damageFeedbackCorutine = DamageFeedbackCorutine();
                StartCoroutine(damageFeedbackCorutine);
            }
            OnDamage?.Invoke(_damage, _command);
            if (_currentHealth <= 0) 
                OnDeath?.Invoke(this);
        }
    }

    IEnumerator damageFeedbackCorutine;
    IEnumerator DamageFeedbackCorutine()
    {
        renderer.material = damageMaterial;
        yield return new WaitForSeconds(0.5f);
        renderer.material = originalMaterial;
    }

    public virtual void Stun(float _duration)
    {
        Debug.Log("Stunned");
    }

    IEnumerator knockbackCorutine;
    public virtual void KnockBack(float _force, Vector3 _hitPoint)
    {
        if (!knockbackState)
        {
            if (knockbackCorutine != null)
                StopCoroutine(knockbackCorutine);
            knockbackState = true;
            knockbackCorutine = KnockbackCorutine();
            myRigidbody.AddExplosionForce(_force, _hitPoint, 1f, 0f, ForceMode.Impulse);
            StartCoroutine(knockbackCorutine);
        }
    }

    IEnumerator KnockbackCorutine()
    {
        yield return new WaitForSeconds(KNOCKBACK_RATE);
        knockbackState = false;
    }

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<Renderer>();
        if (renderer)
            originalMaterial = renderer.material;
        invulnerable = false;
    }

    public void SetMaterial(Material newMaterial)
    {
        if (renderer)
            renderer.material = newMaterial;
    }

    IEnumerator invulnerabilityCorutine;
    public void TempInvulnerability(float _duration)
    {
        if (invulnerabilityCorutine != null)
            StopCoroutine(invulnerabilityCorutine);
        invulnerabilityCorutine = InvulnerabilityCorutine(_duration);
        StartCoroutine(invulnerabilityCorutine);
    }

    IEnumerator InvulnerabilityCorutine(float _duration)
    {
        invulnerable = true;
        yield return new WaitForSeconds(_duration);
        invulnerable = false;
    }
}