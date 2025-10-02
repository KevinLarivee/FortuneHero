using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class Skill : MonoBehaviour
{
    public int playerLevel = 0; 
    private int skillPointsTotal;
    private int skillPointsRestants;

    public float baseDefense = 10f;
    public float baseSpeed = 100f;
    public int baseAttack = 10;
    public float baseDash = 2.5f;
    public int baseHP = 10;
    public int baseDistance = 20;

    public float currentDefense;
    public float currentSpeed;
    public int currentAttack;
    public float currentDash;
    public int currentHP;
    public int currentDistance;

    void Start()
    {
        skillPointsTotal = playerLevel;
        skillPointsRestants = skillPointsTotal;

        // initialiser avec les valeurs de base
        currentDefense = baseDefense;
        currentSpeed = baseSpeed;
        currentAttack = baseAttack;
        currentDash = baseDash;
        currentHP = baseHP;
        currentDistance = baseDistance;

        Debug.Log("SkillPoints Restants: " + skillPointsRestants);
    }

    // --------- ATTACK -------------
    public void UpgradeAttack()
    {
        if (skillPointsRestants > 0)
        {
            currentAttack += 3;
            skillPointsRestants--;
        }
    }

    public void DowngradeAttack()
    {
        if (currentAttack > baseAttack)
        {
            currentAttack -= 3;
            skillPointsRestants++;
        }
    }

    // --------- DEFENSE -------------
    public void UpgradeDefense()
    {
        if (skillPointsRestants > 0)
        {
            currentDefense += 0.5f;
            skillPointsRestants--;
        }
    }

    public void DowngradeDefense()
    {
        if (currentDefense > baseDefense)
        {
            currentDefense -= 0.5f;
            skillPointsRestants++;
        }
    }

    // --------- HP -------------
    public void UpgradeHP()
    {
        if (skillPointsRestants > 0)
        {
            currentHP += 10;
            skillPointsRestants--;
        }
    }

    public void DowngradeHP()
    {
        if (currentHP > baseHP)
        {
            currentHP -= 10;
            skillPointsRestants++;
        }
    }

    // --------- DISTANCE -------------
    public void UpgradeDistance()
    {
        if (skillPointsRestants > 0)
        {
            currentDistance += 5;
            skillPointsRestants--;
        }
    }

    public void DowngradeDistance()
    {
        if (currentDistance > baseDistance)
        {
            currentDistance -= 5;
            skillPointsRestants++;
        }
    }

    // --------- DASH -------------
    public void UpgradeDash()
    {
        if (skillPointsRestants > 0)
        {
            currentDash = Mathf.Max(0.1f, currentDash - 0.2f); 
            skillPointsRestants--;
        }
    }

    public void DowngradeDash()
    {
        if (currentDash < baseDash)
        {
            currentDash += 0.2f;
            skillPointsRestants++;
        }
    }

    // --------- SPEED -------------
    public void UpgradeSpeed()
    {
        if (skillPointsRestants > 0)
        {
            currentSpeed += 10f; // +1%
            skillPointsRestants--;
        }
    }

    public void DowngradeSpeed()
    {
        if (currentSpeed > baseSpeed)
        {
            currentSpeed -= 10f;
            skillPointsRestants++;
        }
    }
}
