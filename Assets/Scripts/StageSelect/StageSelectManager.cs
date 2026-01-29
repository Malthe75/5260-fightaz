using System.Collections.Generic;
using UnityEngine;
public class StageSelectManager : MonoBehaviour
{
    public static StageSelectManager Instance { get; private set; }
    private Dictionary<int, StageDefinition> stageSelections = new();
    public event System.Action<int, StageDefinition> OnStageSelected;
    public event System.Action<int> OnStageDeselected;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SelectStage(int stageIndex, StageDefinition stage)
    {
        stageSelections[stageIndex] = stage;
        OnStageSelected?.Invoke(stageIndex, stage);
        if(stageSelections.Count == 2)
        {
            GameManager.Instance.SetStage(stage);
            GameManager.Instance.LoadScene("TestFightScene");
        }
    }

    public void DeselectStage(int stageIndex)
    {
        if (stageSelections.ContainsKey(stageIndex))
        {
            stageSelections.Remove(stageIndex);
        }
        OnStageDeselected?.Invoke(stageIndex);
    }
}