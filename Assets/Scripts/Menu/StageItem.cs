using UnityEngine;

public class StageItem : MenuItemBase, ICancelable
{
    [Header("Action")]
    [SerializeField] private StageDefinition stage; 
    [SerializeField] private bool quitGame;
    

    public override void Confirm(int stageIndex)
    {
        StageSelectManager.Instance.SelectStage(stageIndex, stage);
    }

    public void Cancel(int stageIndex)
    {
        StageSelectManager.Instance.DeselectStage(stageIndex);
    }
}
