using UnityEngine;

public abstract class MenuItemBase : MonoBehaviour
{
    public abstract void Confirm(int playerIndex);
}

public interface ICancelable
{
    void Cancel(int playerIndex);
}

public interface IHoverable
{
    void Hover(int playerIndex);
}


