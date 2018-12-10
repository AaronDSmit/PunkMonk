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

    private bool hasVolt = false;

    [SerializeField]
    private int currentHealth;

    private bool dead;

    public delegate void Dead(LivingEntity a_entity);
    public event Dead OnDeath;

    #endregion

    #region Properties

    public bool HasVolt
    {
        get { return hasVolt; }

        set
        {
            bool voltChanged = value != hasVolt;

            hasVolt = value;

            if (CompareTag("Enemy") && hasVolt)
            {
                myRenderer.material.SetFloat("_HighlightAmount", 0.5f);
                myRenderer.material.SetColor("_HighlightColour", new Color(1.0f, 0.62f, 0.21f));
                myRenderer.material.SetInt("_UseHighlight", 1);
            }
            else
            {
                myRenderer.material.SetInt("_UseHighlight", 0);
            }

            if (voltChanged)
            {
                VoltChanged();
            }
        }
    }

    public bool IsDead
    {
        get { return dead; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
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

    public override void TakeDamage(int a_damageAmount, Unit a_damageFrom)
    {
        currentHealth -= a_damageAmount;

        if (healthBar != null)
        {
            healthBar.CurrentHealth = currentHealth;
        }

        if (currentHealth <= 0 && !dead)
        {
            if (HasVolt)
            {
                if (CompareTag("Enemy"))
                {
                    Manager.instance.PlayerController.GiveVolt();
                }
                else
                {
                    a_damageFrom.HasVolt = true;
                    a_damageFrom.HasKilled();
                }
            }

            Die();
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        myRenderer = GetComponentInChildren<Renderer>();
        SegmentedHealthBar[] healthBars = GetComponentsInChildren<SegmentedHealthBar>();
        healthBar = healthBars[0];
    }

    protected virtual void Start()
    {
        healthBar.MaxHealth = MaxHealth;
        healthBar.CurrentHealth = CurrentHealth;
    }

    #endregion

    #region Local Methods

    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        if (!dead)
        {
            dead = true;

            Manager.instance.PlayerController.SwitchSelection((Unit)this);

            if (healthBar)
            {
                Destroy(healthBar.gameObject);
            }

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

    protected virtual void VoltChanged()
    {

    }

    #endregion
}