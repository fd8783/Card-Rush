using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiDeck : MonoBehaviour {

    public int[] curDeck = { 0, 0, 0, 0, 0, 0, 1, 2, 3, 4 };
    [SerializeField]
    private int[] deckForShuffle;
    private int deckSize;
    private string cardName;

    private List<int> deck = new List<int>(), usedCard = new List<int>();
    //private int[] holdingCard = new int[4];   //don't apply ai to choose what to play now

    // Use this for initialization
    void Awake()
    {
        deckForShuffle = (int[]) curDeck.Clone();
        deckSize = deckForShuffle.Length;

        CardLoading();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string CardUsed()
    {
        cardName = backGroundSetting.allCardName[deck[0]];
        deck.RemoveAt(0);
        //check remain num of card
        if (deck.Count == 0)
        {
            CardLoading();
        }
        return cardName;
    }

    public void CardSet(int cardNo)
    {

    }

    private void CardLoading()
    {
        Shuffle(deckForShuffle);

        for (int i = 0; i < deckSize; i++)
            deck.Add(deckForShuffle[i]);
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
