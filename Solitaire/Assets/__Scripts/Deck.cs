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
    public bool startFaceUp = false;
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



    /// InitDeck is call by Prospector when it is ready
    public void InitDeck(string deckXMLText)
    {
        // This creates an anchor for all the Card GameObjects in the hierarchy, cleans up the screen
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        // Initialize the Dictionary of SuitSprites with necessary Sprites
        dictSuits = new Dictionary<string, Sprite>()
        {
            {"C", suitClub },
            {"D", suitDiamond},
            {"H", suitHeart},
            {"S", suitSpade }

        }; // cool way to init the dict from the start

        ReadDeck(deckXMLText);

        MakeCards();
    }

    /// ReadDeck parses the XML file passsed to it into CardDefinitions
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

    /// Get the proper CardDefinitions based on Rank (1-14 is Ace to King)
    public CardDefinition GetCardDefinitionByRank(int rank)
    {
        // Search through all of the CardDefinitions
        foreach (CardDefinition cd in cardDefs)
        {
            // If the rank is correct, return this definition
            if (cd.rank == rank)
            {
                return cd;
            }
        }
        return (null);
    }

    /// <summary>
    /// Make the Card GameObjects
    /// </summary>
    public void MakeCards()
    {
        // cardNames will be the names of cards to build
        // Each suit goes from 1 to 14 (e.g., C1 to C14 for Clubs
        string[] letters = new string[]
        {
            "C", "D", "H", "S"
        }; // cool way to init

        foreach (string str in letters) // This steps through the suits in the Letters array and adds the numbers 1-14 to each suit,
        {
            for (int i = 0; i < 13; i++) // recall that index's are 0 based, so there isn't 0 suit so we add 1 to it
            {
                cardNames.Add(str + (i + 1));
            }
        }

        // Make a List to hold add the cards
        cards = new List<Card>();

        // Iterate through all of the card names that were just made
        for (int i = 0; i < cardNames.Count; i++)
        {
            // Make the card and add it to the cards deck, a private helper method that actually goes and creates the cards and add it to this list
            cards.Add(MakeCards(i));
        }

    }

    private Card MakeCards(int cNum)
    {
        // Create a new Card GameObject
        GameObject cgo = Instantiate(prefabCard) as GameObject;
        // Set the transform.parent of the new card to the anchor, for a a clean hierarchy
        cgo.transform.parent = deckAnchor;
        Card card = cgo.GetComponent<Card>(); // Get the card component attached to the prefabCard

        // This line stacks the cars so that they're all in nice rows
        cgo.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);

        // Assign basic values to the Card
        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString(); // first character of card which gives you the suit, recall C14 , C part
        card.rank = int.Parse(card.name.Substring(1));// recall we created the cards C1, we need the 'Number' part '1'
        if (card.suit == "D" || card.suit == "H") // Hearts & diamonds always red coloured cards
        {
            card.colS = "Red";
            card.color = Color.red;
        }
        // Pull the CardDefintion for this card
        card.def = GetCardDefinitionByRank(card.rank);

        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card);

        return card;
    }

    /// <summary>
    /// This doesn't actually add a back to the cards, rather the back will be Sprite
    /// with a higher sorting order then anything else on the card and will be visable when the
    /// card is face down but invsible when the card is face up
    /// </summary>
    /// <param name="card">Card.</param>
    private void AddBack(Card card)
    {
        // Add Card Back
        // The CArd_Back will be able to cover everything else on the Card
        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        _tSR.sprite = cardBack;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        // This is a higher sorting order than anything else
        _tSR.sortingOrder = 2;
        _tGO.name = "back";
        card.back = _tGO;

        // DEfault to face-up
        card.faceUp = startFaceUp;
    }

    private void AddFace(Card card)
    {
        if (card.def.face == "")
        {
            return; // No need to run if this isn't a face card
        }

        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        // Generate the right name and pass it to GetFace()
        _tSp = GetFace(card.def.face + card.suit);
        _tSR.sprite = _tSp; // Assign this Sprite to _tSR
        _tSR.sortingOrder = 1; // set the sorting order
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }

    /// <summary>
    /// Find the proper face card Sprite
    /// </summary>
    /// <returns>The face.</returns>
    /// <param name="faceS">Face s.</param>
    private Sprite GetFace(string faceS)
    {
        foreach (Sprite _tSP in faceSprites)
        {
            // If this Sprite has the right name..
            if (_tSp.name == faceS)
            {
                // ... Then return the Sprite
                return (_tSp);
            }
        }
        // if nothing can be found return null
        return null;
    }

    /// <summary>
    /// Adds the pips ensuring the correct amount depending on the card.
    /// </summary>
    /// <param name="card">Card Object</param>
    private void AddPips(Card card)
    {
        // For each of the pips in the definition
        foreach (Decorator pip in card.def.pips)
        {
            // Instantiate a Sprite GameObject
            _tGO = Instantiate(prefabSprite) as GameObject;
            // Set the parent to be the card GameObject
            _tGO.transform.SetParent(card.transform);
            // Set the position to that specified in the XML
            _tGO.transform.localPosition = pip.loc;
            // Flip if necessary
            if (pip.flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            // Scale if if necssart (only for the Ace)
            if (pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            // Give this GameObject a name
            _tGO.name = "pip";
            // Get the SpriteRender Component
            _tSR = _tGO.GetComponent<SpriteRenderer>();
            // Set the Sprite to the proper suit
            _tSR.sprite = dictSuits[card.suit];
            // Set sorting Order so the pip is rendered above the Card_Front
            _tSR.sortingOrder = 1;
            // Add this to the Cards list of pips
            card.pipGOs.Add(_tGO);
        }
    }

    // These private variables will be reused several times in helper methods
    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;


    private void AddDecorators(Card card)
    {
        // Add Decorators, those smalls pips on the card that identify what suit it is
        foreach (Decorator deco in decorators)
        {
            if (deco.type == "suit")
            {
                // Instantiate a Spite GameObject
                _tGO = Instantiate(prefabSprite) as GameObject;
                // Get the Sprite Renderer component
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                // Set the Sprite to the proper suit
                _tSR.sprite = dictSuits[card.suit];
            }
            else
            {
                _tGO = Instantiate(prefabSprite) as GameObject;
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                // Get the proper Sprite to show this rank
                _tSp = rankSprites[card.rank];
                // Assign this rank Sprite to match the suit
                _tSR.color = card.color;
            }

            // MAke the deco Sprites render above the card
            _tSR.sortingOrder = 1;
            // Make the decorartor SSprite a child of the card
            _tGO.transform.SetParent(card.transform);
            // Set the localPosition based on the location from DeckXML
            _tGO.transform.localPosition = deco.loc;
            // Flip the decorator if needed
            if (deco.flip)
            {
                //An euler rotation of 180 around the Z-axis will flip it
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            // SEt the sclae to keep decos from being too big
            if (deco.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * deco.scale;
            }
            // Name this GameObject so its easy to see
            _tGO.name = deco.type;
            // Add this deco GameObject to the List card.decoGOs
            card.decoGOs.Add(_tGO);
        }
    }

    /// Shuffle the Cards in Deck.cards using pass-by-reference(ref) as the current 
    /// card desk will be shuffled without copying behaviour or returning a new deck
    /// when all the cards have been removed from oCards and added to the temp tCards
    /// Stop shuffling! Then add it back again
    public static void Shuffle(ref List<Card> oCards)
    {
        // Create a temp list to hold the new shuffle order
        List<Card> tCards = new List<Card>();

        int ndx; // This hold the index of the card to be moved
        tCards = new List<Card>();  // Initialize the temp list
        // repeat as long as there are cards in the original List
        while (oCards.Count > 0)
        {
            // Pick the index of a random card
            ndx = UnityEngine.Random.Range(0, oCards.Count);
            // Add that card to the temp List
            tCards.Add(oCards[ndx]);
            // and remove that card from the orginal List
            oCards.RemoveAt(ndx);
        }
        // Replace the orginal List with the temp List
        oCards = tCards;
        // Because oCard is a reference (ref) parameter, the original argument
        // that was passied in is changed as well, pass-by-reference C++ style &


    }

}
