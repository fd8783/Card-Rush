using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class movementCtrl : MonoBehaviour {

    public static Vector2 mouseDir;

    private bool isPasued, againstPasue;
    
    private int[] curDir = { 1, -1 }, targetDir = { 1, -1 };//use 1,-1 to simulate the x,y-coordinate (1 for +, -1 for -)
    private Vector2 curPos, targetPos, targetSca;
    private bool stopRotate = false;   // for attacking
    private float mouseAngle;

    private Transform helmet, weaponCtrl, curWeapon;
    private helmetFacing helmetScript;
    private SortingGroup curWeaponSorting;
    private int[] weaponDirSortingOrder = { 50, 130}; // index 0 for up, index 1 for down
    private Quaternion weaponTargetRot;

    private Animator anim;
    private Rigidbody2D bodyRB;

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        bodyRB = GetComponent<Rigidbody2D>();

        helmet = transform.Find("bodyImg/head/helmet");
        helmetScript = helmet.GetComponent<helmetFacing>();
        weaponCtrl = transform.Find("weapon");
        curWeapon = weaponCtrl.GetChild(0);  //first child should be using weapon, if want to vaild, we can use CompareTag as well
        curWeaponSorting = curWeapon.GetComponent<SortingGroup>();
    }
	
	// Update is called once per frame
	void Update () {
        curPos = transform.position;

        if (!stopRotate)
            Rotate();

    }

    public void Move()
    {

    }

    void Rotate()
    {
        mouseDir = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition) - curPos;
        mouseDir = mouseDir.normalized;

        ChangeDir(targetDir, (int)Mathf.Sign(mouseDir.x), (int)Mathf.Sign(mouseDir.y));

        if (curDir[0] != targetDir[0])
            FilpHor(targetDir[0]);
        if (curDir[1] != targetDir[1])
            FilpVer(targetDir[1]);

        //*don't use curDir = targetDir, it change curDir's address point to targetDir because it's reference type
        ChangeDir(curDir, targetDir);

        //rotate Weapon                                            
        mouseAngle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        weaponTargetRot = Quaternion.Euler(new Vector3(0f, 0f, mouseAngle * transform.localScale.x + (transform.localScale.x > 0 ? 0 : 180)));
        weaponCtrl.localRotation = Quaternion.Lerp(weaponCtrl.localRotation, weaponTargetRot, 0.2f * Time.deltaTime * 60);

    }

    void Rotate(Vector2 dir)
    {
        ChangeDir(targetDir, (int)Mathf.Sign(dir.x), (int)Mathf.Sign(dir.y));

        if (curDir[0] != targetDir[0])
            FilpHor(targetDir[0]);
        if (curDir[1] != targetDir[1])
            FilpVer(targetDir[1]);

        //*don't use curDir = targetDir, it change curDir's address point to targetDir because it's reference type
        ChangeDir(curDir, targetDir);

        //rotate Weapon                                            
        mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weaponTargetRot = Quaternion.Euler(new Vector3(0f, 0f, mouseAngle * transform.localScale.x + (transform.localScale.x > 0 ? 0 : 180)));
        weaponCtrl.localRotation = weaponTargetRot;
    }

    public void StopRotate(float time)
    {
        stopRotate = true;
        StartCoroutine(RotateRecover(time));
    }

    public void ForceRotate(Vector2 forceTargetDir, float time)
    {
        stopRotate = true;
        Rotate(forceTargetDir);
        StartCoroutine(RotateRecover(time));
    }

    IEnumerator RotateRecover(float time)
    {
        yield return new WaitForSeconds(time);
        stopRotate = false;
    }

    //if targetDir is sth like Vector2 Array, it can't do like this because it pass by value, this int[] pass by reference
    void ChangeDir(int[] targetDir, int xDir, int yDir)
    {
        targetDir[0] = xDir;
        targetDir[1] = yDir;
    }

    void ChangeDir(int[] curDir, int[] targetDir)
    {
        curDir[0] = targetDir[0];
        curDir[1] = targetDir[1];
    }

    void FilpHor(int dir)
    {
        targetSca = transform.localScale;
        targetSca.x = dir* Mathf.Abs(targetSca.x);
        transform.localScale = targetSca;
    }

    void FilpVer(int dir)
    {
        curWeaponSorting.sortingOrder = weaponDirSortingOrder[dir == 1 ? 0 : 1];
        helmetScript.FilpVer(dir);
    }
    
}
