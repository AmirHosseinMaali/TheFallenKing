using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility;  // 1 point increase evasion by 1% and crit.chance by 1%
    public Stat intelligence; // 1 point increase magic damage by 1 and magic resistance by 3
    public Stat vitality; // 1 point increased health by 3 or 5 points

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;   // default value 150%

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;  // do damage over time
    public Stat iceDamage;   // reduce armor 20%
    public Stat lightningDamage;  //reduce accuracy 20%

    [Header("Effect ApplyIcons")]
    public GameObject igniteIcon;
    public GameObject chillIcon;
    public GameObject shockIcon;

    [Space]
    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    private float elementTimer;

    private float ignitedDamageCooldown = .3f;
    private float ignitedDamageTimer;
    private float igniteDamage;

    [Space]
    [SerializeField] private GameObject shockStrikePrefab;
    private float shockDamage;


    public float currentHealth;

    public System.Action onHealthChanged;
    protected bool isDead;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        elementTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (elementTimer < 0)
        {
            isIgnited = false;
            isChilled = false;
            isShocked = false;
        }

        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
    }


    

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;

        float totalDamage = damage.GetValue() + strength.GetValue();

        if (canCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(_targetStats);
    }

    #region Magical damage and elements

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        float _fireDamage = fireDamage.GetValue();
        float _iceDamage = iceDamage.GetValue();
        float _lightningDamage = lightningDamage.GetValue();

        float totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0) { return; }

        AttemptToApplyElements(_targetStats, _fireDamage, _iceDamage, _lightningDamage);

    }

    private void AttemptToApplyElements(CharacterStats _targetStats, float _fireDamage, float _iceDamage, float _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3 && _fireDamage > 0)
            {
                _targetStats.ApplyElements(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .4 && _iceDamage > 0)
            {
                _targetStats.ApplyElements(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5 && _lightningDamage > 0)
            {
                _targetStats.ApplyElements(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite) { _targetStats.SetupIgniteDamage(_fireDamage * .2f); }
        if (canApplyShock) { _targetStats.SetupShockDamage(_lightningDamage * .2f); }


        _targetStats.ApplyElements(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void SetupIgniteDamage(float _damage) => igniteDamage = _damage;
    public void SetupShockDamage(float _damage) => shockDamage = _damage;

    public void ApplyElements(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited || !isChilled || !isShocked;
        bool canApplyChill = !isIgnited || !isChilled || !isShocked;
        bool canApplyShock = !isIgnited || !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            elementTimer = 3;

            fx.IgniteFXFor(3);

            igniteIcon.SetActive(true);
            Invoke("DisableIcons", elementTimer);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            elementTimer = 6;

            fx.ChillFXFor(6);

            float slowPercentage = 0.2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, elementTimer);

            chillIcon.SetActive(true);
            Invoke("DisableIcons", elementTimer);
        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null) { return; }

                HitNearestTargetWithShock();
            }
        }

    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked) { return; }

        isShocked = _shock;
        elementTimer = 5;

        fx.ShockFXFor(5);

        shockIcon.SetActive(true);
        Invoke("DisableIcons", elementTimer);
    }

    private void HitNearestTargetWithShock()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
            {
                closestEnemy = transform;
            }
        }



        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            DecreasedHealthBy(igniteDamage);

            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    private void DisableIcons()
    {
        igniteIcon.SetActive(false);
        chillIcon.SetActive(false);
        shockIcon.SetActive(false);

    }

    #endregion

    public virtual void TakeDamage(float _damage)
    {
        DecreasedHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();

        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void DecreasedHealthBy(float _damage)
    {
        currentHealth -= _damage;

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region StatCalculations
    private float CheckTargetArmor(CharacterStats _targetStats, float totalDamage)
    {
        if (_targetStats.isChilled) { totalDamage = _targetStats.armor.GetValue() * .8f; }

        else { totalDamage -= _targetStats.armor.GetValue(); }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private float CheckTargetResistance(CharacterStats _targetStats, float totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        float totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked) { totalEvasion += 20; }

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }

    private bool canCrit()
    {
        float totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance) { return true; }
        else { return false; }
    }

    private float CalculateCriticalDamage(float _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return critDamage;
    }
    public float GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;

    #endregion

}
