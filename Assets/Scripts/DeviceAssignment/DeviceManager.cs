using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceManager : MonoBehaviour
{

    public void OnDeviceLost(PlayerInput pi)
    {
        Debug.Log(pi.currentControlScheme + " device lost for player " + pi.playerIndex+1);
    }

    public void OnDeviceRegained(PlayerInput pi)
    {
        Debug.Log(pi.currentControlScheme + " device regained for player " + pi.playerIndex+1);
    }

    public void OnDeviceChange(PlayerInput pi)
    {
        Debug.Log("Device change: " + pi.currentControlScheme + " for player " + pi.playerIndex+1);
    }

}
