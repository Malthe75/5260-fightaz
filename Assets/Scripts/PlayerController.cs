using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite[] poses;
    public KeyCode[] inputKeys;
    public string opponentTag;
    public int maxHealth = 100;
    public Image healthBar;

    private int currentPose = 0;
    private SpriteRenderer sr;
    private int health;
    private PlayerController opponent;
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = poses[0];
        health = maxHealth;
        opponent = GetComponentInChildren<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < inputKeys.Length; i++)
        {
            if(Input.GetKeyDown(inputKeys[i]))
            {
                ChangePose(i);

                if (i == 1) tryHit();
            }
        }

        if(health <= 0)
        {
            ChangePose(poses.Length- 1); // Change to the last pose (defeated)
        }
    }

    void ChangePose(int index)
    {
        if(index >= 0 && index < poses.Length)
        {
            currentPose = index;
            sr.sprite = poses[currentPose];
        }
    }

    void tryHit()
    {
        float distance = Mathf.Abs(transform.position.x - opponent.transform.position.x);
        if(distance < 2f)
        {
            opponent.TakeDamage(10); // Deal 10 damage to the opponent
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
        if(healthBar != null)
        {
            healthBar.fillAmount = (float)health / maxHealth;
        }

        ChangePose(3); // Change to the hurt pose
    }

}
