using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite[] poses;

    public string opponentTag;

    public int maxHealth = 100;

    public Image healthBar;

    private int currentPose = 0;
    private SpriteRenderer sr;
    private int health;
    private PlayerController opponent;

    [SerializeField] float moveSpeed = 5f;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = poses[0];
        health = maxHealth;
        opponent = GetComponentInChildren<PlayerController>();
    }



    void walk()
    {
        /*
        Vector3 move = Vector3(Input.x, 0, 0) * moveSpeed * Time.deltaTime;
        transform.position += move;
        */
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;

        //if(healthBar != null)
        //{
        //    healthBar.fillAmount = (float)health / maxHealth;
        //}
    }

}
