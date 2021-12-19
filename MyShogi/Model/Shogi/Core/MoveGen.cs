using System;

namespace MyShogi.Model.Shogi.Core
{
    /// <summary>
    /// Move generation
    /// </summary>
    public static class MoveGen
    {
        /// <summary>
        /// Generate a legitimate move.
        /// Use from moves [startIndex]. It is assumed that the return value
        /// is set as endIndex and moves [startIndex] ... moves [endIndex-1] are used.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="moves"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int LegalAll(Position pos , Move[] moves, int startIndex)
        {
            /// Position.IsLegal () plays the illegal move perfectly,
            /// so create a move with NonEvalsion () and eliminate it
            /// with FilterNonLegalMoves ().
            /// It's a little late, but it's okay because Evasion isn't that much ...

            // It shouldn't be a problem if you look up
            // 81 squares honestly, but try to speed up to the minimum.
            var endIndex = startIndex;

            var us = pos.sideToMove;
            var ourPieces = pos.Pieces(us); // Own piece
            Square from , to;
            var enemyField = Bitboard.EnemyField(us); // Enemy

            // The first stage seen from myself
            var rank1_for_us = us == Color.BLACK ? Rank.RANK_1 : Rank.RANK_9;
            var rank2_for_us = us == Color.BLACK ? Rank.RANK_2 : Rank.RANK_8;

            // Where there is no own piece (candidate for destination)
            var movable = ~ pos.Pieces(us);

            while (ourPieces.IsNotZero())
            {
                from = ourPieces.Pop();
                Piece pc = pos.PieceOn(from); // The piece from which it was moved
                Piece pt = pc.PieceType();

                // You can move to the dominant point when you put the piece on the pc,
                // but you can not move to the place where your piece is
                var target = Bitboard.EffectsFrom(pc, from, pos.Pieces()) & movable;
                while (target.IsNotZero())
                {
                    to = target.Pop();

                    // Generate a move to move pc from from to to

                    var r = to.ToRank();

                    // Moving to a box with nowhere to go is an illegal move, so exclude it and generate a move
                    if (!
                        (((pt == Piece.PAWN || pt == Piece.LANCE) && r == rank1_for_us)
                        ||(pt == Piece.KNIGHT && (r == rank1_for_us || r== rank2_for_us)))
                        )

                        moves[endIndex++] = Util.MakeMove(from, to);

                    // Conditions to be formed
                    //   1.Pieces that can be moved
                    //   2.The destination or source is the enemy
                    if ((Piece.PAWN <= pt && pt < Piece.GOLD)
                        && (enemyField & (new Bitboard(from) | new Bitboard(to))).IsNotZero())

                        moves[endIndex++] = Util.MakeMovePromote(from, to);
                }
            }

            // Komauchi move

            var h = pos.Hand(us);
            for (Piece pt = Piece.PAWN; pt < Piece.KING; ++pt)
            {
                // Skip if you don't have that piece
                if (h.Count(pt) == 0)
                    continue;

                for (to = Square.ZERO; to < Square.NB; ++to)
                {
                    // You can only hit a box without a piece
                    if (pos.PieceOn(to) != Piece.NO_PIECE)
                        continue;

                    // I can't hit a piece that has no place to go
                    var r = to.ToRank();
                    if (((pt == Piece.PAWN || pt == Piece.LANCE) && r == rank1_for_us)
                      || (pt == Piece.KNIGHT && (r == rank1_for_us || r == rank2_for_us)))
                        continue;

                    // Only check two steps
                    if (pt == Piece.PAWN
                        && (pos.Pieces(us , Piece.PAWN) & Bitboard.FileBB(to.ToFile())).IsNotZero())
                        continue;

                    moves[endIndex++] = Util.MakeMoveDrop(pt, to );
                }
            }

            // Exclude illegal hands.

            int p = startIndex;
            while (p < endIndex)
            {
                Move m = moves[p];
                if (pos.IsLegal(m))
                    ++p;
                else
                    moves[p] = moves[--endIndex]; // It wasn't an illegal move, so fill in the last move here
            }
            return endIndex;
        }

        /// <summary>
        /// Generate all legal moves in the current phase and output them (for debugging)
        /// </summary>
        /// <param name="pos"></param>
        public static void GenTest(Position pos)
        {
            Move[] moves = new Move[(int)Move.MAX_MOVES];
            int endIndex = MoveGen.LegalAll(pos, moves, 0);

            for (int i = 0; i < endIndex; ++i)
                Console.Write(moves[i].Pretty() + " ");

            Console.WriteLine();
        }
    }
}
