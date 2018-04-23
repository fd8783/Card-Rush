using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageTrigger : MonoBehaviour {

    public string targetTag = "Enemy";

    public float damage = 30f, pushForce = 30f;

    private float penatly;

    private Transform root;
    [SerializeField]
    private cardUsage cardUsingScript;

    private Collider2D col;

    private Rigidbody2D bodyRB;

	// Use this for initialization
	void Awake () {
        root = transform.root;
        cardUsingScript = root.GetComponent<cardUsage>();
        bodyRB = root.GetComponent<Rigidbody2D>();
        Debug.Log(cardUsingScript);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(targetTag))
        {
            penatly = cardUsingScript.GetCurPenatly();
            col.GetComponent<health>().Hurt(damage * penatly, bodyRB.velocity * pushForce);
        }
    }
}
