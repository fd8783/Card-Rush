using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class stageCtrl : MonoBehaviour {

    public static int playerCount = 0,
                      enemyCount = 0;

    bool win = false, lose = false;

    private TextMeshProUGUI stageClear;
    public float showTextTime = 3f;

	// Use this for initialization
	void Awake () {
        stageClear = GameObject.Find("Main Camera/TextUI/stageClear").GetComponent<TextMeshProUGUI>();
        playerCount = 0;
        enemyCount = 0;
    }
	
    void Start()
    {
        //Debug.Log(playerCount + " " + enemyCount);
    }

	// Update is called once per frame
	void Update () {
        if (!(win || lose))
        {
            CheckWin();
            CheckLose();
        }
    }

    void CheckWin()
    {
        if (enemyCount == 0)
        {
            win = true;
            StartCoroutine(ShowText("Stage\n  Clear!"));
        }
    }

    void CheckLose()
    {
        if (playerCount == 0)
        {
            lose = true;
            StartCoroutine(ShowText("You\n  Die!"));
        }
    }

    IEnumerator ShowText(string text)
    {
        stageClear.SetText(text);
        yield return new WaitForSeconds(showTextTime);

        playerCount = 0;
        enemyCount = 0;

        if (win)
            backGroundSetting.NextStage();
        else
            backGroundSetting.GameOver();
    }

    public static void Join(string tag)
    {
        if (tag == "Player")
            playerCount++;
        if (tag == "Enemy")
            enemyCount++;
    }

    public static void Out(string tag)
    {
        if (tag == "Player")
            playerCount--;
        if (tag == "Enemy")
            enemyCount--;
    }
}
