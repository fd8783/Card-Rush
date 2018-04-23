using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardUsing : MonoBehaviour, cardUsage {
    
    private float normalAttackDelay = 0.2f, defenseDelay = 0.2f, counterDelay = 0.1f, dashDelay = 0.1f, specialAttackDelay = 0.5f;
    private float normalAttackTime = 1f, defenseTime = 1f, counterTime = 1f, dashTime = 1f, specialAttackTime = 2f;
    private float normalAttackForce = 100f, defenseForce = 100f, counterForce = 100f, dashForce = 200f, specialAttackForce = 100f;
    private float shieldPower = 10f;

    private float reloadStartTime, reloadFinishTime, coolDownTime, curTargetDelay, curTargetAttackTime, curTargetForce, mouseAngle, targetAngle;
    private bool singleDir =false;
    private Vector2 curTargetDir;

    private AudioSource[] soundList;
    private AudioSource beatSound, highBeatSound, grabBeatSound, dashSound;

    private deckCtrl deckCtrlScript;
    private movementCtrl movementScript;
    private Transform curWeapon;
    private weaponInfo weaponScript;
    private Animator weaponAnim;
    private Rigidbody2D bodyRB;
    private Image reloadImg, countBackBar;
    private float countBackTime, punishTime, maxLatePenalty, curPenalty;
    private bool reloaded = true, firstCardPlayed = false;
    private health healthScript;

	// Use this for initialization
	void Awake () {
        deckCtrlScript = GameObject.Find("deckCtrl").GetComponent<deckCtrl>();
        movementScript = GetComponent<movementCtrl>();
        bodyRB = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<health>();
        reloadImg = GameObject.Find("Main Camera/reload/img").GetComponent<Image>();
        countBackBar = GameObject.Find("Main Camera/reload/bar").GetComponent<Image>();
        countBackTime = backGroundSetting.GetCountBackTime();
        maxLatePenalty = backGroundSetting.GetMaxLatePenalty();
        soundList = GetComponents<AudioSource>();
        beatSound = soundList[0]; highBeatSound = soundList[1]; grabBeatSound = soundList[2]; dashSound = soundList[3];
        UpdateWeaponInfo();
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time > coolDownTime)
        {
            if (!reloaded)
            {
                reloaded = true;
                reloadFinishTime = Time.time;
                punishTime = reloadFinishTime + countBackTime;
                beatSound.Play();
            }
            if (Time.time > punishTime)
            {
                if (firstCardPlayed)
                    RandomUseACard();
                else
                    CheckKeyBoardInput();
            }
            else
            {
                if (firstCardPlayed)
                    UpdateCountBackBar();
                CheckKeyBoardInput();
            }
        }
        else
        {
            if (reloaded)
                reloaded = false;
            UpdateReloadImg();
            CheckEarlyInput();
        }
    }

    void UpdateReloadImg()
    {
        reloadImg.fillAmount = (Time.time - reloadStartTime) / (coolDownTime - reloadStartTime);
    }

    void UpdateCountBackBar()
    {
        countBackBar.fillAmount = (punishTime - Time.time) / (punishTime - reloadFinishTime);
    }

    void RandomUseACard()
    {
        highBeatSound.Play();
        CheckCardUsed(deckCtrlScript.CardUsed(Random.Range(0,4)));
    }

    void CheckEarlyInput()
    {
        if (Input.GetButtonDown("UseCard0"))
        {
            highBeatSound.Play();
            reloadStartTime = Time.time;
            coolDownTime = Time.time + curTargetAttackTime;
            return;
        }
        if (Input.GetButtonDown("UseCard1"))
        {
            highBeatSound.Play();
            reloadStartTime = Time.time;
            coolDownTime = Time.time + curTargetAttackTime;
            return;
        }
        if (Input.GetButtonDown("UseCard2"))
        {
            highBeatSound.Play();
            reloadStartTime = Time.time;
            coolDownTime = Time.time + curTargetAttackTime;
            return;
        }
        if (Input.GetButtonDown("UseCard3"))
        {
            highBeatSound.Play();
            reloadStartTime = Time.time;
            coolDownTime = Time.time + curTargetAttackTime;
            return;
        }
    }

    void CheckKeyBoardInput()
    {
        if (Input.GetButtonDown("UseCard0"))
        {
            grabBeatSound.Play();
            CheckCardUsed(deckCtrlScript.CardUsed(0));
            return;
        }
        if (Input.GetButtonDown("UseCard1"))
        {
            grabBeatSound.Play();
            CheckCardUsed(deckCtrlScript.CardUsed(1));
            return;
        }
        if (Input.GetButtonDown("UseCard2"))
        {
            grabBeatSound.Play();
            CheckCardUsed(deckCtrlScript.CardUsed(2));
            return;
        }
        if (Input.GetButtonDown("UseCard3"))
        {
            grabBeatSound.Play();
            CheckCardUsed(deckCtrlScript.CardUsed(3));
            return;
        }
    }

    void CheckCardUsed(string cardName)
    {
        if (cardName.Equals("noCardHere"))
            return;

        if (!firstCardPlayed)
            firstCardPlayed = true;
        //Debug.Log(cardName);
        curPenalty = (countBackBar.fillAmount) * maxLatePenalty + (1 - maxLatePenalty);
        //Debug.Log(curPenalty);
        switch (cardName.Substring(0,2))
        {
            case "NA":
                NormalAttack();
                break;
            case "DE":
                Defense();
                break;
            case "CO":
                Counter();
                break;
            case "DA":
                Dash();
                break;
            case "SA":
                HughAttack();
                break;
            case "WS":
                break;
            default:
                Debug.Log("can't read card type");
                curTargetForce = 0;
                break;
        }

        reloadStartTime = Time.time;
        coolDownTime = Time.time + curTargetAttackTime;

        //in C#'s substring, the second params means how long it take from string, not the end index :P
        singleDir = true;
        switch (cardName.Substring(cardName.Length - 2, 2))
        {
            case "AD":
                singleDir = false;
                movementScript.StopRotate(curTargetAttackTime);
                curTargetDir = movementCtrl.mouseDir;
                break;
            case "NE":
                curTargetDir = DirOptimize(movementCtrl.mouseDir, new Vector2(1, 1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            case "SE":
                curTargetDir = DirOptimize(movementCtrl.mouseDir, new Vector2(1, -1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            case "NW":
                curTargetDir = DirOptimize(movementCtrl.mouseDir, new Vector2(-1, 1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            case "SW":
                curTargetDir = DirOptimize(movementCtrl.mouseDir, new Vector2(-1, -1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            default:
                Debug.Log("can't read card Dir");
                break;
        }

        StartCoroutine(ChargedDash(curTargetDelay, curTargetDir, curTargetForce, curPenalty));
    }

    Vector2 DirOptimize(Vector2 mouseDir, Vector2 targetDir)
    {
        if (Mathf.Sign(mouseDir.x) == Mathf.Sign(targetDir.x) && Mathf.Sign(mouseDir.y) == Mathf.Sign(targetDir.y))
        {
            //two dir at same quarter
            return mouseDir;
        }

        //it's cross product, you should study harder
        bool isClockWise = Mathf.Sign(Vector2.Angle(mouseDir, targetDir) * Mathf.Sign(mouseDir.x * targetDir.y - mouseDir.y * targetDir.x)) == 1 ? true : false;
        //I didn't normalize the targetDir in above switch so that I can use 1/-1 in here's new Vector2
        if (isClockWise)
        {
            if (Mathf.Sign(targetDir.x) == Mathf.Sign(targetDir.y))
                return new Vector2(targetDir.x, 0);
            else
                return new Vector2(0, targetDir.y);
        }
        else
        {
            if (Mathf.Sign(targetDir.x) == Mathf.Sign(targetDir.y))
                return new Vector2(0, targetDir.y);
            else
                return new Vector2(targetDir.x, 0);
        }
    }

    void NormalAttack()
    {
        weaponAnim.SetTrigger("normalAttack");
        curTargetForce = normalAttackForce;
        curTargetDelay = normalAttackDelay;
        curTargetAttackTime = normalAttackTime;
    }

    void Defense()
    {
        weaponAnim.SetTrigger("defense");
        curTargetForce = defenseForce;
        curTargetDelay = defenseDelay;
        curTargetAttackTime = defenseTime;
        healthScript.ShieldUp(shieldPower);
    }

    void Counter()
    {
        weaponAnim.SetTrigger("counter");
        curTargetForce = counterForce;
        curTargetDelay = counterDelay;
        curTargetAttackTime = counterTime;
    }

    void Dash()
    {
        weaponAnim.SetTrigger("dash");
        curTargetForce = dashForce;
        curTargetDelay = dashDelay;
        curTargetAttackTime = dashTime;
    }

    void HughAttack()
    {
        weaponAnim.SetTrigger("hugeAttack");
        curTargetForce = specialAttackForce;
        curTargetDelay = specialAttackDelay;
        curTargetAttackTime = specialAttackTime;
    }


    IEnumerator ChargedDash(float delay, Vector2 dir, float force, float penalty)
    {
        yield return new WaitForSeconds(delay);
        dashSound.Play();
        bodyRB.AddForce(dir * force * penalty * (singleDir ? backGroundSetting.singleDirBonus:1)); //mouseDir stop updating once attack start
    }

    public float GetCurPenatly()
    {
        return curPenalty;
    }

    public bool isCurSingleDir()
    {
        return singleDir;
    }

    void UpdateWeaponInfo()
    {
        curWeapon = transform.Find("weapon").GetChild(0);  //first child should be using weapon, if want to vaild, we can use CompareTag as well
        weaponAnim = curWeapon.GetComponent<Animator>();
        weaponScript = curWeapon.GetComponent<weaponInfo>();
        float[] delay = weaponScript.GetDelay(),    time = weaponScript.GetTime(),   force = weaponScript.GetForce();
        normalAttackDelay = delay[0]; defenseDelay = delay[1]; counterDelay = delay[2]; dashDelay = delay[3]; specialAttackDelay = delay[4];
        normalAttackTime = time[0]; defenseTime = time[1]; counterTime = time[2]; dashTime = time[3]; specialAttackTime = time[4];
        normalAttackForce = force[0]; defenseForce = force[1]; counterForce = force[2]; dashForce = force[3]; specialAttackForce = force[4];
        shieldPower = weaponScript.GetShieldPower();
    }
}
