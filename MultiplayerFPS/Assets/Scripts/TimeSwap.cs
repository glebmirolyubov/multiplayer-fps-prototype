using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSwap : MonoBehaviour {
    
    private bool isDesert;
	
	void Update () {
        if (PauseMenu.IsOn) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && isDesert)
        {
            transform.position = new Vector3(transform.position.x + 1000f, transform.position.y, transform.position.z + 1000f);
            isDesert = false;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !isDesert)
        {
            transform.position = new Vector3(transform.position.x - 1000f, transform.position.y, transform.position.z - 1000f);
            isDesert = true;
        }
    }

	void OnTriggerEnter(Collider timeTrigger)
	{
        if (timeTrigger.name == "DesertTrigger") {
            isDesert = true;
        } else if (timeTrigger.name == "WinterTrigger") {
            isDesert = false;
        }
	}

}
