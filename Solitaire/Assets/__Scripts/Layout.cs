using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The slotdef class is not a subclass of MonoBehaviour, so it doesn't need a separate c# file
[System.Serializable] // This makes SlotDefs visable in the Unity Inspector Pane
/// Used to store info read from the XML <slot>'s in a more acceisble way. (Recall Chill the F out ios)
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}

public class Layout : MonoBehaviour
{
    public PT_XMLReader xmlr; // Just like Deck, this has a PT_XMLReader
    public PT_XMLHashtable xml; // This var is for faster xml access
    public Vector2 multiplier; // The offset of the tableau's centre 
    // SlotDef references
    public List<SlotDef> slotDefs; // All the SlotDefs for Row0-Row3
    public SlotDef drawPile;
    public SlotDef discardPile;
    // This holds all of the possible names for the layers set by layerID
    public string[] sortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Discard", "Draw" };


    /// <summary>
    /// This function is called to read in the LayoutXML.xml file by taking in XML-formatted string as input and turn it into slotdefs objects
    /// </summary>
    /// <param name="xmlText">Xml text parsed into from a file.</param>
    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText); // The XML is parsed
        xml = xmlr.xml["xml"][0]; // And xml is set as a shortcut to the XML

        // Read the in the multiplier, wich sets card spacing 
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        // Read in the slots
        SlotDef tSD;
        // slotsX is used as a shortcut to all the <slot>'s
        PT_XMLHashList slotsX = xml["slot"];


        for (int i = 0; i < slotsX.Count; i++)
        {
            tSD = new SlotDef(); // create a new SlotDef instance
            if (slotsX[i].HasAtt("type"))
            {
                // If this <slot> has a type attribute parse it
                tSD.type = slotsX[i].att("type"); 
            }
            else
            {
                //if not, set its type to "slot"; it's a card in the rows
                tSD.type = "slot";
            }
            // Various attributes are parsed into numerical values
            tSD.x = float.Parse(slotsX[i].att("x"));
            tSD.y = float.Parse(slotsX[i].att("y"));
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            // This converts the number of the layerID into a text layerName
            // using array subscript, for example, sortingLayerNames[4] is "Discard" where the [4] - layerID is parsed in via the xml
            // This is also used to make sure the correct card lands on top, recall that the z depth within Unity2D is the same
            // So this determines whats on top, the higher the layerID then that card must be on top and shown to the user
            tSD.layerName = sortingLayerNames[tSD.layerID];


            switch (tSD.type)
            {
                // pull additional attribtues based on the tpye of this <slot>
                case "slot":
                    tSD.faceUp = (slotsX[i].att("faceup") == "1");
                    tSD.id = int.Parse(slotsX[i].att("id"));
                    if (slotsX[i].HasAtt("hiddenby"))
                    {
                        string[] hiding = slotsX[i].att("hiddenby").Split(',');
                        foreach (string s in hiding)
                        {
                            tSD.hiddenBy.Add(int.Parse(s));
                        }
                    }
                    slotDefs.Add(tSD);
                    break;

                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
                    drawPile = tSD;
                    break;

                case "discardpile":
                    discardPile = tSD;
                    break;

                default:
                    break;
            }
        }
    }
}
