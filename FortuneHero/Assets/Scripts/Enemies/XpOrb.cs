using UnityEngine;

public enum PickupType { XP, Coin }

[RequireComponent(typeof(SphereCollider))]
public class XpOrb : MonoBehaviour
{
    [Header("Type de pickup")]
    public PickupType type = PickupType.XP;

    [Header("Valeur par orb")]
    [Min(1)] public int xpPerOrb = 5;     // ← 5 XP par boule
    [Min(1)] public int coinsPerOrb = 10; // ← 10 coins par orb

    [Header("Attraction")]
    public float attractRadius = 4f;
    public float moveSpeed = 6f;

    private Transform target;

    void Update()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            if (Vector3.Distance(transform.position, player.transform.position) <= attractRadius)
                target = player.transform;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (type == PickupType.Coin)
        {
            PlayerComponent.Instance.UpdateCoins(coinsPerOrb);
        }
        else // XP
        {
            PlayerComponent.Instance.UpdateXP(xpPerOrb);
            //int level = Mathf.Max(1, PlayerPrefs.GetInt("Level", 1)); 
            //int xp = Mathf.Max(0, PlayerPrefs.GetInt("XP", 0));  
            //int skillUp = PlayerPrefs.GetInt("Skill", 0);

            //xp += Mathf.Max(0, xpPerOrb); 
            //while (xp >= RequiredXpForLevel(level))
            //{
            //    xp -= RequiredXpForLevel(level);
            //    level++;
            //    PlayerPrefs.SetInt("Skill", skillUp + 1); // Indicateur de montée de niveau
            //}

            //PlayerPrefs.SetInt("Level", level);
            //PlayerPrefs.SetInt("XP", xp);
            //PlayerPrefs.Save();
            //Debug.Log($"Niveau {level} avec {xp} XP (next: {RequiredXpForLevel(level)})");
        }

        gameObject.SetActive(false);
    }

    private static int RequiredXpForLevel(int level)
    {
        level = Mathf.Max(1, level);
        float need = 100f * Mathf.Pow(1.1f, level - 1);
        return Mathf.CeilToInt(need);
    }
    protected void ClearXp()
    {
        PlayerPrefs.SetInt("XP", 0);
        PlayerPrefs.Save();
    }
}
