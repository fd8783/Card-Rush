using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageTrigger : MonoBehaviour {

    public string targetTag = "Enemy";

    public float damage = 30f, pushForce = 30f;

    private float penatly, singleDirBonus;
    private bool isCurSingleDir;

    private Transform root;
    [SerializeField]
    private cardUsage cardUsingScript;

    private Collider2D col;

    private Rigidbody2D bodyRB;

	// Use this for initialization
	void Awake () {
        singleDirBonus = backGroundSetting.singleDirBonus;
        root = transform.root;
        cardUsingScript = root.GetComponent<cardUsage>();
        bodyRB = root.GetComponent<Rigidbody2D>();
        //Debug.Log(cardUsingScript);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(targetTag))
        {
            isCurSingleDir = cardUsingScript.isCurSingleDir();
            penatly = cardUsingScript.GetCurPenatly();
            if (isCurSingleDir)
                col.GetComponent<health>().Hurt(damage * singleDirBonus * penatly, bodyRB.velocity * singleDirBonus * pushForce);
            else
                col.GetComponent<health>().Hurt(damage * penatly, bodyRB.velocity * pushForce);
        }
    }
}
