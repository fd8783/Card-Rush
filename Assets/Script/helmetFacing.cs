using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helmetFacing : MonoBehaviour {

    public Sprite frontImg, backImg;
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FilpVer(int dir)
    {
        if (dir == 1) //up (back)
            spriteRenderer.sprite = backImg;
        else    //down (front)
            spriteRenderer.sprite = frontImg;
    }
}
