using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSelectManager : MonoBehaviour
{
    [Header("Input Handlers")]
    public CharacterSelectInputHandler p1Input;
    public CharacterSelectInputHandler p2Input;

    [Header("Selection UI")]
    public List<Transform> stageIcons;
    public RectTransform p1Selector;
    public RectTransform p2Selector;

    private int p1Index = 0, p2Index = 0;
    private bool p1Locked = false, p2Locked = false;

    private void Start()
    {
        p1Input.OnMove += HandleP1Move;
        p1Input.OnConfirm += HandleP1Confirm;

        p2Input.OnMove += HandleP2Move;
        p2Input.OnConfirm += HandleP2Confirm;
    }

    private void HandleP1Move(Vector2 input)
    {
        if (!p1Locked && Mathf.Abs(input.x) > 0.5f)
        {

            ChangeIndex(ref p1Index, input.x > 0 ? 1 : -1);
        }

    }

    private void HandleP1Confirm()
    {
        if (!p1Locked)
        {
            p1Locked = true;
            Debug.Log("P1 locked in: " + stageIcons[p1Index].name);

        }
    }

    private void HandleP2Move(Vector2 input)
    {
        if (!p2Locked && Mathf.Abs(input.x) > 0.5f)
        {
            ChangeIndex(ref p2Index, input.x > 0 ? 1 : -1);
        }
    }

    private void HandleP2Confirm()
    {
        if (!p2Locked)
        {
            p2Locked = true;
            Debug.Log("P2 locked in: " + stageIcons[p2Index].name);
        }
    }

    private void Update()
    {
        p1Selector.position = stageIcons[p1Index].position;
        p2Selector.position = stageIcons[p2Index].position;

        if (p1Locked && p2Locked)
        {
            Debug.Log("Both players locked. Load next scene here.");
            // SceneManager.LoadScene("FightScene");
            // 50 percent chance for one of the maps picked.
        }
    }

    private void ChangeIndex(ref int index, int direction)
    {
        int count = stageIcons.Count;
        index = (index + direction + count) % count;
    }
}
