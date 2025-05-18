using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Image healthBarImage1;

    private float currentHealth;
    private float maxHealth = 100f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(amount + " damage taken!");
        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        healthBarImage1.fillAmount = currentHealth / maxHealth;
    }

}
