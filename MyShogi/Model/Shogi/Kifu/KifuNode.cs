using System.Collections.Generic;

namespace MyShogi.Model.Shogi.Kifu
{
    /// <summary>
    /// Expression of game record
    /// Move in a certain situation, procedure of the main score, etc ..
    ///
    /// The procedure of the main score is moves [0].
    /// Swap the move pointed in the actual game with moves [0] so that it will be brought to moves [0].
    ///
    /// In the case of a branch game record, it is guaranteed that the procedure
    /// of the main game record is stored there.
    /// Also, the same applies when exporting, and the procedure
    /// of the main score should be exported first to the game record file.
    /// </summary>
    public class KifuNode
    {
        public KifuNode(KifuNode prevNode_)
        {
            prevNode = prevNode_;
        }

        /// <summary>
        /// Moves in this phase (there are multiple because there are branches)
        /// moves [0] is the procedure of the main score.
        /// Move moves do not allow duplication.
        /// (After the game, if the move is the same even if the game is restarted from the middle of the game, it will be overwritten.) 
        /// </summary>
        public List<KifuMove> moves = new List<KifuMove>();

        /// <summary>
        /// The node just before. This is passed in the constructor.
        /// It is necessary to set this correctly when changing branches.
        /// </summary>
        public KifuNode prevNode;

        /// <summary>
        /// Game record comment on this aspect
        /// Comments on rootNode (starting phase) are also here.
        /// </summary>
        public string comment;

        public List<Usi.UsiThinkReportMessage> thinkmsgs = new List<Usi.UsiThinkReportMessage>();

        public List<Core.EvalValueEx> evalList = new List<Core.EvalValueEx>();

    }
}
