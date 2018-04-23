using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aiCardUsing : MonoBehaviour, cardUsage {

    public float slowFactor = 2f;

    private bool doInput = false;

    private float normalAttackDelay = 0.2f, defenseDelay = 0.2f, counterDelay = 0.1f, dashDelay = 0.1f, specialAttackDelay = 0.5f;
    private float normalAttackTime = 1f, defenseTime = 1f, counterTime = 1f, dashTime = 1f, specialAttackTime = 2f;
    private float normalAttackForce = 100f, defenseForce = 100f, counterForce = 100f, dashForce = 200f, specialAttackForce = 100f;

    private float reloadStartTime, reloadFinishTime, coolDownTime, curTargetDelay, curTargetAttackTime, curTargetForce, mouseAngle, targetAngle;
    private Vector2 curTargetDir;

    private aiDeck deckCtrlScript;
    private aiMovement movementScript;
    private Transform curWeapon;
    private weaponInfo weaponScript;
    private Animator weaponAnim;
    private Rigidbody2D bodyRB;
    private Image reloadImg, countBackBar;
    private float countBackTime, punishTime, maxLatePenalty, curPenalty;
    private bool reloaded = true, firstCardPlayed = false;

    // Use this for initialization
    void Awake()
    {
        deckCtrlScript = GetComponent<aiDeck>();
        movementScript = GetComponent<aiMovement>();
        bodyRB = GetComponent<Rigidbody2D>();
        reloadImg = transform.Find("UI/reload/img").GetComponent<Image>();
        countBackBar = transform.Find("UI/reload/bar").GetComponent<Image>();
        countBackTime = backGroundSetting.GetCountBackTime() * slowFactor;
        maxLatePenalty = backGroundSetting.GetMaxLatePenalty();
        UpdateWeaponInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > coolDownTime)
        {
            if (!reloaded)
            {
                reloaded = true;
                reloadFinishTime = Time.time;
                punishTime = reloadFinishTime + countBackTime;
            }
            if (Time.time > punishTime)
            {
                ForceToUseACard();
            }
            else
            {
                UpdateCountBackBar();
                CheckInput();
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

    IEnumerator SimulateInput(float time)
    {
        doInput = false;
        yield return new WaitForSeconds(time);
        doInput = true;
    }

    void UpdateReloadImg()
    {
        reloadImg.fillAmount = (Time.time - reloadStartTime) / (coolDownTime - reloadStartTime);
    }

    void UpdateCountBackBar()
    {
        countBackBar.fillAmount = (punishTime - Time.time) / (punishTime - reloadFinishTime);
    }

    void CheckEarlyInput()
    {
        if (doInput)
        {
            reloadStartTime = Time.time;
            coolDownTime = Time.time + curTargetAttackTime;
            //Debug.Log("TooFAST");
            StartCoroutine(SimulateInput(Random.Range(curTargetAttackTime*0.9f,curTargetAttackTime+maxLatePenalty*1.1f)));
        }
    }

    void CheckInput()
    {
        if (doInput)
        {
            CheckCardUsed(deckCtrlScript.CardUsed());
            StartCoroutine(SimulateInput(Random.Range(curTargetAttackTime * 0.9f, curTargetAttackTime + maxLatePenalty * 1.1f)));
        }
    }

    void ForceToUseACard()
    {
        CheckCardUsed(deckCtrlScript.CardUsed());
        StartCoroutine(SimulateInput(Random.Range(curTargetAttackTime * 0.9f, curTargetAttackTime + maxLatePenalty * 1.1f)));
    }

    void CheckCardUsed(string cardName)
    {
        if (cardName.Equals("noCardHere"))
            return;

        //Debug.Log(cardName);
        curPenalty = (countBackBar.fillAmount) * maxLatePenalty + (1 - maxLatePenalty);
        //Debug.Log(curPenalty);
        switch (cardName.Substring(0, 2))
        {
            case "NA":
                NormalAttack();
                break;
            case "DE":
                break;
            case "CO":
                break;
            case "DA":
                break;
            case "SA":
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
        switch (cardName.Substring(cardName.Length - 2, 2))
        {
            case "AD":
                movementScript.StopRotate(curTargetAttackTime);
                curTargetDir = movementScript.movingDir;
                break;
            case "NE":
                curTargetDir = DirOptimize(movementScript.movingDir, new Vector2(1, 1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            case "SE":
                curTargetDir = DirOptimize(movementScript.movingDir, new Vector2(1, -1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            case "NW":
                curTargetDir = DirOptimize(movementScript.movingDir, new Vector2(-1, 1));
                movementScript.ForceRotate(curTargetDir, curTargetAttackTime);
                break;
            case "SW":
                curTargetDir = DirOptimize(movementScript.movingDir, new Vector2(-1, -1));
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

    IEnumerator ChargedDash(float delay, Vector2 dir, float force, float penalty)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log(dir.ToString("F4") + "   " + force + "   " + penalty);
        bodyRB.AddForce(dir * force * penalty); //mouseDir stop updating once attack start
    }

    public float GetCurPenatly()
    {
        return curPenalty;
    }

    void UpdateWeaponInfo()
    {
        curWeapon = transform.Find("weapon").GetChild(0);  //first child should be using weapon, if want to vaild, we can use CompareTag as well
        weaponAnim = curWeapon.GetComponent<Animator>();
        weaponScript = curWeapon.GetComponent<weaponInfo>();
        float[] delay = weaponScript.GetDelay(), time = weaponScript.GetTime(), force = weaponScript.GetForce();
        normalAttackDelay = delay[0]; defenseDelay = delay[1]; counterDelay = delay[2]; dashDelay = delay[3]; specialAttackDelay = delay[4];
        normalAttackTime = time[0]* slowFactor; defenseTime = time[1] * slowFactor; counterTime = time[2] * slowFactor; dashTime = time[3] * slowFactor; specialAttackTime = time[4] * slowFactor;
        normalAttackForce = force[0]; defenseForce = force[1]; counterForce = force[2]; dashForce = force[3]; specialAttackForce = force[4];
    }
}
