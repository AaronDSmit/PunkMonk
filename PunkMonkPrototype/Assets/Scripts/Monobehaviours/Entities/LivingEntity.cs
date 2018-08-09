using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : Entity
{
    [SerializeField] protected float maxHealth = 100;

    [SerializeField] private Material dissolveMat;

    [SerializeField] private float deathAnimationTime;

    protected float currentHealth;

    protected bool dead;

    protected Renderer myRenderer;

    // event
    public delegate void Dead(LivingEntity a_entity);
    public event Dead OnDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        myRenderer = GetComponentInChildren<Renderer>();
        dead = false;
    }

    public override void TakeDamage(float a_damageAmount)
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

        StartCoroutine(AnimateDeath());
    }

    private IEnumerator AnimateDeath()
    {
        myRenderer.material = dissolveMat;

        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / deathAnimationTime;

            myRenderer.material.SetFloat("_DissolveAmount", Mathf.Lerp(0, 1, t));

            yield return null;
        }

        Destroy(gameObject);
    }

    #region Getters / Setters

    public bool IsDead
    {
        get { return dead; }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
    }

    public float HealthPercent
    {
        get { return currentHealth / maxHealth; }
    }

    #endregion
}