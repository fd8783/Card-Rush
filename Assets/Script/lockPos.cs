using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockPos : MonoBehaviour {

    private Vector3 startPos, filpPos, curSca;
    private Transform checkPT;

	// Use this for initialization
	void Awake () {
        startPos = transform.position;
        checkPT = transform.root.Find("bodyImg/body/handCheckPT");

    }
	
	// Update is called once per frame
	void LateUpdate() {
        transform.position = checkPT.position;
	}
}
