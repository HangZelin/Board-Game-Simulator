using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNO
{

    public enum CardType { num, draw2, draw4, reverse, skip, wild }
    public enum CardColor { red, yellow, blue, green, black };

    public struct CardInfo
    {
        public CardType cardType;
        public CardColor cardColor;
        public int num;
    }
}

