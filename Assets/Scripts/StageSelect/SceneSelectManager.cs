using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSelectManager : MonoBehaviour
{
    [Header("Selection UI")]
    public List<Transform> stageIcons;
    public RectTransform p1Selector;
    public RectTransform p2Selector;

    private int p1Index = 0, p2Index = 0;
    private bool p1Locked = false, p2Locked = false;

    private bool p1MoveReleased = true;
    private bool p2MoveReleased = true;

    private CharacterSelectInputHandler p1Input;
    private CharacterSelectInputHandler p2Input;


    void Start()
    {
        // Find player input handlers from PlayerInput.all
        foreach (var player in PlayerInput.all)
        {
            var inputHandler = player.GetComponent<CharacterSelectInputHandler>();
            if (player.playerIndex == 0)
            {
                p1Input = inputHandler;
                p1Input.OnMove += HandleP1Move;
                p1Input.OnConfirm += HandleP1Confirm;
            }
            else if (player.playerIndex == 1)
            {
                p2Input = inputHandler;
                p2Input.OnMove += HandleP2Move;
                p2Input.OnConfirm += HandleP2Confirm;
            }
        }
    }
    private void HandleP1Move(Vector2 input)
    {
        if (!p1Locked)
        {
            if (Mathf.Abs(input.x) < 0.2f)
                p1MoveReleased = true;

            if (Mathf.Abs(input.x) > 0.5f && p1MoveReleased)
            {
                ChangeIndex(ref p1Index, input.x > 0 ? 1 : -1);
                p1MoveReleased = false;
            }
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
        if (!p2Locked)
        {
            if (Mathf.Abs(input.x) < 0.2f)
                p2MoveReleased = true;

            if (Mathf.Abs(input.x) > 0.5f && p2MoveReleased)
            {
                ChangeIndex(ref p2Index, input.x > 0 ? 1 : -1);
                p2MoveReleased = false;
            }
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
            // 50/50 chance between each map.
            StageData.selectedStageIndex = Random.value < 0.5f ? p1Index : p2Index;
            SceneManager.LoadScene("SampleScene");
            Debug.Log("Next Scene");
        }
    }

    private void ChangeIndex(ref int index, int direction)
    {
        int count = stageIcons.Count;
        index = (index + direction + count) % count;
    }
}