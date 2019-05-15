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
    }
}
