using UnityEngine;

public class StageItem : MenuItemBase, ICancelable
{
    [Header("Action")]
    [SerializeField] private StageDefinition stage; 
    [SerializeField] private bool quitGame;
    

    public override void Confirm(int playerIndex = 0)
    {
        if (!string.IsNullOrEmpty("fdasf"))
        {
            GameManager.Instance.LoadScene("gfdsfd");
        }
        else if (quitGame)
        {
            Application.Quit();
        }
    }

    public void Cancel(int playerIndex)
    {
        // Implement cancel logic if needed
    }
}
