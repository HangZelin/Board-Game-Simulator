﻿using System.Collections;
using UnityEngine;

namespace BGS.Chess_3D
{
    public class Knight_mul : Chessplayer_mul
    {
        public override bool[,] PossibleMoves()
        {
            bool[,] r = new bool[8, 8];

            // Up left
            Move(CurrentX - 1, CurrentY + 2, ref r);

            // Up right
            Move(CurrentX + 1, CurrentY + 2, ref r);

            // Down left
            Move(CurrentX - 1, CurrentY - 2, ref r);

            // Down right
            Move(CurrentX + 1, CurrentY - 2, ref r);


            // Left Down
            Move(CurrentX - 2, CurrentY - 1, ref r);

            // Right Down
            Move(CurrentX + 2, CurrentY - 1, ref r);

            // Left Up
            Move(CurrentX - 2, CurrentY + 1, ref r);

            // Right Up
            Move(CurrentX + 2, CurrentY + 1, ref r);

            return r;
        }

    }
}
