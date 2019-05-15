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
    public PT_XMLReader xmlr;

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
        print(s);
    }
}
