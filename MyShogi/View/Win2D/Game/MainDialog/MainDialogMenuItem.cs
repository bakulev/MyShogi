using System.IO;
using System.Windows.Forms;
using MyShogi.App;
using MyShogi.Model.Common.ObjectModel;
using MyShogi.Model.Common.Tool;
using MyShogi.Model.Common.Utility;
using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Kifu;
using MyShogi.Model.Shogi.LocalServer;
using MyShogi.View.Win2D.Setting;

namespace MyShogi.View.Win2D
{
    /// <summary>
    /// Main window with game board etc.
    ///
    /// Only menu related items are separated into this file.
    /// </summary>
    public partial class MainDialog : Form
    {
        /// <summary>
        /// [UI thread] : Dynamically add menu items.
        /// The menu is different between the commercial version and the freeware version, so you need to add it dynamically here.
        /// </summary>
        public void UpdateMenuItems(PropertyChangedEventArgs args = null)
        {
            // If you do your best, you can speed up, but this method is not called during the game,
            // ToolStrip has not been updated during the CPU x CPU game, and
            // When it's CPU x human, it's an error even if it's a little slow, so it's okay ...

            var config = TheApp.app.Config;
            var shortcut = TheApp.app.KeyShortcut;
            shortcut.InitEvent1(); // Register the handler for the Shortcut key in this delegate.

            // It is necessary to draw the used file name in the Text part of the main window.
            UpdateCaption();

            // -- Add menu.
            {
                // If it is MenuStrip, it does not respond to clicks from the inactive state, so use MenuStripEx.
                var menu = new MenuStripEx();

                // -- LocalGameServer flags.
                // However, null check is required because it may be called with gameServer == null at initialization.

                // LocalGameServer.GameMode may have the value rewritten now, so it may exclude the event.
                // Therefore, it is necessary to use args.value (GameModeEnum) passed as an argument, but it is possible that args.value is another type. (When passing Board Reverse etc.)
                // Therefore, if args.value is GameModeEnum, use this, otherwise it can't be helped, so the one passed last time is used as it is.
                // (LocalGameServer values are not used directly in menus)
                var gameMode =
                    (args != null && args.value != null && args.value is GameModeEnum) ? (GameModeEnum)args.value :
                    gameServer == null ? GameModeEnum.NotInit :
                    lastGameMode;
                lastGameMode = gameMode;

                // Exam mode (normal engine)
                var consideration = gameMode == GameModeEnum.ConsiderationWithEngine;
                // Examination mode (for Tsume Shogi)
                var mate_consideration = gameMode == GameModeEnum.ConsiderationWithMateEngine;
                // During the game
                var inTheGame = gameMode == GameModeEnum.InTheGame;
                // Editing the board
                var inTheBoardEdit = gameMode == GameModeEnum.InTheBoardEdit;
                // Board surface reversal
                var boardReverse = gameServer == null ? false : gameServer.BoardReverse;

                var item_file = new ToolStripMenuItem();
                item_file.Text = "File (& F)";
                menu.Items.Add(item_file);

                // Disable the entire file menu item during a game, etc.
                item_file.Enabled = gameMode == GameModeEnum.ConsiderationWithoutEngine;

                // --Menu under "File"
                {
                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Open the game record (& O)";
                        item.ShortcutKeys = Keys.Control | Keys.O;
                        // Processing shortcut keys in subwindows
                        shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O) { item.PerformClick(); e.Handled = true; } });
                        item.Click += (sender, e) =>
                        {
                            using (var fd = new OpenFileDialog())
                            {
                                // Specify the choices that appear
                                // in File Types If not specified, all files are displayed
                                fd.Filter = string.Join("|", new string[]
                                    {
                                "Game record file|*.kif;*.kifu;*.ki2;*.kif2;*.ki2u;*.kif2u;*.csa;*.psn;*.psn2;*.sfen;*.json;*.jkf;*.txt",
                                "KIF format|*.kif;*.kifu",
                                "KIF2 format|*.ki2;*.kif2;*.ki2u;*.kif2u",
                                "CSA format|*.csa",
                                "PSN format|*.psn",
                                "PSN2 format|*.psn2",
                                "SFEN format|*.sfen",
                                "All files|*.*",
                                    });
                                fd.FilterIndex = 1;
                                fd.Title = "Select the game record file to open";

                                // Display a dialog
                                if (fd.ShowDialog() == DialogResult.OK)
                                    ReadKifuFile(fd.FileName);
                            }
                        };
                        item_file.DropDownItems.Add(item);
                    }

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Overwrite save of game record (& S)";
                        item.ShortcutKeys = Keys.Control | Keys.S;
                        // サHandling of shortcut keys in the window
                        shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S) { item.PerformClick(); e.Handled = true; } });
                        item.Enabled = ViewModel.LastFileName != null; // It is effective only when reading a game record.
                        item.Click += (sender, e) =>
                        {
                            var path = ViewModel.LastFileName;

                            // "Open" or "Save As" Overwrites an unsaved file.
                            // "Preservation of the aspect" is not a game record, so ignore it.
                            // The file format is automatically determined from the extension.
                            gameServer.KifuWriteCommand(path, KifuFileTypeExtensions.StringToKifuFileType(path));

                            //UseKifuFile(path);
                            // You should have opened this file just before overwriting and it should have been recorded in MRUF at that time.
                        };
                        item_file.DropDownItems.Add(item);
                    }

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Save the game record as (& N)";
                        item.ShortcutKeys = Keys.Control | Keys.S | Keys.Shift;
                        shortcut.AddEvent1( e => { if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.S) { item.PerformClick(); e.Handled = true; } });
                        item.Click += (sender, e) =>
                        {
                            using (var fd = new SaveFileDialog())
                            {

                                // Specify the choices that appear in File Types If not specified, all files are displayed
                                fd.Filter = "KIF format(*.KIF)|*.KIF|KIF2 format(*.KI2)|*.KI2|CSA format(*.CSA)|*.CSA"
                                        + "|PSN format(*.PSN)|*.PSN|PSN2 format(*.PSN2)|*.PSN2"
                                        + "|SFEN format(*.SFEN)|*.SFEN|All files(*.*)|*.*";
                                fd.FilterIndex = 1;
                                fd.Title = "Select the file format to save the game record";
                                // By default, first move + second move + YYYYMMDDhhmmss.kif
                                // Kakinoki and kifu for Windows seem to be in this format.
                                var default_filename = $"{gameServer.DefaultKifuFileName()}.KIF";
                                fd.FileName = default_filename;
                                // Since it is escaped with this and the file name that the dialog cannot be used is not returned, the subsequent escape is unnecessary.

                                // Display a dialog
                                if (fd.ShowDialog() == DialogResult.OK)
                                {
                                    var path = fd.FileName;
                                    try
                                    {
                                        KifuFileType kifuType;
                                        switch (fd.FilterIndex)
                                        {
                                            case 1: kifuType = KifuFileType.KIF; break;
                                            case 2: kifuType = KifuFileType.KI2; break;
                                            case 3: kifuType = KifuFileType.CSA; break;
                                            case 4: kifuType = KifuFileType.PSN; break;
                                            case 5: kifuType = KifuFileType.PSN2; break;
                                            case 6: kifuType = KifuFileType.SFEN; break;

                                            // Should be automatically determined from the file name
                                            default:
                                                kifuType = KifuFileTypeExtensions.StringToKifuFileType(path);
                                                if (kifuType == KifuFileType.UNKNOWN)
                                                    kifuType = KifuFileType.KIF; // I don't know, KIF format is fine.
                                                break;
                                        }

                                        gameServer.KifuWriteCommand(path, kifuType);
                                        ViewModel.LastFileName = path; // Record the last saved file.
                                        UseKifuFile(path);
                                    }
                                    catch
                                    {
                                        TheApp.app.MessageShow("File export error", MessageShowType.Error);
                                    }
                                }
                            }
                        };
                        item_file.DropDownItems.Add(item);
                    }

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Preservation of aspects (& I)"; // Since P wants to be used for printing, use "I" in position as a shortcut key.
                        item.Click += (sender, e) =>
                        {
                            using (var fd = new SaveFileDialog())
                            {

                                // Specify the choices that appear in File Types If not specified, all files are displayed
                                fd.Filter = "KIF format(*.KIF)|*.KIF|KIF2 format(*.KI2)|*.KI2|CSA format(*.CSA)|*.CSA"
                                        + "|PSN format(*.PSN)|*.PSN|PSN2 format(*.PSN2)|*.PSN2"
                                        + "|SFEN format(*.SFEN)|*.SFEN|SVG format(*.SVG)|*.SVG|All files(*.*)|*.*";
                                fd.FilterIndex = 1;
                                fd.Title = "Select the file format to save the aspect";

                                // Display a dialog
                                if (fd.ShowDialog() == DialogResult.OK)
                                {
                                    var path = fd.FileName;
                                    try
                                    {
                                        KifuFileType kifuType;
                                        switch (fd.FilterIndex)
                                        {
                                            case 1: kifuType = KifuFileType.KIF; break;
                                            case 2: kifuType = KifuFileType.KI2; break;
                                            case 3: kifuType = KifuFileType.CSA; break;
                                            case 4: kifuType = KifuFileType.PSN; break;
                                            case 5: kifuType = KifuFileType.PSN2; break;
                                            case 6: kifuType = KifuFileType.SFEN; break;
                                            case 7: kifuType = KifuFileType.SVG; break;

                                            // Should be automatically determined from the file name
                                            default:
                                                kifuType = KifuFileTypeExtensions.StringToKifuFileType(path);
                                                if (kifuType == KifuFileType.UNKNOWN)
                                                    kifuType = KifuFileType.KIF; // I don't know, KIF format is fine.
                                                break;
                                        }

                                        gameServer.PositionWriteCommand(path, kifuType);

                                        // Since this file was used, record it in MRUF.
                                        UseKifuFile(path);
                                    }
                                    catch
                                    {
                                        TheApp.app.MessageShow("File export error", MessageShowType.Error);
                                    }
                                }
                            }
                        };
                        item_file.DropDownItems.Add(item);
                    }

                    item_file.DropDownItems.Add(new ToolStripSeparator());

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Copy game record / plaque to clipboard (& C)";

                        var itemk1 = new ToolStripMenuItem();
                        itemk1.Text = "Game record KIF format (& 1)";
                        itemk1.ShortcutKeys = Keys.Control | Keys.C;
                        shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) { item.PerformClick(); e.Handled = true; } });

                        // If you set this shortcut key, you can export it even during a game, but there is no problem with exporting.
                        itemk1.Click += (sender, e) => { gameServer.KifuWriteClipboardCommand(KifuFileType.KIF); };
                        item.DropDownItems.Add(itemk1);

                        var itemk2 = new ToolStripMenuItem();
                        itemk2.Text = "Game record KI2 format(&2)";
                        itemk2.Click += (sender, e) => { gameServer.KifuWriteClipboardCommand(KifuFileType.KI2); };
                        item.DropDownItems.Add(itemk2);

                        var itemk3 = new ToolStripMenuItem();
                        itemk3.Text = "Game record CSA format(&3)";
                        itemk3.Click += (sender, e) => { gameServer.KifuWriteClipboardCommand(KifuFileType.CSA); };
                        item.DropDownItems.Add(itemk3);

                        var itemk4 = new ToolStripMenuItem();
                        itemk4.Text = "Game record SFEN format(&4)";
                        itemk4.Click += (sender, e) => { gameServer.KifuWriteClipboardCommand(KifuFileType.SFEN); };
                        item.DropDownItems.Add(itemk4);

                        var itemk5 = new ToolStripMenuItem();
                        itemk5.Text = "Game record PSN format(&5)";
                        itemk5.Click += (sender, e) => { gameServer.KifuWriteClipboardCommand(KifuFileType.PSN); };
                        item.DropDownItems.Add(itemk5);

                        var itemk6 = new ToolStripMenuItem();
                        itemk6.Text = "Game record PSN2 format(&6)";
                        itemk6.Click += (sender, e) => { gameServer.KifuWriteClipboardCommand(KifuFileType.PSN2); };
                        item.DropDownItems.Add(itemk6);

                        item.DropDownItems.Add(new ToolStripSeparator());

                        var itemp1 = new ToolStripMenuItem();
                        itemp1.Text = "aspect KIF(BOD) format(&A)";
                        itemp1.Click += (sender, e) => { gameServer.PositionWriteClipboardCommand(KifuFileType.KI2); };
                        item.DropDownItems.Add(itemp1);

                        var itemp2 = new ToolStripMenuItem();
                        itemp2.Text = "aspect CSA format(&B)";
                        itemp2.Click += (sender, e) => { gameServer.PositionWriteClipboardCommand(KifuFileType.CSA); };
                        item.DropDownItems.Add(itemp2);

                        var itemp3 = new ToolStripMenuItem();
                        itemp3.Text = "aspect SFEN format(&C)";
                        itemp3.Click += (sender, e) => { gameServer.PositionWriteClipboardCommand(KifuFileType.SFEN); };
                        item.DropDownItems.Add(itemp3);

                        var itemp4 = new ToolStripMenuItem();
                        itemp4.Text = "aspect PSN format(&D)";
                        itemp4.Click += (sender, e) => { gameServer.PositionWriteClipboardCommand(KifuFileType.PSN); };
                        item.DropDownItems.Add(itemp4);

                        var itemp5 = new ToolStripMenuItem();
                        itemp5.Text = "aspect PSN2 format(&E)";
                        itemp5.Click += (sender, e) => { gameServer.PositionWriteClipboardCommand(KifuFileType.PSN2); };
                        item.DropDownItems.Add(itemp5);

                        item_file.DropDownItems.Add(item);
                    }

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Paste game record / plaque from clipboard (& P)";
                        // If you set this shortcut key, you can paste it even during a game,
                        // but by looking at GameMode, it is not processed during a game.
                        item.ShortcutKeys = Keys.Control | Keys.V;
                        shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) { item.PerformClick(); e.Handled = true; } });
                        item.Click += (sender, e) => { CopyFromClipboard(); };
                        item_file.DropDownItems.Add(item);
                    }

                    item_file.DropDownItems.Add(new ToolStripSeparator());

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "End (& X)";
                        item.Click += (sender, e) => { TheApp.app.ApplicationExit(); };
                        item_file.DropDownItems.Add(item);
                    }

                    // MRUF : Recently used files

                    {
                        var mruf = TheApp.app.Config.MRUF;
                        ToolStripMenuItem sub_item = null;
                        for (int i = 0; i < mruf.Files.Count; ++i)
                        {
                            var display_name = mruf.GetDisplayFileName(i);
                            if (display_name == null)
                                break;

                            if (i == 0)
                                item_file.DropDownItems.Add(new ToolStripSeparator());
                            else if (i == 3)
                            {
                                sub_item = new ToolStripMenuItem();
                                sub_item.Text = "File history continuation (& R)";
                                item_file.DropDownItems.Add(sub_item);
                            }

                            {
                                var item = new ToolStripMenuItem();
                                item.Text = display_name;
                                var kifu_file_path = mruf.Files[i];
                                item.Click += (sender, e) => { ReadKifuFile(kifu_file_path); };
                                if (i < 3)
                                    item_file.DropDownItems.Add(item);
                                else
                                    sub_item.DropDownItems.Add(item);
                            }

                            if (i == mruf.Files.Count - 1) // Last element
                            {
                                var item = new ToolStripMenuItem();
                                item.Text = "Clear file history (& T)";
                                item.Click += (sender, e) =>
                                {
                                    if (TheApp.app.MessageShow("Do you want to clear the file history? Click \"OK\" to clear it.", MessageShowType.ConfirmationOkCancel) == DialogResult.OK)
                                    {
                                        mruf.Clear();
                                        UpdateMenuItems();
                                    }
                                };
                                item_file.DropDownItems.Add(item);
                            }
                        }
                    }
                }

                var item_playgame = new ToolStripMenuItem();
                item_playgame.Text = "Game (& P)"; // PlayGame
                item_playgame.Enabled = gameServer != null && !gameServer.InTheGame; // Disable this menu during the game
                menu.Items.Add(item_playgame);

                // -- Menu under "Game"
                {
                    { // -- Normal game
                        var item = new ToolStripMenuItem();
                        item.Text = "Normal game (& N)"; // NormalGame
                        item.ShortcutKeys = Keys.Control | Keys.N; // NewGameのN
                        shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N) { item.PerformClick(); e.Handled = true; } });
                        item.Click += (sender, e) =>
                        {
                            using (var dialog = new GameSettingDialog(this))
                            {
                                FormLocationUtility.CenteringToThisForm(dialog, this);
                                dialog.ShowDialog(this); // Keep it in Modal Dialog.
                            }
                        };

                        item_playgame.DropDownItems.Add(item);
                    }

                    item_playgame.DropDownItems.Add(new ToolStripSeparator());

                    { // -- Review mode

                        var item = new ToolStripMenuItem();
                        item.Text = consideration ? "Exit Review Mode (& C) " : " Review Engine Settings (& C)"; // ConsiderationMode

                        // toolStrip button text changes depending on whether it is in review mode.
                        toolStripButton5.Text = consideration ? "The end" : "Inspection";
                        toolStripButton5.ToolTipText = consideration ? "Exit the review mode." : "Enter review mode.";
                        toolStripButton5.Enabled = !inTheGame;
                        item.Click += (sender, e) =>
                        {
                            if (consideration)
                                ToggleConsideration(); // Exit review mode
                            else
                                ShowConsiderationEngineSettingDialog(); // On the examination engine selection screen
                        };

                        item_playgame.DropDownItems.Add(item);
                    }


                    // "Solution" button: Game record analysis
                    //toolStripButton6.Enabled = !inTheGame;

                    { // -- Review mode

                        var item = new ToolStripMenuItem();
                        item.Text = mate_consideration ? "Exit the jamming review mode (& M)" : "Check engine setting (& M)"; // MateMode

                        // toolStrip button text changes depending on whether it is in review mode.
                        toolStripButton7.Text = mate_consideration ? "The end" : "Stuff";
                        toolStripButton7.ToolTipText = mate_consideration ? "Exits the jamming review mode." : "Enter the jamming review mode.";
                        // "Tsume" button: Tsume shogi button
                        toolStripButton7.Enabled = !inTheGame;
                        item.Click += (sender, e) =>
                        {
                            if (mate_consideration)
                                ToggleMateConsideration();
                            else
                                ShowMateEngineSettingDialog(); // On the selection screen of the engine

                        };

                        item_playgame.DropDownItems.Add(item);
                    }

                    item_playgame.DropDownItems.Add(new ToolStripSeparator());

                    { // -- List of game results

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Game result list (& R)"; // game Result
                        item_.Click += (sender, e) =>
                        {
                            using (var dialog = new GameResultDialog())
                            {
                                dialog.ViewModel.AddPropertyChangedHandler("KifuClicked", (args_) =>
                                {
                                    var filename = (string)args_.value;
                                    // Read this file.
                                    var path = Path.Combine(TheApp.app.Config.GameResultSetting.KifuSaveFolder, filename);
                                    try
                                    {
                                        ReadKifuFile(path);
                                    }
                                    catch
                                    {
                                        TheApp.app.MessageShow("The game record file could not be read.", MessageShowType.Error);
                                    }
                                });

                                FormLocationUtility.CenteringToThisForm(dialog, this);
                                dialog.ShowDialog(this);
                            }
                        };

                        item_playgame.DropDownItems.Add(item_);
                    }


                    { // -- Setting to save game results

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Game result save settings (& S)"; // Alphabetically next to R
                        item_.Click += (sender, e) =>
                        {
                            using (var dialog = new GameResultWindowSettingDialog())
                            {
                                FormLocationUtility.CenteringToThisForm(dialog, this);
                                dialog.ShowDialog(this);
                            }
                        };

                        item_playgame.DropDownItems.Add(item_);
                    }

                }

                // "Configuration"
                var item_settings = new ToolStripMenuItem();
                item_settings.Text = "Settings (& S)"; // Settings
                menu.Items.Add(item_settings);
                {
                    var item = new ToolStripMenuItem();
                    item.Text = "Audio settings (& S)"; // Sound setting
                    item.Enabled = config.CommercialVersion != 0; // Only commercial version can be selected
                    item.Click += (sender, e) =>
                    {
                        using (var dialog = new SoundSettingDialog())
                        {
                            FormLocationUtility.CenteringToThisForm(dialog, this);
                            dialog.ShowDialog(this);
                        }
                    };
                    item_settings.DropDownItems.Add(item);
                }

                {
                    var item = new ToolStripMenuItem();
                    item.Text = "Display settings (& D)"; // Display setting
                    item.Click += (sender, e) =>
                    {
                        using (var dialog = new DisplaySettingDialog())
                        {
                            FormLocationUtility.CenteringToThisForm(dialog, this);
                            dialog.ShowDialog(this);
                        }
                    };
                    item_settings.DropDownItems.Add(item);
                }

                {
                    var item = new ToolStripMenuItem();
                    item.Text = "Operation settings (& O)"; // Operation setting
                    item.Click += (sender, e) =>
                    {
                        using (var dialog = new OperationSettingDialog())
                        {
                            FormLocationUtility.CenteringToThisForm(dialog, this);
                            dialog.ShowDialog(this);
                        }
                    };
                    item_settings.DropDownItems.Add(item);
                }

