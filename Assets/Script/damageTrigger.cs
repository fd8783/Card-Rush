using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageTrigger : MonoBehaviour {
	
	private List<Collider2D> touched = new List<Collider2D>();

	private Collider2D damageZone;
	private bool isDamaging = false, touchedListCleared =true;

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
		damageZone = GetComponent<Collider2D>();
        root = transform.root;
        cardUsingScript = root.GetComponent<cardUsage>();
        bodyRB = root.GetComponent<Rigidbody2D>();
		//Debug.Log(cardUsingScript);
    }
	
	// Update is called once per frame
	void Update () {
		isDamaging = damageZone.isActiveAndEnabled;
		if (!isDamaging)
		{
			if (!touchedListCleared)
			{
				Debug.Log(touched.Count);
				touched.Clear();
				touchedListCleared = true;
			}
		}
	}

    void OnTriggerStay2D(Collider2D col)
    {
		if (isDamaging && col.CompareTag(targetTag))
		{
			if (!touched.Contains(col))
			{
				if (touchedListCleared)
					touchedListCleared = false;
				touched.Add(col);
				isCurSingleDir = cardUsingScript.isCurSingleDir();
				penatly = cardUsingScript.GetCurPenatly();
				if (isCurSingleDir)
					col.GetComponent<health>().Hurt(damage * singleDirBonus * penatly, bodyRB.velocity * singleDirBonus * pushForce);
				else
					col.GetComponent<health>().Hurt(damage * penatly, bodyRB.velocity * pushForce);
			}
		}
	}
}
