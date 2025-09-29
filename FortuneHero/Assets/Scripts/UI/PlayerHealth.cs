using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Image healthBar;  

    void Start()
    {
        currentHealth = maxHealth;


        healthBar.fillAmount = 1f;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.fillAmount = currentHealth / (float)maxHealth; ;

        if (currentHealth <= 0)
        {
            //
        }
    }

  
}