#if false 
                // I don't use this. It becomes difficult to understand as software for general users.            
                {
                    var item = new ToolStripMenuItem();
                    item.Text = "Engine auxiliary setting (& E)"; // Engine Subsetting
                    item.Click += (sender, e) =>
                    {
                        using (var dialog = new EngineSubSettingDialog())
                        {
                            FormLocationUtility.CenteringToThisForm(dialog, this);
                            dialog.ShowDialog(this);
                        }
                    };
                    item_settings.DropDownItems.Add(item);
                }
#endif

                item_settings.DropDownItems.Add(new ToolStripSeparator());

                // -- Adding an external thinking engine (editing engine_define.xml)
                {
                    var item_edit_engine_define = new ToolStripMenuItem();
                    item_edit_engine_define.Text = "Use of external thinking engine (& U)";
                    item_edit_engine_define.Click += (sender, e) =>
                    {
                        using (var dialog = new EngineDefineEditDialog())
                        {
                            FormLocationUtility.CenteringToThisForm(dialog, this);
                            dialog.ShowDialog(this);
                        }
                    };
                    item_settings.DropDownItems.Add(item_edit_engine_define);
                }


                // -- Initialize settings
                {
                    var item_init = new ToolStripMenuItem();
                    item_init.Text = "Initialize settings (& I)";
                    item_settings.DropDownItems.Add(item_init);

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Initialization of each engine setting (& E)";
                        item.Click += (sender, e) =>
                        {
                            if (TheApp.app.MessageShow("Do you want to initialize all engine settings? Click \"OK\" to initialize it and reflect it at the next startup.", MessageShowType.ConfirmationOkCancel) == DialogResult.OK)
                            {
                                TheApp.app.DeleteEngineOption = true;
                            }
                        };
                        item_init.DropDownItems.Add(item);
                    }

                    {
                        var item = new ToolStripMenuItem();
                        item.Text = "Initialization of each display setting (& D)";
                        item.Click += (sender, e) =>
                        {
                            if (TheApp.app.MessageShow("Do you want to initialize all display settings and audio settings? Click \"OK\" to initialize it and reflect it at the next startup.", MessageShowType.ConfirmationOkCancel) == DialogResult.OK)
                            {
                                TheApp.app.DeleteGlobalOption = true;
                            }
                        };
                        item_init.DropDownItems.Add(item);
                    }
                }


                var item_boardedit = new ToolStripMenuItem();
                item_boardedit.Text = "Board editing (& E)"; // board Edit
                item_boardedit.Enabled = !inTheGame;
                menu.Items.Add(item_boardedit);

                // Addition of board editing
                {
                    {   // -- Start of board editing
                        var item = new ToolStripMenuItem();
                        item.Text = inTheBoardEdit ? "End of board editing (& B)" : "Start of board editing (& B)"; // Board edit
                        item.ShortcutKeys = Keys.Control | Keys.E; // boardEdit
                        shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E) { item.PerformClick(); e.Handled = true; } });
                        item.Click += (sender, e) =>
                        {
                            gameServer.ChangeGameModeCommand(
                                inTheBoardEdit ?
                                    GameModeEnum.ConsiderationWithoutEngine :
                                    GameModeEnum.InTheBoardEdit
                            );
                        };
                        item_boardedit.DropDownItems.Add(item);
                    }

                    {   // -- Change of turn
                        var item = new ToolStripMenuItem();
                        item.Enabled = inTheBoardEdit;
                        item.Text = "Change of turn (& T)"; // Turn change
                        item.Click += (sender, e) =>
                        {
                            var raw_pos = gameServer.Position.CreateRawPosition();
                            raw_pos.sideToMove = raw_pos.sideToMove.Not();
                            var sfen = Position.SfenFromRawPosition(raw_pos);
                            gameScreenControl1.SetSfenCommand(sfen);
                        };
                        item_boardedit.DropDownItems.Add(item);
                    }

                    {   // -- The initial phase of Hirate
                        var item = new ToolStripMenuItem();
                        item.Enabled = inTheBoardEdit;
                        item.Text = "Hirate initial plaque placement (& N)"; // No handicaped
                        item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.NoHandicap.ToSfen()); };
                        item_boardedit.DropDownItems.Add(item);
                    }

                    {   // -- Phase of falling pieces
                        var item_handicap = new ToolStripMenuItem();
                        item_handicap.Enabled = inTheBoardEdit;
                        item_handicap.Text = "Initial phase placement of piece drop (& H)"; // Handicaped
                        item_boardedit.DropDownItems.Add(item_handicap);

                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Incense drop (& 1)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapKyo.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Right scent drop (& 2)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapRightKyo.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Handicap (& 3)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapKaku.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Handicap (& 4)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapHisya.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Flying incense drop (& 5)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapHisyaKyo.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Two pieces dropped (& 6)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap2.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Three pieces dropped (& 7)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap3.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Four pieces dropped (& 8)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap4.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Five pieces dropped (& 9)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap5.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Left five drops (& A)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapLeft5.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Six pieces dropped (& B)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap6.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Eight pieces dropped (& C)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap8.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Ten sheets dropped (& D)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Handicap10.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }
                        {
                            var item = new ToolStripMenuItem();
                            item.Enabled = inTheBoardEdit;
                            item.Text = "Ayumu 3 sheets (& E)";
                            item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.HandicapPawn3.ToSfen()); };
                            item_handicap.DropDownItems.Add(item);
                        }

                    }

                    {   // -- Arrangement for Tsume Shogi (in the piece box)
                        var item = new ToolStripMenuItem();
                        item.Enabled = inTheBoardEdit;
                        item.Text = "Placed for Tsume Shogi (& M)"; // Mate
                        item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Mate1.ToSfen()); };
                        item_boardedit.DropDownItems.Add(item);
                    }

                    {   // -- Phase for Sotama Tsume Shogi
                        var item = new ToolStripMenuItem();
                        item.Enabled = inTheBoardEdit;
                        item.Text = "Placed for Sotama Tsume Shogi (& D)"; // Dual king mate
                        item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Mate2.ToSfen()); };
                        item_boardedit.DropDownItems.Add(item);
                    }

                    {
                        // - With twin balls, everything except the ball is in the piece box

                        var item = new ToolStripMenuItem();
                        item.Enabled = inTheBoardEdit;
                        item.Text = "Place all but the balls in the piece box with twin balls (& U)"; // dUal king
                        item.Click += (sender, e) => { gameScreenControl1.SetSfenCommand(BoardType.Mate3.ToSfen()); };
                        item_boardedit.DropDownItems.Add(item);
                    }
                }

                // -- "Edit game record"

                var kifu_edit = new ToolStripMenuItem();
                kifu_edit.Text = "Game record editing (& K)"; // Kifu edit
                kifu_edit.Enabled = !inTheGame;
                menu.Items.Add(kifu_edit);

                // -- Menu under "Edit game record"
                {
                    var item = new ToolStripMenuItem();
                    item.Text = "Clear branches other than the main score (& C)"; // Clear
                    item.Click += (sender, e) =>
                    {
                        if (TheApp.app.MessageShow("This operation deletes the branches on the current game record other than the main game record.",
                            MessageShowType.WarningOkCancel) == DialogResult.OK)
                        {
                            gameServer.ClearSubKifuTreeCommand();
                        }
                    };
                    kifu_edit.DropDownItems.Add(item);
                }

                // -- 「ウインドウ」

                var item_window = new ToolStripMenuItem();
                item_window.Text = "Window (& W)"; // Window
                menu.Items.Add(item_window);

                // -- 「ウインドウ」配下のメニュー
                {

                    { // -- 棋譜ウィンドウ

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Game record window (& K)"; // Kifu window

                        item_window.DropDownItems.Add(item_);

                        var dock = config.KifuWindowDockManager;

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = dock.Visible ? "Hide (& V) " : " Redisplay (& V)"; // visible // 
                            item.ShortcutKeys = Keys.Control | Keys.K; // KifuWindow
                            shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.K) { item.PerformClick(); e.Handled = true; } });
                            item.Click += (sender, e) => { dock.Visible ^= true; dock.RaisePropertyChanged("DockState", dock.DockState); };
                            item_.DropDownItems.Add(item);
                        }


                        { // フローティングの状態
                            var item = new ToolStripMenuItem();
                            item.Text = "Display position (& F)"; // Floating window mode
                            item_.DropDownItems.Add(item);

                            {

                                var item1 = new ToolStripMenuItem();
                                item1.Text = "Embed in main window (& 0) (Embedded Mode)";
                                item1.Checked = dock.DockState == DockState.InTheMainWindow;
                                item1.Click += (sender, e) => { dock.DockState = DockState.InTheMainWindow; };
                                item.DropDownItems.Add(item1);

                                var item2 = new ToolStripMenuItem();
                                item2.Text = "Float from the main window and always keep the relative position (& 1) (Follow Mode)";
                                item2.Checked = dock.DockState == DockState.FollowToMainWindow;
                                item2.Click += (sender, e) => { dock.DockState = DockState.FollowToMainWindow; };
                                item.DropDownItems.Add(item2);

                                var item3a = new ToolStripMenuItem();
                                item3a.Text = "Float from the main window and place it above the main window (& 2) (Dock Mode)";
                                item3a.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Top;
                                item3a.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Top); };
                                item.DropDownItems.Add(item3a);

                                var item3b = new ToolStripMenuItem();
                                item3b.Text = "Float from the main window and place it on the left side of the main window (& 3) (Dock Mode)";
                                item3b.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Left;
                                item3b.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Left); };
                                item.DropDownItems.Add(item3b);

                                var item3c = new ToolStripMenuItem();
                                item3c.Text = "Float from the main window and place it on the right side of the main window (& 4) (Dock Mode)";
                                item3c.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Right;
                                item3c.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Right); };
                                item.DropDownItems.Add(item3c);

                                var item3d = new ToolStripMenuItem();
                                item3d.Text = "Float from the main window and place it at the bottom of the main window (& 5) (Dock Mode)";
                                item3d.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Bottom;
                                item3d.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Bottom); };
                                item.DropDownItems.Add(item3d);

                                var item4 = new ToolStripMenuItem();
                                item4.Text = "Float from the main window and place it freely (& 6) (Floating Mode)";
                                item4.Checked = dock.DockState == DockState.FloatingMode;
                                item4.Click += (sender, e) => { dock.DockState = DockState.FloatingMode; };
                                item.DropDownItems.Add(item4);
                            }
                        }

                        { // 横幅
                            var item = new ToolStripMenuItem();
                            item.Text = "Width when embedded in the main window (& W)"; // Width
                            item_.DropDownItems.Add(item);

                            {
                                var item1 = new ToolStripMenuItem();
                                item1.Text = "100% (normal) (& 1)"; // None
                                item1.Checked = config.KifuWindowWidthType == 0;
                                item1.Click += (sender, e) => { config.KifuWindowWidthType = 0; };
                                item.DropDownItems.Add(item1);

                                var item2 = new ToolStripMenuItem();
                                item2.Text = "125%(&2)";
                                item2.Checked = config.KifuWindowWidthType == 1;
                                item2.Click += (sender, e) => { config.KifuWindowWidthType = 1; };
                                item.DropDownItems.Add(item2);

                                var item3 = new ToolStripMenuItem();
                                item3.Text = "150%(&3)";
                                item3.Checked = config.KifuWindowWidthType == 2;
                                item3.Click += (sender, e) => { config.KifuWindowWidthType = 2; };
                                item.DropDownItems.Add(item3);

                                var item4 = new ToolStripMenuItem();
                                item4.Text = "175%(&4)";
                                item4.Checked = config.KifuWindowWidthType == 3;
                                item4.Click += (sender, e) => { config.KifuWindowWidthType = 3; };
                                item.DropDownItems.Add(item4);

                                var item5 = new ToolStripMenuItem();
                                item5.Text = "200%(&5)";
                                item5.Checked = config.KifuWindowWidthType == 4;
                                item5.Click += (sender, e) => { config.KifuWindowWidthType = 4; };
                                item.DropDownItems.Add(item5);
                            }
                        }

                    }

                    { // ×ボタンで消していた検討ウィンドウの復活

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Review window (& C)"; // Consideration window
                        item_window.DropDownItems.Add(item_);

                        var dock = config.EngineConsiderationWindowDockManager;

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = dock.Visible ? "Hide (& V) " : " Redisplay (& V)"; // visible // 
                            item.ShortcutKeys = Keys.Control | Keys.R; // EngineConsiderationWindowのR。Eが盤面編集のEditのEで使ってた…。
                            shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.R) { item.PerformClick(); e.Handled = true; } });
                            item.Click += (sender, e) => { dock.Visible ^= true; dock.RaisePropertyChanged("DockState", dock.DockState); };
                            item_.DropDownItems.Add(item);
                        }


                        { // フローティングの状態
                            var item = new ToolStripMenuItem();
                            item.Text = "Display position (& F)"; // Floating window mode
                            item_.DropDownItems.Add(item);

                            {

                                var item1 = new ToolStripMenuItem();
                                item1.Text = "Embed in main window (& 0) (Embedded Mode)";
                                item1.Checked = dock.DockState == DockState.InTheMainWindow;
                                item1.Click += (sender, e) => { dock.DockState = DockState.InTheMainWindow; };
                                item.DropDownItems.Add(item1);

                                var item2 = new ToolStripMenuItem();
                                item2.Text = "Float from the main window and always keep the relative position (& 1) (Follow Mode)";
                                item2.Checked = dock.DockState == DockState.FollowToMainWindow;
                                item2.Click += (sender, e) => { dock.DockState = DockState.FollowToMainWindow; };
                                item.DropDownItems.Add(item2);

                                var item3a = new ToolStripMenuItem();
                                item3a.Text = "Float from the main window and place it above the main window (& 2) (Dock Mode)";
                                item3a.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Top;
                                item3a.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Top); };
                                item.DropDownItems.Add(item3a);

                                var item3b = new ToolStripMenuItem();
                                item3b.Text = "Float from the main window and place it on the left side of the main window (& 3) (Dock Mode)";
                                item3b.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Left;
                                item3b.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Left); };
                                item.DropDownItems.Add(item3b);

                                var item3c = new ToolStripMenuItem();
                                item3c.Text = "Float from the main window and place it on the right side of the main window (& 4) (Dock Mode)";
                                item3c.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Right;
                                item3c.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Right); };
                                item.DropDownItems.Add(item3c);

                                var item3d = new ToolStripMenuItem();
                                item3d.Text = "Float from the main window and place it at the bottom of the main window (& 5) (Dock Mode)";
                                item3d.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Bottom;
                                item3d.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Bottom); };
                                item.DropDownItems.Add(item3d);

                                var item4 = new ToolStripMenuItem();
                                item4.Text = "Float from the main window and place it freely (& 6) (Floating Mode)";
                                item4.Checked = dock.DockState == DockState.FloatingMode;
                                item4.Click += (sender, e) => { dock.DockState = DockState.FloatingMode; };
                                item.DropDownItems.Add(item4);
                            }
                        }

                        { // 縦幅
                            var item = new ToolStripMenuItem();
                            item.Text = "Height when embedded in the main window (& H)"; // Height
                            item_.DropDownItems.Add(item);

                            {
                                var item1 = new ToolStripMenuItem();
                                item1.Text = "100% (normal) (& 1)"; // None
                                item1.Checked = config.ConsiderationWindowHeightType == 0;
                                item1.Click += (sender, e) => { config.ConsiderationWindowHeightType = 0; };
                                item.DropDownItems.Add(item1);

                                var item2 = new ToolStripMenuItem();
                                item2.Text = "125%(&2)";
                                item2.Checked = config.ConsiderationWindowHeightType == 1;
                                item2.Click += (sender, e) => { config.ConsiderationWindowHeightType = 1; };
                                item.DropDownItems.Add(item2);

                                var item3 = new ToolStripMenuItem();
                                item3.Text = "150%(&3)";
                                item3.Checked = config.ConsiderationWindowHeightType == 2;
                                item3.Click += (sender, e) => { config.ConsiderationWindowHeightType = 2; };
                                item.DropDownItems.Add(item3);

                                var item4 = new ToolStripMenuItem();
                                item4.Text = "175%(&4)";
                                item4.Checked = config.ConsiderationWindowHeightType == 3;
                                item4.Click += (sender, e) => { config.ConsiderationWindowHeightType = 3; };
                                item.DropDownItems.Add(item4);

                                var item5 = new ToolStripMenuItem();
                                item5.Text = "200%(&5)";
                                item5.Checked = config.ConsiderationWindowHeightType == 4;
                                item5.Click += (sender, e) => { config.ConsiderationWindowHeightType = 4; };
                                item.DropDownItems.Add(item5);
                            }
                        }

                    }

                    { // ×ボタンで消していた検討ウィンドウの復活

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Mini board (& M)"; // Mini shogi board
                        item_window.DropDownItems.Add(item_);

                        var dock = config.MiniShogiBoardDockManager;

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = dock.Visible ? "Hide (& V) " : " Redisplay (& V)"; // visible // 
                            item.ShortcutKeys = Keys.Control | Keys.M; // Mini shogi boardのM。
                            shortcut.AddEvent1(e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.M) { item.PerformClick(); e.Handled = true; } });
                            item.Click += (sender, e) => { dock.Visible ^= true; dock.RaisePropertyChanged("DockState", dock.DockState); };
                            item_.DropDownItems.Add(item);
                        }


                        { // フローティングの状態
                            var item = new ToolStripMenuItem();
                            item.Text = "Display position (& F)"; // Floating window mode
                            item_.DropDownItems.Add(item);

                            {

                                var item1 = new ToolStripMenuItem();
                                item1.Text = "Embed in review window (& 0) (Embedded Mode)";
                                item1.Checked = dock.DockState == DockState.InTheMainWindow;
                                item1.Click += (sender, e) => { dock.DockState = DockState.InTheMainWindow; };
                                item.DropDownItems.Add(item1);

                                var item2 = new ToolStripMenuItem();
                                item2.Text = "Float from the review window and always keep the relative position (& 1) (Follow Mode)";
                                item2.Checked = dock.DockState == DockState.FollowToMainWindow;
                                item2.Click += (sender, e) => { dock.DockState = DockState.FollowToMainWindow; };
                                item.DropDownItems.Add(item2);

                                var item3a = new ToolStripMenuItem();
                                item3a.Text = "Float from the review window and place it above the main window (& 2) (Dock Mode)";
                                item3a.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Top;
                                item3a.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Top); };
                                item.DropDownItems.Add(item3a);

                                var item3b = new ToolStripMenuItem();
                                item3b.Text = "Float from the review window and place it on the left side of the main window (& 3) (Dock Mode)";
                                item3b.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Left;
                                item3b.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Left); };
                                item.DropDownItems.Add(item3b);

                                var item3c = new ToolStripMenuItem();
                                item3c.Text = "Float from the review window and place it on the right side of the main window (& 4) (Dock Mode)";
                                item3c.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Right;
                                item3c.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Right); };
                                item.DropDownItems.Add(item3c);

                                var item3d = new ToolStripMenuItem();
                                item3d.Text = "Float from the review window and place it at the bottom of the main window (& 5) (Dock Mode)";
                                item3d.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Bottom;
                                item3d.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Bottom); };
                                item.DropDownItems.Add(item3d);

                                var item4 = new ToolStripMenuItem();
                                item4.Text = "Float from the review window and place it freely (& 6) (Floating Mode)";
                                item4.Checked = dock.DockState == DockState.FloatingMode;
                                item4.Click += (sender, e) => { dock.DockState = DockState.FloatingMode; };
                                item.DropDownItems.Add(item4);
                            }
                        }

                    }

                    { // ×ボタンで消していた検討ウィンドウの復活

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Trend Graph (& G)"; // eval graph
                        item_window.DropDownItems.Add(item_);

                        var dock = config.EvalGraphDockManager;

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = dock.Visible ? "Hide (& V) " : " Redisplay (& V)"; // visible // 
                            item.ShortcutKeys = Keys.Control | Keys.G; // graph のG。
                            shortcut.AddEvent1(e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G) { item.PerformClick(); e.Handled = true; } });
                            item.Click += (sender, e) => { dock.Visible ^= true; dock.RaisePropertyChanged("DockState", dock.DockState); };
                            item_.DropDownItems.Add(item);
                        }


                        { // フローティングの状態
                            var item = new ToolStripMenuItem();
                            item.Text = "Display position (& F)"; // Floating window mode
                            item_.DropDownItems.Add(item);

                            {

                                var item1 = new ToolStripMenuItem();
                                item1.Text = "Embed in review window (& 0) (Embedded Mode)";
                                item1.Checked = dock.DockState == DockState.InTheMainWindow;
                                item1.Click += (sender, e) => { dock.DockState = DockState.InTheMainWindow; };
                                item.DropDownItems.Add(item1);

                                var item2 = new ToolStripMenuItem();
                                item2.Text = "Float from the review window and always keep the relative position (& 1) (Follow Mode)";
                                item2.Checked = dock.DockState == DockState.FollowToMainWindow;
                                item2.Click += (sender, e) => { dock.DockState = DockState.FollowToMainWindow; };
                                item.DropDownItems.Add(item2);

                                var item3a = new ToolStripMenuItem();
                                item3a.Text = "Float from the review window and place it above the main window (& 2) (Dock Mode)";
                                item3a.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Top;
                                item3a.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Top); };
                                item.DropDownItems.Add(item3a);

                                var item3b = new ToolStripMenuItem();
                                item3b.Text = "Float from the review window and place it on the left side of the main window (& 3) (Dock Mode)";
                                item3b.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Left;
                                item3b.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Left); };
                                item.DropDownItems.Add(item3b);

                                var item3c = new ToolStripMenuItem();
                                item3c.Text = "Float from the review window and place it on the right side of the main window (& 4) (Dock Mode)";
                                item3c.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Right;
                                item3c.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Right); };
                                item.DropDownItems.Add(item3c);

                                var item3d = new ToolStripMenuItem();
                                item3d.Text = "Float from the review window and place it at the bottom of the main window (& 5) (Dock Mode)";
                                item3d.Checked = dock.DockState == DockState.DockedToMainWindow && dock.DockPosition == DockPosition.Bottom;
                                item3d.Click += (sender, e) => { dock.SetState(DockState.DockedToMainWindow, DockPosition.Bottom); };
                                item.DropDownItems.Add(item3d);

                                var item4 = new ToolStripMenuItem();
                                item4.Text = "Float from the review window and place it freely (& 6) (Floating Mode)";
                                item4.Checked = dock.DockState == DockState.FloatingMode;
                                item4.Click += (sender, e) => { dock.DockState = DockState.FloatingMode; };
                                item.DropDownItems.Add(item4);
                            }
                        }

                    }

                    item_window.DropDownItems.Add(new ToolStripSeparator());

                    {
                        // デバッグウィンドウ

                        var item_ = new ToolStripMenuItem();
                        item_.Text = "Log for debugging (& D)"; // Debug window

                        item_window.DropDownItems.Add(item_);

                        {
                            // デバッグ

                            {
                                // デバッグウィンドウ

                                var item1 = new ToolStripMenuItem();
                                item1.Text = "Show debug window (& D)"; // Debug Window
                                item1.ShortcutKeys = Keys.Control | Keys.D;
                                shortcut.AddEvent1( e => { if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D) { item1.PerformClick(); e.Handled = true; } });
                                item1.Click += (sender, e) =>
                                {
                                    if (debugDialog != null)
                                    {
                                        debugDialog.Dispose();
                                        debugDialog = null;
                                    }

                                    var log = Log.log1;
                                    if (log != null)
                                    {
                                            // セットされているはずなんだけどなぁ…。おかしいなぁ…。
                                            debugDialog = new DebugWindow((MemoryLog)log);
                                    }

                                    if (debugDialog != null)
                                    {
                                        FormLocationUtility.CenteringToThisForm(debugDialog, this);
                                        debugDialog.Show();
                                    }
                                };
                                item_.DropDownItems.Add(item1);
                            }

                            {
                                // ファイルへのロギング

                                var item1 = new ToolStripMenuItem();
                                var enabled = config.FileLoggingEnable;
                                item1.Text = enabled ? "End logging to file (& L) " : " Start logging to file (& L)"; // Logging
                                item1.Checked = enabled;

                                item1.Click += (sender, e) => { config.FileLoggingEnable ^= true; };
                                item_.DropDownItems.Add(item1);
                            }

                            //item_.DropDownItems.Add(new ToolStripSeparator());

                        }

                    }

                }

                // 「ヘルプ」
                {
                    var item_others = new ToolStripMenuItem();
                    item_others.Text = "Help (& H)"; // Help
                    menu.Items.Add(item_others);

                    {
                        var item1 = new ToolStripMenuItem();
                        item1.Text = "Frequently Asked Questions (& F)"; // Faq
                        item1.Click += (sender, e) =>
                        {
                                // MyShogi公式のFAQ
                                var url = "https://github.com/yaneurao/MyShogi/tree/master/MyShogi/docs/faq.md";

                            System.Diagnostics.Process.Start(url);
                        };
                        item_others.DropDownItems.Add(item1);
                    }

                    {
                        var item1 = new ToolStripMenuItem();
                        item1.Text = "Operation explanation (online manual) (& M)"; // Manual
                        item1.Click += (sender, e) =>
                        {
                                // MyShogi公式のonline manual
                                var url = "https://github.com/yaneurao/MyShogi/tree/master/MyShogi/docs/online_manual.md";

                            System.Diagnostics.Process.Start(url);
                        };
                        item_others.DropDownItems.Add(item1);
                    }

                    item_others.DropDownItems.Add(new ToolStripSeparator());

                    {
                        // aboutダイアログ

                        var item1 = new ToolStripMenuItem();
                        item1.Text = "Version information (& V)"; // Version
                        item1.Click += (sender, e) =>
                        {
                            using (var dialog = new AboutYaneuraOu())
                            {
                                FormLocationUtility.CenteringToThisForm(dialog, this);
                                dialog.ShowDialog(this);
                            }
                        };
                        item_others.DropDownItems.Add(item1);
                    }

                    {
                        // システム情報ダイアログ

                        var item1 = new ToolStripMenuItem();
                        item1.Text = "System Information (& S)"; // System Infomation
                        item1.Click += (sender, e) =>
                        {
                            using (var dialog = new SystemInfoDialog())
                            {
                                FormLocationUtility.CenteringToThisForm(dialog, this);
                                dialog.ShowDialog(this);
                            }
                        };
                        item_others.DropDownItems.Add(item1);
                    }

                    item_others.DropDownItems.Add(new ToolStripSeparator());

                    {
                        var item1 = new ToolStripMenuItem();
                        item1.Text = "Check for updates (& U)"; // Update
                        item1.Click += (sender, e) =>
                        {
                                // ・オープンソース版は、MyShogiのプロジェクトのサイト
                                // ・商用版は、マイナビの公式サイトのアップデートの特設ページ
                                // が開くようにしておく。
                                var url = config.CommercialVersion == 0 ?
                                    "https://github.com/yaneurao/MyShogi" :
                                    "https://book.mynavi.jp/ec/products/detail/id=92007"; // 予定地

                                System.Diagnostics.Process.Start(url);
                        };
                        item_others.DropDownItems.Add(item1);
                    }

                }

                // Turn it on and use it only during development.
