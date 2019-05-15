using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class for each individual card in the deck. The Card script also contains the CardDefinition class (which holds info about where
/// sprites are to be positioned on each rank of card, rank being: Ace, 2, 3, 4 etc) and the Decorator class (which holds info about the
/// decorators (image/ sprite of suits; for example spades, clubs etc as well the sprite letter_10) and pips described in the XML document.
/// A decorator is the suit of the card in sprite form and the pips are the large images of the suit and depends on the cards number, so for 10 of spades
/// it would have 10 large Spade pips.
/// </summary>

public class Card : MonoBehaviour
{
    //This will be defined later
}

[System.Serializable] // A Serializable class is able to be editied in the Inspector
// This class stores information about each decorator or pi from DeckXML
public class Decorator
{
    public string type; // For card pips, type = "pip"
    public Vector3 loc; // The location of the Sprite on the Card
    public bool flip = false; // Whether to flip the Sprite vertically
    public float scale = 1f; // The scale of the Sprite
}

[System.Serializable]
// This class stores information for each rank of card (rank means Ace, King, 2, 3 etc)
public class CardDefinition
{
    public string face; // Sprite to use for each face card
    public int rank; // The rank (1-12) of this card
    public List<Decorator> pips = new List<Decorator>(); // Pips used

    /* Recall that Pips are the decorator images like the clubs/ spades etc in the corners of each non-face card (ie numbered)
     */   
}

