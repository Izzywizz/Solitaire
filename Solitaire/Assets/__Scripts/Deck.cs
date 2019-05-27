using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Deck class interprets the info in DECKXML.xml and uses that info to create an entire deck of cards
/// </summary>
public class Deck : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Suits
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;

    // Prefabs
    public GameObject prefabCard;
    public GameObject prefabSprite;

    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;

    // InitDEck is call by Prospector when it is ready
    public void InitDeck(string deckXMLText)
    {
        ReadDeck(deckXMLText);
    }

    // ReadDeck parses the XML file passsed to it into CardDefinitions
    private void ReadDeck(string deckXMLText)
    {
        xmlr = new PT_XMLReader(); // Create a new instance of PT_XMLReader (our xml helper util class)
        xmlr.Parse(deckXMLText); // Use that PT_XMLReader to parse DeckXML

        // This prints a test to show you how xmlr can be used
        string s = "xml[0] decorator[0]";
        s += "type=" + xmlr.xml["xml"][0]["decorator"][0].att("type");
        s += " x=" + xmlr.xml["xml"][0]["decorator"][0].att("x");
        s += " y=" + xmlr.xml["xml"][0]["decorator"][0].att("y");
        s += " scale=" + xmlr.xml["xml"][0]["decorator"][0].att("scale");

        //print(s);
        // Read decorators for all Cards
        decorators = new List<Decorator>(); // Init the List of Decorators
        // Grab an PT_XMLHashList of all <decorators> in the XML file.
        PT_XMLHashList xDecos = xmlr.xml["xml"][0]["decorator"];
        Decorator deco;
        for (int i = 0; i < xDecos.Count; i++)
        {
            //For each <decorator> in the XML
            deco = new Decorator(); // Make a new DEcorator
            // Copy the attributes of the <decorator> to the Decorator
            deco.type = xDecos[i].att("type");
            // bool deco.flip is true if the text of the flip attribute is "1"
            deco.flip = (xDecos[i].att("flip") == "1");
            // floats need to be parsed from the attribute strings
            deco.scale = float.Parse(xDecos[i].att("scale"));
            // Vector3 loc initializes to [0,0,0] so we just need to modify it
            deco.loc.x = float.Parse(xDecos[i].att("x"));
            deco.loc.y = float.Parse(xDecos[i].att("y"));
            deco.loc.z = float.Parse(xDecos[i].att("z"));

            // Add the temporat deco to the List decorators
            decorators.Add(deco);
        }

        // Read pip locations for each card number
        cardDefs = new List<CardDefinition>(); // Init the List of Cards
        // Grab an PT_XMLHashList of all the <cards> in the XML file
        PT_XMLHashList xCardDefs = xmlr.xml["xml"][0]["card"];
        for (int i = 0; i < xCardDefs.Count; i++)
        {
            // For each of the <cards> Create a new CardDefifinition
            CardDefinition cDef = new CardDefinition();
            // Parse the attribute
            cDef.rank = int.Parse(xCardDefs[i].att("rank"));
            // Grab an PT_XMLHashList of all the <pips> on this <card>
            PT_XMLHashList xPips = xCardDefs[i]["pip"];
            if (xPips != null)
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    // Iterate through all the <pips>
                    deco = new Decorator();
                    deco.type = "pip";
                    deco.flip = (xPips[j].att("flip") == "1");
                    deco.loc.x = float.Parse(xPips[j].att("x"));
                    deco.loc.y = float.Parse(xPips[j].att("y"));
                    deco.loc.z = float.Parse(xPips[j].att("z"));

                    if (xPips[j].HasAtt("scale"))
                    {
                        deco.scale = float.Parse(xPips[j].att("scale"));
                    }
                    cDef.pips.Add(deco);
                }
            }
            // Face cards (Jack, Queen, and King) have face attributes
            if (xCardDefs[i].HasAtt("face"))
            {
                cDef.face = xCardDefs[i].att("face");
            }
            cardDefs.Add(cDef);
            // cDef.face is the base name of the card, so FaceCard_11 is Jacks face Sprile
            // FaceCard_11C is the Jack of clubs and FaceCard_11H is for the Jact of hearts and so on 
        }
    }
}
