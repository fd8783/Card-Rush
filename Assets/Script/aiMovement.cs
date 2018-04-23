using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class aiMovement : MonoBehaviour {

    public Vector2 movingDir;

    public LayerMask targetLayer;
    private Vector2 areaLeftUpPT, areaRightDownPT;
    private Collider2D[] visibleTarget;
    private int visibleCount;
    public Transform curTarget, area, UIobj;

    private bool isPasued, againstPasue;

    private int[] curDir = { 1, -1 }, targetDir = { 1, -1 };//use 1,-1 to simulate the x,y-coordinate (1 for +, -1 for -)
    [SerializeField]
    private Vector2 curPos, targetPos, targetSca;
    private bool stopRotate = false;   // for attacking
    private float mouseAngle;

    private Transform helmet, weaponCtrl, curWeapon;
    private helmetFacing helmetScript;
    private SortingGroup curWeaponSorting;
    private int[] weaponDirSortingOrder = { 50, 130 }; // index 0 for up, index 1 for down
    private Quaternion weaponTargetRot;

    private Animator anim;
    private Rigidbody2D bodyRB;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        bodyRB = GetComponent<Rigidbody2D>();
        area = GameObject.Find("Main Camera/area").transform;
        areaLeftUpPT = area.Find("leftUpPT").position;
        areaRightDownPT = area.Find("rightDownPT").position;

        helmet = transform.Find("bodyImg/head/helmet");
        helmetScript = helmet.GetComponent<helmetFacing>();
        weaponCtrl = transform.Find("weapon");
        curWeapon = weaponCtrl.GetChild(0);  //first child should be using weapon, if want to vaild, we can use CompareTag as well
        curWeaponSorting = curWeapon.GetComponent<SortingGroup>();
        UIobj = transform.Find("UI");
    }

    // Update is called once per frame
    void Update()
    {
        curPos = transform.position;

        FindTarget();
        if (!stopRotate)
            Rotate();

    }

    public void Move()
    {

    }

    void FindTarget()
    {
        if (curTarget == null)
            curTarget = FindClosestTarget();
        if (curTarget != null)
        {
            targetPos = curTarget.position;
        }
    }

    void Rotate()
    {
        movingDir = targetPos - curPos;
        movingDir = movingDir.normalized;

        ChangeDir(targetDir, (int)Mathf.Sign(movingDir.x), (int)Mathf.Sign(movingDir.y));

        if (curDir[0] != targetDir[0])
            FilpHor(targetDir[0]);
        if (curDir[1] != targetDir[1])
            FilpVer(targetDir[1]);

        //*don't use curDir = targetDir, it change curDir's address point to targetDir because it's reference type
        ChangeDir(curDir, targetDir);

        //rotate Weapon                                            
        mouseAngle = Mathf.Atan2(movingDir.y, movingDir.x) * Mathf.Rad2Deg;
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

    Transform FindClosestTarget()
    {
        Transform closestTarget;
        float closestDis, newDis;
        int closestTargetNum;

        visibleTarget = Physics2D.OverlapAreaAll(areaLeftUpPT, areaRightDownPT, targetLayer);
        visibleCount = visibleTarget.Length;

        if (visibleCount > 0)
        {
            closestDis = Vector2.Distance(curPos, visibleTarget[0].transform.position);
            closestTargetNum = 0;
            closestTarget = visibleTarget[0].transform;
            for (int i = 1; i < visibleCount; i++)
            {
                newDis = Vector2.Distance(curPos, visibleTarget[i].transform.position);
                if (newDis < closestDis)
                {
                    closestDis = newDis;
                    closestTargetNum = i;
                }
            }
            closestTarget = visibleTarget[closestTargetNum].transform;
            return closestTarget;
        }
        else
        {
            Debug.Log("No Target Here");
            return null;
        }
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
        targetSca.x = dir * Mathf.Abs(targetSca.x);
        transform.localScale = targetSca;
        UIobj.localScale = targetSca;
    }

    void FilpVer(int dir)
    {
        curWeaponSorting.sortingOrder = weaponDirSortingOrder[dir == 1 ? 0 : 1];
        helmetScript.FilpVer(dir);
    }
}
