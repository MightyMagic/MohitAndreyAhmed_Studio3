using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreRoomOutside : MonoBehaviour
{
    [SerializeField] LoreRoomManager loreManager;
    //[SerializeField] GameObject disableTrigger;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           loreManager.DisableLoreRoom();
        }
    }
}