#if false //DEBUG

                    // Add an item to execute the test code to the menu for debugging.
                    {
                        var item_debug = new ToolStripMenuItem();
                        item_debug.Text = "Debug (& G)"; // debuG

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = "DevTest1.Test1()";
                            item.Click += (sender, e) => { DevTest1.Test1(); };
                            item_debug.DropDownItems.Add(item);
                        }

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = "DevTest1.Test2()";
                            item.Click += (sender, e) => { DevTest1.Test2(); };
                            item_debug.DropDownItems.Add(item);
                        }

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = "DevTest1.Test3()";
                            item.Click += (sender, e) => { DevTest1.Test3(); };
                            item_debug.DropDownItems.Add(item);
                        }

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = "DevTest1.Test4()";
                            item.Click += (sender, e) => { DevTest1.Test4(); };
                            item_debug.DropDownItems.Add(item);
                        }

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = "DevTest1.Test5()";
                            item.Click += (sender, e) =>
                            {
                                // Write some experimental code here.
                            };
                            item_debug.DropDownItems.Add(item);
                        }

                        {
                            var item = new ToolStripMenuItem();
                            item.Text = "DevTest2.Test1()";
                            item.Click += (sender, e) => { DevTest2.Test1(); };
                            item_debug.DropDownItems.Add(item);
                        }

                        menu.Items.Add(item_debug);
                    }
