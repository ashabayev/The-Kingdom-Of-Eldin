using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider myHealthBar;
    int maxHealth = 100;
    // Start is called before the first frame update
    void Start()
    {
        myHealthBar.value = maxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.A))
        {
            myHealthBar.value += 1;
        }
        if (Input.GetKeyDown (KeyCode.S))
        {
            myHealthBar.value -= 1;
        }
    }
}
