using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponInfo : MonoBehaviour {

    public float normalAttackDelay = 0.2f, defenseDelay = 0.2f, counterDelay = 0.1f, dashDelay = 0.1f, specialAttackDelay = 0.5f;
    public float normalAttackTime = 1f, defenseTime = 1f, counterTime = 1f, dashTime = 1f, specialAttackTime = 2f;
    public float normalAttackForce = 100f, defenseForce = 50f, counterForce = 50f, dashForce = 200f, specialAttackForce = 100f;
    public float shieldPower = 10f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float[] GetDelay()
    {
        float[] temp = { normalAttackDelay,defenseDelay,counterDelay,dashDelay,specialAttackDelay};
        return temp;
    }

    public float[] GetTime()
    {
        float[] temp = { normalAttackTime, defenseTime, counterTime, dashTime, specialAttackTime };
        return temp;
    }

    public float[] GetForce()
    {
        float[] temp = { normalAttackForce, defenseForce, counterForce, dashForce, specialAttackForce };
        return temp;
    }

    public float GetShieldPower()
    {
        return shieldPower;
    }
}
