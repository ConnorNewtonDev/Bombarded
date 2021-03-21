using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;

public class ReadyUpToggle : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed()
    {
        LobbyManager.instance.StartFight();
    }
}
