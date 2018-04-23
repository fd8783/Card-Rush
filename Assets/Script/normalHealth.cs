using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class normalHealth : MonoBehaviour, health {

    public float hp, maxhp = 100f, crushCoefficient, baseCrushVel = 0.5f, maxShieldCount = 100f;
    private float shieldCount;

    public Image healthBarImg, shieldBarImg;

    private bool isDeath = false;

    private int fragmentLayer = (1 >> 11);

    private Transform head, body;
    private Rigidbody2D headRB, bodyRB, thisRB;

    private MonoBehaviour[] allScripts;

    // Use this for initialization
    void Awake() {


        Recover();
        thisRB = GetComponent<Rigidbody2D>();
        head = transform.Find("bodyImg/head");
        headRB = head.GetComponent<Rigidbody2D>();
        body = transform.Find("bodyImg/body");
        bodyRB = body.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        stageCtrl.Join(tag);
    }

    // Update is called once per frame
    void Update() {
        if (hp <= 0)
        {
            stageCtrl.Out(tag);
            isDeath = true;
            if (Mathf.Abs(hp) > 1)
                Crushed(baseCrushVel + Mathf.Log(Mathf.Abs(hp), 2) * crushCoefficient);
            else
                Crushed(Random.Range(baseCrushVel * 0.5f, baseCrushVel));
        }
        else
        {
            healthbarupdate();
            if (shieldBarImg != null)
                shieldBarUpdate();
        }

    }

    void healthbarupdate()
    {
        healthBarImg.fillAmount = hp / maxhp;
        //healthBarImg.color = Color.HSVToRGB((howmuchgreen * (hp / maxhp) / 359), 1, (float)200 / 255);
    }

    void shieldBarUpdate()
    {
        shieldBarImg.fillAmount = shieldCount / maxShieldCount;
    }

    public void Hurt(float damage)
    {
        if (shieldCount > 0)
        {
            if (damage > shieldCount)
            {
                damage -= shieldCount;
                shieldCount = 0;
                hp -= damage;
            }
            else
            {
                shieldCount -= damage;
            }
        }
        else
        {
            hp -= damage;
        }
    }

    public void Hurt(float damage, Vector2 vel)
    {
        if (shieldCount > 0)
        {
            if (damage > shieldCount)
            {
                damage -= shieldCount;
                shieldCount = 0;
                hp -= damage;
                thisRB.AddForce(vel);
            }
            else
            {
                shieldCount -= damage;
                thisRB.AddForce(vel*0.5f);
            }
        }
        else
        {
            hp -= damage;
            thisRB.AddForce(vel);
        }
    }

    public void ShieldUp(float amount)
    {
        shieldCount = Mathf.Min(shieldCount+amount, 100f);
    }

    public void Recover()
    {
        hp = maxhp;
    }

    public void Crushed(float coefficient)
    {
        //seprate to head and body two parts
        //head 
        head.gameObject.layer = fragmentLayer;
        head.tag = "Fragment";
        head.GetComponent<CircleCollider2D>().enabled = true;
        headRB.WakeUp();
        head.parent = null;

        //body 
        body.gameObject.layer = fragmentLayer;
        body.tag = "Fragment";
        body.GetComponent<CircleCollider2D>().enabled = true;
        bodyRB.WakeUp();
        body.parent = null;

        //disable all script
        allScripts = gameObject.GetComponents<MonoBehaviour>();
        for (int i = 0; i < allScripts.Length; i++)
        {
            allScripts[i].enabled = false;
        }

        //if needed, bounce them out
        if (coefficient > 0)
        {
            Vector2 randomVel;

            randomVel = Random.insideUnitCircle * coefficient;
            headRB.AddForce(randomVel);
            randomVel = Random.insideUnitCircle * coefficient;
            bodyRB.AddForce(randomVel);
        }
        Destroy(gameObject);
    }
}
