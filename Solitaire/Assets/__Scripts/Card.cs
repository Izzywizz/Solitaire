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
    [Header("Set Dynamically")]
    public string suit; // Suit of the Card (C, D, or S)
    public int rank; // Rank of the card (1-14)
    public Color color = Color.black; // color to tint pips
    public string colS = "Black"; // or "red". Name of the Color

    // This Lists holds all of the Decorator GameObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    // This List holds all of the Pip GameObjects;
    public List<GameObject> pipGOs = new List<GameObject>();

    public GameObject back; // The GameObject of the back of the card

    public CardDefinition def; // Parsed from DecXML.xml

    // Changing the sorting order will actually give the illusion that card has fliped nad remain unseen
    public bool faceUp
    {
        get
        {
            return (!back.activeSelf);
        }
        set
        {
            back.SetActive(!value);
        }
    }

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

