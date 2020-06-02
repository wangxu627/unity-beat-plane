using UnityEngine;
using System.Collections;



public class Health : MonoBehaviour
{
    /// the model to disable (if set so)
    public GameObject Model;

    /// the current health of the character
    public int CurrentHealth ;
    /// If this is true, this object can't take damage
    public bool Invulnerable = false;	

    public int InitialHealth = 10;
    /// the maximum amount of health of the object
    public int MaximumHealth = 10;
    public bool DestroyOnDeath = true;
    /// the time (in seconds) before the character is destroyed or disabled
    public float DelayBeforeDestruction = 0f;
    /// the points the player gets when the object's health reaches zero
    public int PointsWhenDestroyed;
    /// if this is set to false, the character will respawn at the location of its death, otherwise it'll be moved to its initial position (when the scene started)
    public bool RespawnAtInitialLocation = false;
    /// if this is true, the controller will be disabled on death
    /// if this is true, the model will be disabled instantly on death (if a model has been set)
    public bool DisableModelOnDeath = true;
    /// if this is true, collisions will be turned off when the character dies
    public bool DisableCollisionsOnDeath = true;

    // hit delegate
    public delegate void OnHitDelegate();
    public OnHitDelegate OnHit;

    // respawn delegate
    public delegate void OnReviveDelegate();
    public OnReviveDelegate OnRevive;

    // death delegate
    public delegate void OnDeathDelegate();
    public OnDeathDelegate OnDeath;

    protected Vector3 _initialPosition;
    protected Renderer _renderer;
    protected Collider _collider;
    protected bool _initialized = false;
    protected Color _initialColor;

    /// <summary>
    /// On Start, we initialize our health
    /// </summary>
    protected virtual void Start()
    {
        Initialization();
    }

    /// <summary>
    /// Grabs useful components, enables damage and gets the inital color
    /// </summary>
    protected virtual void Initialization()
    {
        if (Model != null)
        {
            Model.SetActive(true);
        }        
        
        _collider = GetComponent<Collider>();

        _initialPosition = transform.position;
        _initialized = true;
        CurrentHealth = InitialHealth;
        DamageEnabled();
        UpdateHealthBar (false);
    }

    /// <summary>
    /// When the object is enabled (on respawn for example), we restore its initial health levels
    /// </summary>
    protected virtual void OnEnable()
    {
        CurrentHealth = InitialHealth;
        if (Model != null)
        {
            Model.SetActive(true);
        }            
        DamageEnabled();
        UpdateHealthBar (false);
    }

    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public virtual void Damage(int damage, float invincibilityDuration)
    {
        // if the object is invulnerable, we do nothing and exit
        if (Invulnerable)
        {
            return;
        }

        // if we're already below zero, we do nothing and exit
        if ((CurrentHealth <= 0) && (InitialHealth != 0))
        {
            return;
        }

        // we decrease the character's health by the damage
        float previousHealth = CurrentHealth;
        CurrentHealth -= damage;

        if (OnHit != null)
        {
            OnHit();
        }

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabled(invincibilityDuration));	
        }
        
        // we update the health bar
        UpdateHealthBar(true);

        // if health has reached zero
        if (CurrentHealth <= 0)
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;

            Kill();
        }
    }

    /// <summary>
    /// Kills the character, vibrates the device, instantiates death effects, handles points, etc
    /// </summary>
    public virtual void Kill()
    {
        CurrentHealth = 0;

        // we prevent further damage
        DamageDisabled();

        // we make it ignore the collisions from now on
        if (DisableCollisionsOnDeath)
        {
            if (_collider != null)
            {
                _collider.enabled = false;
            }
        }

        OnDeath?.Invoke();

        if (DisableModelOnDeath && (Model != null))
        {
            Model.SetActive(false);
        }

        if (DelayBeforeDestruction > 0f)
        {
            Invoke ("DestroyObject", DelayBeforeDestruction);
        }
        else
        {
            // finally we destroy the object
            DestroyObject();	
        }
    }

    /// <summary>
    /// Revive this object.
    /// </summary>
    public virtual void Revive()
    {
        if (!_initialized)
        {
            return;
        }


        if (_collider != null)
        {
            _collider.enabled = true;
        }
       
        if (_renderer!= null)
        {
            _renderer.material.color = _initialColor;
        }            

        if (RespawnAtInitialLocation)
        {
            transform.position = _initialPosition;
        }

        Initialization();
        UpdateHealthBar(false);
        if (OnRevive != null)
        {
            OnRevive ();
        }
    }

    /// <summary>
    /// Destroys the object, or tries to, depending on the character's settings
    /// </summary>
    protected virtual void DestroyObject()
    {
        if (!DestroyOnDeath)
        {
            return;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Called when the character gets health (from a stimpack for example)
    /// </summary>
    /// <param name="health">The health the character gets.</param>
    /// <param name="instigator">The thing that gives the character health.</param>
    public virtual void GetHealth(int health,GameObject instigator)
    {
        // this function adds health to the character's Health and prevents it to go above MaxHealth.
        CurrentHealth = Mathf.Min (CurrentHealth + health,MaximumHealth);
        UpdateHealthBar(true);
    }

    /// <summary>
    /// Resets the character's health to its max value
    /// </summary>
    public virtual void ResetHealthToMaxHealth()
    {
        CurrentHealth = MaximumHealth;
        UpdateHealthBar (false);
    }	

    /// <summary>
    /// Updates the character's health bar progress.
    /// </summary>
    protected virtual void UpdateHealthBar(bool show)
    {
       
    }

    /// <summary>
    /// Prevents the character from taking any damage
    /// </summary>
    public virtual void DamageDisabled()
    {
        Invulnerable = true;
    }

    /// <summary>
    /// Allows the character to take damage
    /// </summary>
    public virtual void DamageEnabled()
    {
        Invulnerable = false;
    }

    /// <summary>
    /// makes the character able to take damage again after the specified delay
    /// </summary>
    /// <returns>The layer collision.</returns>
    public virtual IEnumerator DamageEnabled(float delay)
    {
        yield return new WaitForSeconds (delay);
        Invulnerable = false;
    }

    /// <summary>
    /// On Disable, we prevent any delayed destruction from running
    /// </summary>
    protected virtual void OnDisable()
    {
        CancelInvoke();
    }
}
