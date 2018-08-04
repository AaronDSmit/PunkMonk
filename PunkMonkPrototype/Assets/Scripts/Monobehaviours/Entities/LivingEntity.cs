using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : Entity
{
    [SerializeField] protected float startHealth = 100;

    protected float currentHealth;

    protected bool dead;

    // event
    public delegate void Dead(LivingEntity a_entity);
    public event Dead OnDeath;

    protected virtual void Awake()
    {
        currentHealth = startHealth;
        dead = false;
    }

    public override void TakeDamage(Element a_damageType, float a_damageAmount)
    {
        currentHealth -= a_damageAmount;

        //if (hpBar != null)
        //{
        //    hpBar.UpdateValue(LifePercent);
        //}

        if (currentHealth <= 0.0f && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        dead = true;

        if (OnDeath != null)
        {
            OnDeath(this);
        }

        // play Death animation/ particle effects

        Destroy(gameObject);
    }

    // Returns entity's dead status
    public bool IsDead
    {
        get { return dead; }
    }

    public float LifePercent
    {
        get { return currentHealth / startHealth; }
    }
}