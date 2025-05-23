using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    private void Start()
    {
        Instantiate(player1, new Vector3(0 * 2.0f, 0, 0), Quaternion.identity);
        Instantiate(player2, new Vector3(0 * 2.0f, 0, 0), Quaternion.identity);

    }
}
