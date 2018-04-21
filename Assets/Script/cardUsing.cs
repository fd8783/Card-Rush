using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardUsing : MonoBehaviour {

    //may be these two class sould put in weapon script and able to be set up for each weapon
    public float normalAttackDelay = 0.2f;
    public float normalAttackTime = 1f, DefenseTime = 1f, CounterTime = 1f, DashTime = 1f, HugeAttackTime = 2f;
    public float normalAttackForce = 100f;

    private float coolDownTime;

    private movementCtrl movementScript;
    private Transform curWeapon;
    private Animator weaponAnim;
    private Rigidbody2D bodyRB;

	// Use this for initialization
	void Awake () {
        movementScript = GetComponent<movementCtrl>();
        curWeapon = transform.Find("weapon").GetChild(0);  //first child should be using weapon, if want to vaild, we can use CompareTag as well
        weaponAnim = curWeapon.GetComponent<Animator>();
        bodyRB = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > coolDownTime)
            CheckKeyBoardInput();
    }

    void CheckKeyBoardInput()
    {
        if (Input.GetButtonDown("UseCard1"))
        {
            NormalAttack();
        }
    }

    void NormalAttack()
    {
        movementScript.StopRotate(normalAttackTime);
        StartCoroutine(ChargedNormalAttack(normalAttackDelay));
        coolDownTime = Time.time + normalAttackTime;
        weaponAnim.SetTrigger("normalAttack");
    }

    IEnumerator ChargedNormalAttack(float time)
    {
        yield return new WaitForSeconds(time);
        bodyRB.AddForce(movementCtrl.mouseDir * normalAttackForce);
    }
}
