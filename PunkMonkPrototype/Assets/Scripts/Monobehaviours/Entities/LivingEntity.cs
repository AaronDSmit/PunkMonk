using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : Entity
{
    #region Unity Inspector Fields

    [SerializeField]
    private Material dissolveMat = null;

    [SerializeField]
    private float deathAnimationTime = 1.0f;

    [SerializeField]
    protected int maxHealth = 5;

    #endregion

    #region Reference Fields

    protected Renderer myRenderer;

    protected SegmentedHealthBar healthBar;

    #endregion

    #region Local Fields

    private int currentHealth;

    private bool dead;

    public delegate void Dead(LivingEntity a_entity);
    public event Dead OnDeath;

    #endregion

    #region Properties

    public bool IsDead
    {
        get { return dead; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public float HealthPercent
    {
        get { return currentHealth / maxHealth; }
    }

    #endregion

    #region Public Methods

    public override void TakeDamage(int a_damageAmount)
    {
        currentHealth -= a_damageAmount;

        if (healthBar != null)
        {
            healthBar.CurrentHealth = currentHealth;
        }

        if (currentHealth <= 0.0f && !dead)
        {
            Die();
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        myRenderer = GetComponentInChildren<Renderer>();
        healthBar = GetComponentInChildren<SegmentedHealthBar>();

        dead = false;
    }

    #endregion

    #region Local Methods

    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        if (!dead)
        {
            dead = true;

            if (OnDeath != null)
            {
                OnDeath(this);
            }

            // play Death animation/ particle effects

            StartCoroutine(AnimateDeath());
        }
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

        currentTile.Exit();

        Destroy(gameObject);
    }

    #endregion
}