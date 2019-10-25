using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SwitchCharactee : MonoBehaviour
{

    int selectedCharacter = 1;
    String characterName;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedCharacter = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedCharacter = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedCharacter = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedCharacter = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedCharacter--;
            if (selectedCharacter < 1)
            {
                selectedCharacter = 4;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            selectedCharacter++;
            if (selectedCharacter > 4)
            {
                selectedCharacter = 1;
            }
        }

        if (selectedCharacter == 1)
        {

            GameObject.Find("playersprite1").GetComponent<Renderer>().enabled = true;
            GameObject.Find("player-hurt-2").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-skip-3").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-idle-4").GetComponent<Renderer>().enabled = false;
            
        }
        else if (selectedCharacter == 2)
        {
            GameObject.Find("playersprite1").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-hurt-2").GetComponent<Renderer>().enabled = true;
            GameObject.Find("player-skip-3").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-idle-4").GetComponent<Renderer>().enabled = false;
        }
        else if (selectedCharacter == 3)
        {
            GameObject.Find("playersprite1").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-hurt-2").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-skip-3").GetComponent<Renderer>().enabled = true;
            GameObject.Find("player-idle-4").GetComponent<Renderer>().enabled = false;
        }
        else if (selectedCharacter == 4)
        {
            GameObject.Find("playersprite1").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-hurt-2").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-skip-3").GetComponent<Renderer>().enabled = false;
            GameObject.Find("player-idle-4").GetComponent<Renderer>().enabled = true;
        }

    }


}
