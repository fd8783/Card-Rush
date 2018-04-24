using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deckCtrl : MonoBehaviour {
    [SerializeField]
    private List<int> curPlayerDeck;
    [SerializeField]
    private int[] deckForShuffle;
    private int deckSize;
    private string cardName;

    private Transform cardPos, deck, usedCard, cardInstance;
    private Transform[] cardContainer = new Transform[4];
    private Animator[] cardContainerAnim = new Animator[4];

	// Use this for initialization
	void Awake () {
        //Debug.Log(backGroundSetting.GetCurDeck().Count);
        deck = transform.Find("deck");
        usedCard = transform.Find("used");

        curPlayerDeck = backGroundSetting.GetCurDeck();
        deckForShuffle = curPlayerDeck.ToArray();
        deckSize = deckForShuffle.Length;
        cardPos = transform.Find("cardPos");
        for (int i =0; i< cardContainer.Length; i++)
        {
            cardContainer[i] = cardPos.GetChild(i).GetChild(0);
            cardContainerAnim[i] = cardPos.GetChild(i).GetComponent<Animator>();
        }

        CardLoading(true);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public string CardUsed(int cardNo)
    {
        if (cardContainer[cardNo].childCount == 0)
            return "noCardHere";
        cardContainerAnim[cardNo].SetTrigger("use");
        cardInstance = cardContainer[cardNo].GetChild(0);
        cardName = cardInstance.name;
        cardInstance.parent = usedCard;
        cardInstance.localPosition = Vector2.zero;
        cardInstance.localScale = Vector3.one;
        //check remain num of card
        if (deck.childCount > 1)
        {
            CardSet(cardNo);
        }
        else if (usedCard.childCount >= deckSize+1) //with a cover as first child
        {
            CardLoading(false);
        }
        return cardName.Replace("(Clone)", "");
    }

    public void CardSet(int cardNo)
    {
        cardInstance = deck.GetChild(1);
        cardInstance.parent = cardContainer[cardNo];
        cardInstance.localPosition = Vector2.zero;
        cardInstance.localScale = Vector3.one;
        cardContainerAnim[cardNo].SetTrigger("set");
    }

    private void CardLoading(bool firstTimeLoading)
    {
        Shuffle(deckForShuffle);
        if (firstTimeLoading)
        {
            for (int i = 0; i < deckSize; i++)
                Instantiate(Resources.Load("card/" + backGroundSetting.allCardName[deckForShuffle[i]]), deck);
        }
        else
        {
            for (int i = 0; i < deckSize; i++)
            {
                cardInstance = usedCard.GetChild(1);
                cardInstance.parent = deck;
                cardInstance.localPosition = Vector2.zero;
                cardInstance.localScale = Vector3.one;
            }
        }
        for (int i = 0; i< cardContainer.Length; i++)
        {
            CardSet(i);
        }
    }

    private void Shuffle(int[] array)
    {
        int curIndex = array.Length, randomIndex;
        int temp;

        while (curIndex > 0)
        {
            randomIndex = Random.Range(0, curIndex);
            curIndex--;
            temp = array[curIndex];
            array[curIndex] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
