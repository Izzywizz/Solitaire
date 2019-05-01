﻿using System.Collections;
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
}
