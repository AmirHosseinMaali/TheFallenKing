using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;

    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLefts = new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
        {
            return;
        }

        if (!currentCrystal)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
            currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));
        }
        else
        {
            if (canMoveToEnemy)
            {
                return;
            }

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>().FinishCrystal();
            }
        }
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseStacks)
        {
            if (crystalLefts.Count > 0)
            {
                if (crystalLefts.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }

                cooldown = 0;

                GameObject crystalToSpawn = crystalLefts[crystalLefts.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLefts.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                if (crystalLefts.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefillCrystal();
                }

                return true;
            }
        }
        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLefts.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLefts.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0) { return; }

        cooldownTimer = multiStackCooldown;
        RefillCrystal();
    }
}