#endif



                // メニューのフォントを設定しなおす。
                FontUtility.ReplaceFont(menu, config.FontManager.MenuStrip);

                // レイアウトロジックを停止する
                // メニューの差し替え時間をなるべく小さくしてちらつきを防止する。
                // (しかしこれでもちらつく。なんぞこれ…)
                using (var slb = new SuspendLayoutBlock(this))
                {
                    // フォームのメインメニューとする
                    MainMenuStrip = menu;

                    Controls.Add(menu);

                    // 前回設定されたメニューを除去する
                    // 古いほうのmenu、removeしないと駄目
                    if (old_menu != null)
                    {
                        Controls.Remove(old_menu);
                        old_menu.Dispose();
                    }

                    old_menu = menu;
                    // 次回このメソッドが呼び出された時にthis.Controls.Remove(old_menu)する必要があるので
                    // 記憶しておかないと駄目。
                }
                // レイアウトロジックを再開する
            }

            // 画面の描画が必要になるときがあるので..
            gameScreenControl1.ForceRedraw();
        }

        /// <summary>
        /// 前回のメニュー項目。
        /// </summary>
        private MenuStripEx old_menu { get; set; } = null;

        /// <summary>
        /// 前回にUpdateMenuItems()が呼び出された時のGameMode。
        /// </summary>
        private GameModeEnum lastGameMode = GameModeEnum.ConsiderationWithoutEngine;
    }
}

