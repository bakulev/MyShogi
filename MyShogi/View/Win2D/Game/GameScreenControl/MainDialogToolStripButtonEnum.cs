namespace MyShogi.View.Win2D
{
    /// <summary>
    /// It is difficult to call it by the constant button number
    /// corresponding to the button attached to ToolStrip
    /// in the main dialog, so give it a name.
    /// </summary>
    public enum MainDialogToolStripButtonEnum
    {
        RESIGN, // End button 
        UNDO_MOVE, // waited 
        MOVE_NOW, // Hurry up and point 
        INTERRUPT, // Suspension 

        REWIND, // ◁ button 
        FORWARD, // ▷ button 
        MAIN_BRANCH, // Main score 
    }
}
