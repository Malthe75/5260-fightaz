using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Image healthBarImage1;
    public Image healthBarImage2;

    private float currentHealth1;
    private float currentHealth2;
    private float maxHealth1 = 100f; // Might need to be changed to make more sense? 
    private float maxHealth2 = 100f;

    // MAKE takedamage1 and 2 new names and make them private.
    private void Start()
    {
        currentHealth1 = maxHealth1;
        currentHealth2 = maxHealth2;
        UpdateHealthBar();
    }

    public void TakeDamage(float amount, string playerName)
    {
        switch (playerName)
        {
            case "Player1":
                TakeDamage1(amount);
                break;
            case "Player2":
                TakeDamage2(amount);
                break;
        }
    }
    private void TakeDamage1(float amount)
    {
        currentHealth1 -= amount;
        currentHealth1 = Mathf.Clamp(currentHealth1, 0, maxHealth1);
        Debug.Log(amount + " damage taken!   1");
        UpdateHealthBar();
    }
    private void TakeDamage2(float amount)
    {
        currentHealth2 -= amount;
        currentHealth2 = Mathf.Clamp(currentHealth2, 0, maxHealth2);
        Debug.Log(amount + " damage taken!   2");
        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        healthBarImage1.fillAmount = currentHealth1 / maxHealth1;
        healthBarImage2.fillAmount = currentHealth2 / maxHealth2;
    }

}
