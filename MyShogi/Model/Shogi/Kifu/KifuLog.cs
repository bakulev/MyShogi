using System;

namespace MyShogi.Model.Shogi.Kifu
{
    /// <summary>
    /// Property set such as thought log
    /// </summary>
    public class KifuLog
    {
        /// <summary>
        /// Thought log for this move (output by the engine)
        /// Last output PV, evaluation value, etc. by the "go" command for this move
        /// </summary>
        public string engineComment;

        /// <summary>
        /// Time when this move was pointed (if you want to write)
        /// In the root phase, the start date and time of the game is entered.
        /// </summary>
        public DateTime moveTime;
    }
}
