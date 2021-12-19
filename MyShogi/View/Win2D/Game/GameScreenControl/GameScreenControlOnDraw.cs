using MyShogi.App;
using MyShogi.Model.Resource.Images;
using MyShogi.Model.Shogi.Core;
using System;
using System.Drawing;
using System.Linq;
using SColor = MyShogi.Model.Shogi.Core.Color; // 将棋のほうのColor
using ShogiCore = MyShogi.Model.Shogi.Core;
using SPRITE = MyShogi.Model.Resource.Images.SpriteManager;

namespace MyShogi.View.Win2D
{
    public partial class GameScreenControl
    {
        /// <summary>
        /// 描画時に呼び出される。
        /// 対局盤面を描画する。
        /// </summary>
        public void OnDraw(Graphics g_)
        {
            // 初期化が終わっていない。この描画は出来ない。
            if (gameServer == null)
                return;

            graphics = g_;
            /// 以降、このクラスのDrawSprite(),DrawString()は正常にaffine変換されて描画されるものとする。

            // ここでdirtyをfalseにしておく。
            // 描画中に他のスレッドからの依頼によりDirty=trueになったら、再度描画がなされなくてはならないため。
            dirty = false;

            // ここではDrawSprite()とDrawString()だけで描画を完結させてあるので複数Viewへの対応は(描画だけなら)容易。

            var app = TheApp.app;
            var config = app.Config;

            // 描画する局面
            // これに基づいて描画しないと駄目(途中でgameServer.Positionが差し替わる必要があるので、参照を掴んでおく必要がある)
            var pos = gameServer.Position; // MainDialogViewModel。掴んでいる駒などのViewの状態
            if (pos == null)
                return; // 初期化まだ終わってない。

            var state = viewState;

            var picked_from = state.picked_from;

            // 持ち上げている駒のスプライトと座標(最後に描画するために積んでおく)

            SpriteEx picked_sprite = null;

            // 盤面を反転させて描画するかどうか
            // このメソッドが呼び出されている最中にこの値が変化しうるので、このメソッド内では、ここで取得したものを一貫して使う必要がある。
            var reverse = gameServer.BoardReverse;

            // -- Drawing on the board
            {
                // Editing the board → It is necessary to draw the piece box.
                var inTheBoardEdit = gameServer.InTheBoardEdit;

                // If the coordinate system is specified as straight,
                // it will be affine-transformed and drawn appropriately.
                DrawSprite(new Point(0, 0), SPRITE.Board(PieceTableVersion, inTheBoardEdit));
            }

            // -- Drawing of pieces
            {
                // -- Drawing when the piece is lifted

                var DrawPickedSprite = (Action<Sprite, Point>)((sprite, dest) =>
                 {
                    // Draw the effect applied to the source box.
                    //DrawSprite(dest, SPRITE.PieceMove(PieceMoveEffect.PickedFrom));

                    switch (config.PickedMoveDisplayStyle)
                     {
                         case 0:
                            // Draw with a slight lift.
                            picked_sprite = new SpriteEx(sprite, dest + new Size(-5, -20));
                             break;

                         case 1:
                            // Since it is a mode to follow the mouse cursor,
                            // it is at the position of the mouse cursor ..
                            picked_sprite = new SpriteEx(sprite, MouseClientLocation
                                 + new Size(-piece_img_size.Width / 2, -piece_img_size.Height / 2));
                             break;
                     }
                 });

                // -- Pieces on the board

                // Last move (Note that it does not exist on the initial board,
                // etc., lastMove == Move.NONE)
                var lastMove = pos.State().lastMove;
                // The box from which the last move was made
                var lastMoveFrom = (lastMove != ShogiCore.Move.NONE && !lastMove.IsDrop()) ? lastMove.From() : Square.NB;
                // The destination box of the last move
                var lastMoveTo = (lastMove != ShogiCore.Move.NONE) ? lastMove.To() : Square.NB;

                for (Square sq = Square.ZERO; sq < Square.NB; ++sq)
                {
                    var pc = pos.PieceOn(sq);
                    var dest = PieceLocation((SquareHand)sq, reverse);

                    // When the dialog is displayed or when you are holding a piece,
                    // it is confusing that there is an effect of the final hand, so turn it off.
                    if (state.state == GameScreenControlViewStateEnum.Normal)
                    {
                        // If this is the final move source box, draw the effect.
                        if (sq == lastMoveFrom)
                        {
                            var piece_to = pos.PieceOn(lastMoveTo);
                            DrawSprite(dest, SPRITE.PieceMove(PieceMoveEffect.From, piece_to));
                        }

                        // If this is the destination box for the final move, draw the effect.
                        if (sq == lastMoveTo)
                            DrawSprite(dest, SPRITE.PieceMove(PieceMoveEffect.To));
                    }

                    // In the board inversion mode, the pieces are swapped first and second to draw.
                    var sprite = SPRITE.Piece(reverse ? pc.Inverse() : pc);

                    // If it is the piece you are lifting now,
                    // draw it as if you were lifting it a little.
                    if (picked_from != SquareHand.NB)
                    {
                        // However, I want to draw it in the foreground, so I draw this piece last.
                        // (So skip this drawing)
                        if (sq == (Square)picked_from)
                        {
                            DrawPickedSprite(sprite, dest);
                            continue; // Since it is lifted, skip the normal piece drawing.
                        }
                        else
                        {
                            // The effect of the candidate box to move to when lifting the piece

                            // Is it a candidate box for the destination?
                            var movable = viewState.picked_piece_legalmovesto.IsSet(sq);

                            if (movable && config.PickedMoveToColorType >= 4)
                            // Brighten the box of the candidate to move to
                            {
                                var picked_pc = pos.PieceOn(picked_from);
                                DrawSprite(dest, SPRITE.PieceMove(PieceMoveEffect.PickedTo, picked_pc));
                            }
                            else if (!movable && config.PickedMoveToColorType < 4)
                                // Darken the boxes other than the candidates for the destination
                                DrawSprite(dest, SPRITE.PieceMove(PieceMoveEffect.PickedTo));

                        }
                    }

                    // Normal drawing of pieces
                    DrawSprite(dest, sprite);
                }


                // Generate legal moves and only include them.
                // I feel that this generation, when the situation changes, it is enough to do it once ..
                // You shouldn't click it many times, so no.
                Bitboard moves1_bb = Bitboard.ZeroBB();
                Bitboard moves1_bbd = Bitboard.ZeroBB();
                Bitboard moves2_bb = Bitboard.ZeroBB();
                Bitboard moves2_bbd = Bitboard.ZeroBB();
                Move[] pissible_moves1 = new Move[(int)ShogiCore.Move.MAX_MOVES];
                Move[] pissible_moves2 = new Move[(int)ShogiCore.Move.MAX_MOVES];
                var possible_pos = pos.Clone();
                int n1 = MoveGen.LegalAll(possible_pos, pissible_moves1, 0);
                if (pos.sideToMove == SColor.BLACK)
                    possible_pos.sideToMove = SColor.WHITE;
                else if (pos.sideToMove == SColor.WHITE)
                    possible_pos.sideToMove = SColor.BLACK;
                int n2 = MoveGen.LegalAll(possible_pos, pissible_moves2, 0);
                // The move destination square that matches
                // the move source square for all the generated legal moves
                // is reflected in the Bitboard.
                for (int i = 0; i < n1; ++i)
                {
                    var m = pissible_moves1[i];
                    // Where the piece can move
                    if (!m.IsDrop()) moves1_bb |= m.To();
                    else moves1_bbd |= m.To();
                }
                for (int i = 0; i < n2; ++i)
                {
                    var m = pissible_moves2[i];
                    // Where the piece can move
                    if (!m.IsDrop()) moves2_bb |= m.To();
                    else moves2_bbd |= m.To();
                }

                int paddingX = 5;
                int paddingY = 2;

                var my_move_rect = new Rectangle(
                    ((int)PieceMoveEffect.PickedFrom % 8) * SPRITE.piece_img_width,
                    ((int)PieceMoveEffect.PickedFrom / 8) * SPRITE.piece_img_height,
                    SPRITE.piece_img_width, SPRITE.piece_img_height);
                var my_move_sprite = new Sprite(TheApp.app.ImageManager.PieceMoveImage.image, my_move_rect);
                if (my_move_sprite != null)
                {
                    var moves_image = my_move_sprite.image;
                    var moves1_rect = new Rectangle(
                        my_move_sprite.rect.X + paddingX, my_move_sprite.rect.Y + paddingY,
                        my_move_sprite.rect.Width - paddingX * 2, my_move_sprite.rect.Height / 8);
                    var moves2_rect = new Rectangle(
                        my_move_sprite.rect.X + paddingX, my_move_sprite.rect.Y + paddingY,
                        my_move_sprite.rect.Width - paddingX * 2, my_move_sprite.rect.Height / 8);
                    var moves1_sprite = new Sprite(moves_image, moves1_rect);
                    var moves2_sprite = new Sprite(moves_image, moves2_rect);
                    int k = 0;
                    for (Square sq = Square.ZERO; k++ < 300 && sq < Square.NB; ++sq)
                    {
                        var pc = pos.PieceOn(sq);
                        var dest = PieceLocation((SquareHand)sq, reverse);
                        // Is it a candidate box for the destination?

                        // My moves
                        if ((pos.sideToMove == SColor.BLACK && reverse) || (pos.sideToMove == SColor.WHITE && !reverse))
                        {
                            // Show places that could be beat and defended places.
                            if (moves1_bb.IsSet(sq) || pos.EffectedTo(reverse ? SColor.BLACK : SColor.WHITE, sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + paddingY), moves2_sprite);
                            // Places where peace could be dropped.
                            if (moves2_bbd.IsSet(sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + moves1_sprite.rect.Height + paddingY), moves2_sprite);
                        }
                        if ((pos.sideToMove == SColor.WHITE && reverse) || (pos.sideToMove == SColor.BLACK && !reverse))
                        {
                            // Show places that could be beat and defended places.
                            if (moves1_bb.IsSet(sq) || pos.EffectedTo(reverse ? SColor.WHITE : SColor.BLACK, sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + my_move_sprite.rect.Height - moves1_sprite.rect.Height - paddingY), moves2_sprite);
                            // Places where peace could be dropped.
                            if (moves2_bbd.IsSet(sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + my_move_sprite.rect.Height - moves1_sprite.rect.Height - moves1_sprite.rect.Height - paddingY), moves2_sprite);
                        }
                    }
                }

                var enemy_move_rect = new Rectangle(
                    ((int)PieceMoveEffect.From % 8) * SPRITE.piece_img_width,
                    ((int)PieceMoveEffect.From / 8) * SPRITE.piece_img_height,
                    SPRITE.piece_img_width, SPRITE.piece_img_height);
                var enemy_move_sprite = new Sprite(TheApp.app.ImageManager.PieceMoveImage.image, enemy_move_rect);
                if (enemy_move_sprite != null)
                {
                    var moves_image = enemy_move_sprite.image;
                    var moves1_rect = new Rectangle(
                        enemy_move_sprite.rect.X + paddingX, enemy_move_sprite.rect.Y + paddingY,
                        enemy_move_sprite.rect.Width - paddingX * 2, enemy_move_sprite.rect.Height / 8);
                    var moves2_rect = new Rectangle(
                        enemy_move_sprite.rect.X + paddingX, enemy_move_sprite.rect.Y + paddingY,
                        enemy_move_sprite.rect.Width - paddingX * 2, enemy_move_sprite.rect.Height / 8);
                    var moves1_sprite = new Sprite(moves_image, moves1_rect);
                    var moves2_sprite = new Sprite(moves_image, moves2_rect);
                    int k = 0;
                    for (Square sq = Square.ZERO; k++ < 300 && sq < Square.NB; ++sq)
                    {
                        var pc = pos.PieceOn(sq);
                        var dest = PieceLocation((SquareHand)sq, reverse);
                        
                        // Enemy moves
                        if ((pos.sideToMove == SColor.BLACK && !reverse) || (pos.sideToMove == SColor.WHITE && reverse))
                        {
                            // Show places that could be beat and defended places.
                            if (moves2_bb.IsSet(sq) || pos.EffectedTo(reverse ? SColor.BLACK : SColor.WHITE, sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + paddingY), moves2_sprite);
                            // Places where peace could be dropped.
                            if (moves2_bbd.IsSet(sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + moves1_sprite.rect.Height + paddingY), moves2_sprite);
                        }
                        if ((pos.sideToMove == SColor.WHITE && !reverse) || (pos.sideToMove == SColor.BLACK && reverse))
                        {
                            // Show places that could be beat and defended places.
                            if (moves2_bb.IsSet(sq) || pos.EffectedTo(reverse ? SColor.WHITE : SColor.BLACK, sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + enemy_move_sprite.rect.Height - moves1_sprite.rect.Height - paddingY), moves2_sprite);
                            // Places where peace could be dropped.
                            if (moves2_bbd.IsSet(sq))
                                DrawSprite(new Point(dest.X + paddingX, dest.Y + enemy_move_sprite.rect.Height - moves1_sprite.rect.Height - moves1_sprite.rect.Height - paddingY), moves2_sprite);
                        }
                    }
                }

                // -- Drawing of hand pieces

                foreach (var c in All.Colors())
                {
                    Hand h = pos.Hand(c);

                    // 枚数によって位置が自動調整されるの、わりと見づらいので嫌。
                    // 駒種によって位置固定で良いと思う。

                    //同種の駒が3枚以上になったときに、その駒は1枚だけを表示して、
                    //数字を右肩表示しようと考えていたのですが、例えば、金が2枚、
                    //歩が3枚あるときに、歩だけが数字表示になるのはどうもおかしい気が
                    //するのです。2枚以上は全部数字表示のほうが良いだろう。

                    foreach (var pc in hand_piece_list)
                    {
                        int count = h.Count(pc);
                        if (count != 0)
                        {
                            // この駒の描画されるべき位置を求めるためにSquareHand型に変換する。
                            var piece = Util.ToHandPiece(c, pc);
                            var dest = PieceLocation(piece , reverse);

                            // 物理画面で後手側の駒台への描画であるか(駒を180度回転さて描画しないといけない)
                            var is_white_in_display = (c == SColor.WHITE) ^ reverse;

                            var sprite = SPRITE.Piece(is_white_in_display ? pc.Inverse() : pc);

                            // この駒を掴んでいるならすごしずれたところに描画する。
                            // ただし、掴んでいるので描画を一番最後に回す
                            if (picked_from == piece)
                                // 掴んでいる表現
                                DrawPickedSprite(sprite, dest);
                            else
                                // 駒の通常の描画
                                DrawSprite(dest, sprite);

                            // 駒の枚数を表す数字の描画(枚数が2枚以上のとき)
                            if (count >= 2)
                                DrawSprite(dest + hand_number_offset, SPRITE.HandNumber(count));
                        }
                    }

                }

                // -- 駒箱の駒の描画(盤面編集時のみ)
                if (gameServer.InTheBoardEdit)
                {

                    // 通常の駒台 0 , 細長い駒台なら1
                    var v = PieceTableVersion;
                    // 表示倍率
                    var ratio = v == 0 ? 1.0f : 0.6f;

                    // 駒箱の駒
                    var h = pos.Hand(SColor.NB);

                    foreach (var pt in piece_box_list)
                    {
                        int count = pos.PieceBoxCount(pt);
                        if (count > 0)
                        {
                            var dest = PieceLocation(Util.ToPieceBoxPiece(pt) , reverse);
                            var sprite = SPRITE.Piece(pt);

                            var piece = Util.ToPieceBoxPiece(pt);

                            // この駒、選択されているならば
                            if (picked_from == piece)
                            {
                                // 移動元の升に適用されるエフェクトを描画する。
                                DrawSprite(dest, SPRITE.PieceMove(PieceMoveEffect.PickedFrom), ratio);

                                // マウスカーソルに追随させる(or 少し持ち上げた風にする)
                                // これ配置した時に先手の駒になるが、盤面反転させているなら、後手の駒になって欲しい気はする。
                                // →　Dropの処理の時になんとかする。
                                DrawPickedSprite(sprite, dest);
                            }
                            else
                            {
                                // 駒の描画
                                DrawSprite(dest, sprite, ratio);
                            }

                            // 数字の描画(枚数が2枚以上のとき)
                            if (count >= 2)
                                DrawSprite(dest + hand_number_offset2[v], SPRITE.HandBoxNumber(count), ratio);
                        }
                    }
                }
            }

            // -- 盤の段・筋を表す数字の表示
            {
                var version = config.BoardNumberImageVersion;
                DrawSprite(board_number_pos[0], SPRITE.BoardNumberFile(reverse));
                DrawSprite(board_number_pos[1], SPRITE.BoardNumberRank(reverse));
            }

            // 手番側が先手なら0、後手なら1。ただし、盤面反転しているなら、その逆。
            int side = pos.sideToMove == SColor.BLACK ? 0 : 1;
            side = reverse ? (side ^ 1) : side;

            if (Setting.NamePlateVisible)
            {

                // -- 対局者氏名
                {
                    switch (PieceTableVersion)
                    {
                        // 通常状態の駒台表示
                        case 0:
                            // 手番側を赤文字で表現するなら、その処理
                            var playerColors = new[] { Brushes.Black, Brushes.Black };
                            if (config.TurnDisplay == 2)
                                playerColors[side] = Brushes.IndianRed;

                            DrawString(name_plate_name[0], gameServer.ShortDisplayNameWithTurn(reverse ? SColor.WHITE : SColor.BLACK), 26 , new DrawStringOption(playerColors[0],0));
                            DrawString(name_plate_name[1], gameServer.ShortDisplayNameWithTurn(reverse ? SColor.BLACK : SColor.WHITE), 26 , new DrawStringOption(playerColors[1],0));
                            break;

                        // 細長い状態の駒台表示
                        case 1:
                            DrawSprite(turn_slim_pos, SPRITE.NamePlateSlim(pos.sideToMove, reverse));
                            DrawString(name_plate_slim_name[0], gameServer.ShortDisplayNameWithTurn(reverse ? SColor.WHITE : SColor.BLACK), 26, new DrawStringOption(Brushes.White, 2));
                            DrawString(name_plate_slim_name[1], gameServer.ShortDisplayNameWithTurn(reverse ? SColor.BLACK : SColor.WHITE), 26, new DrawStringOption(Brushes.White, 0));
                            break;
                    }
                }

                // -- 持ち時間等
                {
                    switch (PieceTableVersion)
                    {
                        // 通常状態の駒台表示
                        case 0:
                            foreach(var c in All.IntColors())
                            {
                                var isWhite = c == 1;

                                // 対局時間設定
                                var timeSettingString = gameServer.TimeSettingString(reverse ^ isWhite ? SColor.WHITE : SColor.BLACK);
                                // 2行に渡っているなら残り時間の表示位置を調整してやる。
                                var offset = (timeSettingString == null || !timeSettingString.Contains("\r")) ? Size.Empty : new Size(0, 10);
                                DrawString(time_setting_pos[c], timeSettingString, 18);

                                // 残り時間
                                DrawString(time_setting_pos2[c] + offset , gameServer.RestTimeString(reverse ^ isWhite ? SColor.WHITE : SColor.BLACK), 28, new DrawStringOption(Brushes.Black, 1));
                            }
                            break;

                        // 細長い状態の駒台表示
                        case 1:
                            // 対局時間設定(表示する場所がなさげ)
                            //DrawString(time_setting_slim_pos[0], gameServer.TimeSettingString(reverse ? SColor.WHITE : SColor.BLACK), 18 , new DrawStringOption(Brushes.Black, 2));
                            //DrawString(time_setting_slim_pos[1], gameServer.TimeSettingString(reverse ? SColor.BLACK : SColor.WHITE), 18 , new DrawStringOption(Brushes.Black, 0));

                            // 残り時間
                            DrawString(time_setting_slim_pos2[0], gameServer.RestTimeString(reverse ? SColor.WHITE : SColor.BLACK), 24, new DrawStringOption(Brushes.Black, 1));
                            DrawString(time_setting_slim_pos2[1], gameServer.RestTimeString(reverse ? SColor.BLACK : SColor.WHITE), 24, new DrawStringOption(Brushes.Black, 1));
                            break;
                    }
                }

                // -- 手番の表示
                {
                    switch (PieceTableVersion)
                    {
                        case 0:
                            if (config.TurnDisplay == 1)
                                DrawSprite(turn_normal_pos[side], SPRITE.TurnNormal());
                            break;
                        case 1: DrawSprite(turn_slim_pos, SPRITE.TurnSlim(pos.sideToMove, reverse)); break;
                    }
                }
            }

            // -- 持ち上げている駒があるなら、一番最後に描画する。
            {
                if (picked_sprite != null)
                    DrawSprite(picked_sprite);
            }

            // -- 成り、不成の選択ダイアログを出している最中であるなら
            if (state.state == GameScreenControlViewStateEnum.PromoteDialog)
            {
                // 相手側の駒のダイアログなら180度反転させる
                bool flip;
                var dest = CalcPromoteDialogLocation(state,reverse,out flip);
                DrawSprite(dest, SPRITE.PromoteDialog(state.promote_dialog_selection, state.moved_piece_type) , (flip ? -1.0f : 1.0f) );
            }

            // -- エンジン初期化中のダイアログ

            if (gameServer.EngineInitializing)
                DrawSprite(engine_init_pos, SPRITE.EngineInit());

            // -- animator (この上位に位置する)の描画

            animatorManager.OnDraw();

            // -- 連続対局中のメッセージ
            {
                var cont = gameServer==null ? null : gameServer.continuousGame.GetGamePlayingString();
                if (cont != null)
                {
                    // 赤文字でセンタリングして表示
                    //DrawString(continuos_game_pos, cont, 22, new DrawStringOption(Brushes.Red, Brushes.Black /* 影つき */ , 1));

                    // →　人によっては、これは、やや見づらいらしい。

                    // 青文字でセンタリングして表示
                    DrawString(continuos_game_pos, cont, 22, new DrawStringOption(Brushes.Blue, Brushes.White /* 影つき */ , 1));
                }
            }

            // リソースリークを調べる(デバッグ時)
#if false
            GdiResourceWatcher.DisplayMemory();
#endif
        }
    }
}
