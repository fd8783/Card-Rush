﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickAnyWhereToDisable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
            gameObject.SetActive(false);
	}
}
