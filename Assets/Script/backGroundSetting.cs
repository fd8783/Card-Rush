using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backGroundSetting : MonoBehaviour {

    public backGroundSetting instance = null;

    public static int curStage = 0; //0 for Menu

    public bool firstTime = true;

    private static float countBackTime = 0.6f,  //time that player must play a card after reloaded
                         maxLatePenalty = 0.4f;    //0.5f = 50%

    public static string[] allWeaponName = { "", "", "", "", "", "", "", "", "", "",   
                                          "", "", "", "", "", "", "", "", "", "",},

                           //AD all direction, NE north west, SE south east
                           allCardName = { "NA_AD", "NA_NE", "NA_NW", "NA_SW", "NA_SE", "", "", "", "", "",         //NA normal attack 
                                           "DE_AD", "DE_NE", "DE_NW", "DE_SW", "DE_SE", "", "", "", "", "",         //DE defense
                                           "CO_AD", "CO_NE", "CO_NW", "CO_SW", "CO_SE", "", "", "", "", "",         //CO counter
                                           "DA_AD", "DA_NE", "DA_NW", "DA_SW", "DA_SE", "", "", "", "", "",         //DA dash
                                           "SA_HUGE_AD", "SA_HUGE_NE", "SA_HUGE_NW", "SA_HUGE_SW", "SA_HUGE_SE", "", "", "", "", "",         //SA special attack
                                           "", "", "", "", "", "", "", "", "", "",};       //WS weapon skill


    private static List<int> cardPlayerEnable = new List<int>(),
                             cardPlayerDisable = new List<int>(),
                             weaponPlayerEnable = new List<int>(),
                             weaponPlayerDisable = new List<int>();

    //default deck
    public static List<int> curPlayerDeck = new List<int> { 0,0,0,10,20,20,30,31,32,0,0,0,1,2,3,4,41,10,10,43 };
    private static int curPlayerWeapon;

	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        //Debug.Log(allCardName.Length + " " + cardPlayerAbleToUse.Length + " " + curPlayerDeck.Count);

        //don't know what happen, can't reach this
        if (firstTime)
            SetUpCardAndWeapon();
    }
	
	// Update is called once per frame
	void Update () {
        CheckExc();

    }

    void SetUpCardAndWeapon()
    {
        for (int i = 0; i < 5; i++)
            cardPlayerEnable.Add(i);
        for (int i = 5; i < allCardName.Length; i++)
            cardPlayerDisable.Add(i);
        for (int i = 0; i < 1; i++)
            weaponPlayerEnable.Add(i);
        for (int i = 1; i < allWeaponName.Length; i++)
            weaponPlayerDisable.Add(i);

    }

    public static List<int> GetCurDeck()
    {
        return curPlayerDeck;
    }

    public static float GetCountBackTime()
    {
        return countBackTime;
    }

    public static float GetMaxLatePenalty()
    {
        return maxLatePenalty;
    }

    public static void NextStage()
    {
        curStage++;
        SceneManager.LoadScene(curStage);
    }

    public static void GameOver()
    {
        curStage = 0;
        SceneManager.LoadScene(curStage);
    }

    public void StartGame()
    {
        if (firstTime)
        {
            curStage = 1;
            SceneManager.LoadScene(1);  //Tutorial
            firstTime = false;
        }
        else
        {
            curStage = 2;
            SceneManager.LoadScene(2);  //Skip Tutorial
        }
    }

    public void CheckExc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
