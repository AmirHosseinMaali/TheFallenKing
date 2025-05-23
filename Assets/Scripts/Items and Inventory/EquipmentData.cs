using System.Collections.Generic;
using UnityEngine;


public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class EquipmentData : ItemData
{
    public EquipmentType equipmentType;

    [Header("Major Stats")]
    public float strength; // 1 point increase damage by 1 and crit.power by 1%
    public float agility;  // 1 point increase evasion by 1% and crit.chance by 1%
    public float intelligence; // 1 point increase magic damage by 1 and magic resistance by 3
    public float vitality; // 1 point increased health by 3 or 5 points

    [Header("Offensive Stats")]
    public float damage;
    public float critChance;
    public float critPower;   // default value 150%

    [Header("Defensive Stats")]
    public float health;
    public float armor;
    public float evasion;
    public float magicResistance;

    [Header("Magic Stats")]
    public float fireDamage;  // do damage over time
    public float iceDamage;   // reduce armor 20%
    public float lightningDamage;  //reduce accuracy 20%

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;


    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);


        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);


        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);


        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }




}
