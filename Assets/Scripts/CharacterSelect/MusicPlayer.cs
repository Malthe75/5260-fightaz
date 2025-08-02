using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    void Awake()
    {
        // Check if there's already a MusicPlayer in the scene
        if (FindObjectsOfType<MusicPlayer>().Length > 1)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
