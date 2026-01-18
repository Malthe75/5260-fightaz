using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeviceAssigner : MonoBehaviour
{

    public void OnDeviceLost(InputDevice device, InputAction.CallbackContext context)
    {
        Debug.Log("Device lost: " + device.displayName);
    }

    public void OnDeviceRegained(InputDevice device)
    {
        Debug.Log("Device regained: " + device.displayName);
    }

    public void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log("Device change: " + device.displayName + " Change: " + change.ToString());
    }
}
