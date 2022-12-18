using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    //public Gradient gradient; Changes the colour of the healthbar fill as the amount of health decreases
    
    public void SetMaxHealth(int health) {
        slider.maxValue = health;
        slider.value = health;
    }
 
    public void SetHealth(int health) {
        slider.value = health;
    }
}
