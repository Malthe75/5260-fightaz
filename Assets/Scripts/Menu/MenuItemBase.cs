using UnityEngine;

public abstract class MenuItemBase : MonoBehaviour
{
    public abstract void Confirm();
}

public interface ICancelable
{
    void Cancel();
}


