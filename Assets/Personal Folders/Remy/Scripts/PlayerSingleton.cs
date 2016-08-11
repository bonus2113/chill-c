using UnityEngine;
using System.Collections;

public class PlayerSingleton : MonoBehaviour {

    private static PlayerSingleton m_Instance = null; 
    public static PlayerSingleton Instance
    {
        get
        {
            return m_Instance;
        }
    }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Debug.LogWarning("Player instance already exists. Deleting...");
        }
    }
}
