using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceManager : MonoBehaviour
{

    private PlayerInput playerInput;
    private bool isConnected = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Check if a device is connected
        if (playerInput.devices.Count > 0 && isConnected == false)
        {
            SetupPlayerConfig();
        }
        else
        {
            Debug.Log("No device connected yet.");
        }
    }

    private void SetupPlayerConfig()
    {   
        int playerIndex = PlayerManager.Instance.GetPlayerCount();
        PlayerManager.Instance.AddPlayer(new PlayerConfig
        {
            playerIndex = playerIndex,
            inputDevice = playerInput.devices[0],
            playerName = "Player " + (playerIndex + 1),
            controlScheme = playerInput.currentControlScheme
        }
        );

        isConnected = true;
    }

    public void OnDeviceLost(PlayerInput pi)
    {
        PlayerManager.Instance.ChangeControlScheme(pi.currentControlScheme, pi.playerIndex);
        isConnected = false;
    }

    public void OnDeviceRegained(PlayerInput pi)
    {
        isConnected = true;
        PlayerManager.Instance.ChangeControlScheme(pi.currentControlScheme, pi.playerIndex);
    }

    public void OnDeviceChange(PlayerInput pi)
    {
        PlayerManager.Instance.ChangeControlScheme(pi.currentControlScheme, pi.playerIndex);
        isConnected = true;
    }

}
