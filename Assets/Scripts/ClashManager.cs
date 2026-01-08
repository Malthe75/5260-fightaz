using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ClashSpriteEntry
{
    public Sprite sprite;
    public string value;
}
public class ClashManager : MonoBehaviour
{
    private PlayerClash p1;
    private PlayerClash p2;
    [SerializeField] List<ClashSpriteEntry> clashSprites;
    private List<ClashSpriteEntry> clashList = new List<ClashSpriteEntry>();

    public void Initialize(PlayerClash player1, PlayerClash player2)
    {
        p1 = player1;
        p2 = player2;
    }

    private void Update()
    {
        if(p1.shouldClash && p2.shouldClash)
        {
            StartClash();
        }if(p1.wonClash || p2.wonClash)
        {
            Endclash();
        }
    }

    private void RandomizeSprites()
    {
        for (int i = 0; i < 4; i++)
        {
            int r = Random.Range(0, 4);
            clashList.Add(clashSprites[r]);
        }
    }
    

    public void StartClash()
    {
        RandomizeSprites();
        p1.PlayClash(clashList);
        p2.PlayClash(clashList);
        FightManagerTest.Instance.SwitchAllActionMaps("Clash");
        FightManagerTest.Instance.ChangeAllStates("Clash");
    }

    private void Endclash()
    {
        clashList.Clear();
        p1.ResetClash();
        p2.ResetClash();
        FightManagerTest.Instance.SwitchAllActionMaps("Player");
    }
}