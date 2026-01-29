using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceManager : MonoBehaviour
{
    public void OnDeviceLost(PlayerInput pi)
    {
        Debug.Log("DeviceManager: Device lost for player " + (pi.playerIndex + 1));
    }

    public void OnDeviceRegained(PlayerInput pi)
    {
        Debug.Log("DeviceManager: Device regained for player " + (pi.playerIndex + 1));
    }

    public void OnDeviceChange(PlayerInput pi)
    {
        Debug.Log("DeviceManager: Device changed for player " + (pi.playerIndex + 1) + " to " + pi.currentControlScheme);
    }

}
