using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The Prospector class manages the overall game. Whereas Deck handles the creation of cards. Prospector turns those cards into a game.
/// Prospector collects the cards into various piles (like the draw pile and discard pile).
/// </summary>

public class Prospector : MonoBehaviour
{
    public static Prospector S; //Singleton

    [Header("Set in Insepctor")]
    public TextAsset deckXML;

    [Header("Set Dynamically")]
    public Deck deck;

    private void Awake()
    {
        S = this; // Set up a Singleton for Prospector
    }

    private void Start()
    {
        deck = GetComponent<Deck>(); // Get the Deck
        deck.InitDeck(deckXML.text); // Pass the full text associated with DeckXML to it
        if (deck != null)
        {
            Deck.Shuffle(ref deck.cards); // This shuffles the deck by reference
            Card card;
            // The loop repositions the cards after the shuffle has occured
            for (int cNum = 0; cNum < deck.cards.Count; cNum++)
            {
                card = deck.cards[cNum];
                card.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 12 * 4, 0);
            }
        }
    }
}
