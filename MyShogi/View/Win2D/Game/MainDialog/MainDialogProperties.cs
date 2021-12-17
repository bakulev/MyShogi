using System.Windows.Forms;
using MyShogi.Model.Shogi.LocalServer;

namespace MyShogi.View.Win2D
{
    /// <summary>
    /// Main window with game board etc.
    ///
    /// Only property is collected here.
    /// </summary>
    public partial class MainDialog : Form
    {

        /// <summary>
        /// Returns an instance of LocalGameServer associated with an active GameScreenControl.
        /// Currently, GameScreenControl only creates one instance, so it is active.
        /// </summary>
        public LocalGameServer gameServer { get { return gameScreenControl1.gameServer; } }

        // -- DockWindow

        // Game record Control

        /// <summary>
        /// Returns an instance of KifuControl associated with an active GameScreenControl.
        /// Currently, GameScreenControl only creates one instance, so it is active.
        /// </summary>
        public KifuControl kifuControl { get { return gameScreenControl1.kifuControl; } }

        /// <summary>
        /// For when using the game record window in floating mode.
        /// </summary>
        public DockWindow kifuDockWindow { get; set; }


        // Consider Control

        /// <summary>
        /// This is the main body of the examination window.
        /// This generation is done by MainDialog.
        /// 
        /// Fill this in ↓ and use it.
        /// </summary>
        public EngineConsiderationMainControl engineConsiderationMainControl;

        /// <summary>
        /// A container for filling and using the review window.
        /// For engine thinking output.
        /// </summary>
        public DockWindow engineConsiderationDockWindow;


        // Mini board

        /// <summary>
        /// Control of the mini board surface buried in the examination window.
        /// This instance is also effective when you remove it from the examination window and use it as a Dock. 
        /// </summary>
        public MiniShogiBoard miniShogiBoard { get { return engineConsiderationMainControl.MiniShogiBoard; } }

        /// <summary>
        /// A container for filling and using the mini board.
        /// </summary>
        public DockWindow miniShogiBoardDockWindow;


        // Trend graph

        /// <summary>
        /// Control of the stance graph buried in the review window.
        /// This instance is also effective when you remove it from the examination window and use it as a Dock.
        /// </summary>
        public EvalGraphControl evalGraphControl { get { return engineConsiderationMainControl.EvalGraphControl; } }
        /// <summary>
        /// A container for filling and using the situation graph.
        /// </summary>
        public Info.EvalGraphDialog evalGraphDialog;

        // --Single window

        /// <summary>
        /// Debug window
        /// </summary>
        public Form debugDialog;
    }
}
