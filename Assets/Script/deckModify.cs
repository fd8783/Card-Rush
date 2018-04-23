using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class deckModify : MonoBehaviour {

    public List<int> curDeck;

    public string defaultDeckString, curDeckString;

    public InputField deckNumInput;

    private GameObject popUp;
    private Text popUpText;
    private string popUpErrorText;

	// Use this for initialization
	void Awake () {
        curDeck = backGroundSetting.GetCurDeck();
        deckNumInput = transform.Find("area/curDeck").GetComponent<InputField>();
        defaultDeckString = "";

        foreach (int No in curDeck)
            defaultDeckString += No + ",";
        defaultDeckString = defaultDeckString.Substring(0, defaultDeckString.Length - 1);   //get the last "," out

        curDeckString = string.Copy(defaultDeckString);

        deckNumInput.text = curDeckString;

        popUp = transform.Find("area/popUp").gameObject;
        popUpText = popUp.transform.Find("Text").GetComponent<Text>();
        popUpErrorText = popUpText.text;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ConfirmClicked()
    {
        List<int> checkNewList = new List<int>();
        int num;
        string[] splitNum;
        curDeckString = deckNumInput.text;

        splitNum = curDeckString.Split(',');
        foreach (string numString in splitNum)
        {
            if (numString.Equals(""))
                continue;
            if (int.TryParse(numString, out num))
            {
                if (num >= 0 && num < backGroundSetting.allCardName.Length)
                {
                    if (!backGroundSetting.allCardName[num].Equals(""))
                        checkNewList.Add(num);
                }
            }
            else
            {
                //wrong input
                Debug.Log("wrong"+ numString);
                popUp.SetActive(true);
                popUpText.text = popUpErrorText;
                return;
            }
        }
        if (checkNewList.Count < 10)
        {
            popUp.SetActive(true);
            popUpText.text = "You must enter at least 10 cards for a deck!";
            return;
        }

        popUp.SetActive(true);
        string newDeckString = "Change Deck as [";
        foreach (int newCardNum in checkNewList)
            newDeckString += newCardNum.ToString() + ",";
        newDeckString = newDeckString.Substring(0, newDeckString.Length - 1) + "]\n";
        popUpText.text = newDeckString;

        backGroundSetting.UpdateCurDeck(checkNewList);

        StartCoroutine(BackToMenu(2));
    }

    public void DefaultClicked()
    {
        deckNumInput.text = defaultDeckString;
    }

    IEnumerator BackToMenu(float time)
    {
        yield return new WaitForSeconds(time);
        backGroundSetting.GameOver();
    }
}
