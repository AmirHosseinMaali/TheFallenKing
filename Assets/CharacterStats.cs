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


    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    private float elementTimer;

    private float ignitedDamageCooldown = .3f;
    private float ignitedDamageTimer;
    private float igniteDamage;


    public float currentHealth;

    public System.Action onHealthChanged;

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
            //DisableIcons();

            isIgnited = false;
            isChilled = false;
            isShocked = false;
        }

        if (ignitedDamageTimer < 0 && isIgnited)
        {
            DecreasedHealthBy(igniteDamage);

            if (currentHealth < 0) { Die(); }
            ignitedDamageTimer = ignitedDamageCooldown;
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

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        float _fireDamage = fireDamage.GetValue();
        float _iceDamage = iceDamage.GetValue();
        float _lightningDamage = lightningDamage.GetValue();

        float totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetMagicalResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0) { return; }

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


        _targetStats.ApplyElements(canApplyIgnite, canApplyChill, canApplyShock);

    }

    public void SetupIgniteDamage(float _damage) => igniteDamage = _damage;

    private static float CheckTargetMagicalResistance(CharacterStats _targetStats, float totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyElements(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked) { return; }

        if (_ignite)
        {
            isIgnited = _ignite;
            elementTimer = 3;

            fx.IgniteFXFor(3);

            igniteIcon.SetActive(true);
        }
        if (_chill)
        {
            isChilled = _chill;
            elementTimer = 6;

            fx.ChillFXFor(6);

            chillIcon.SetActive(true);
        }
        if (_shock)
        {
            isShocked = _shock;
            elementTimer = 5;

            fx.ShockFXFor(5);

            shockIcon.SetActive(true);
        }
    }

    public virtual void TakeDamage(float _damage)
    {
        DecreasedHealthBy(_damage);

        if (currentHealth <= 0)
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

    }

    private float CheckTargetArmor(CharacterStats _targetStats, float totalDamage)
    {
        if (_targetStats.isChilled) { totalDamage = _targetStats.armor.GetValue() * .8f; }

        else { totalDamage -= _targetStats.armor.GetValue(); }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
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

    private void DisableIcons()
    {
        igniteIcon.SetActive(false);
        chillIcon.SetActive(false);
        shockIcon.SetActive(false);
    }
}
