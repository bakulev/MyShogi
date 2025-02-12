﻿using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Usi;
using MyShogi.Model.Common.ObjectModel;
using MyShogi.Model.Common.Process;

namespace MyShogi.Model.Shogi.Player
{
    /// <summary>
    /// A thinking engine that interacts with
    /// the USI protocol is implemented as a Player-derived class.
    /// </summary>
    public class UsiEnginePlayer : Player
    {
        // Then call Start () to start.
        public UsiEnginePlayer()
        {
            Initializing = true;
            Engine = new UsiEngine(); // Just generate it. It hasn't started yet.
        }

        public void Start(string exePath)
        {
            Engine.AddPropertyChangedHandler("State", StateChanged);

            var data = new ProcessNegotiatorData(exePath)
            {
                IsLowPriority = true
            };

            Engine.Connect(data);
            // It is assumed that the connection is established.
        }

        public PlayerTypeEnum PlayerType
        {
            get { return PlayerTypeEnum.UsiEngine; }
        }

        /// <summary>
        /// The engine name passed by the engine by the USI protocol
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 対局者名(これが画面上に表示する名前として使われる)
        /// あとでちゃんと書く。
        /// </summary>
        public string DisplayName { get { return Name; } }

        /// <summary>
        /// 通常探索なのか、詰将棋探索なのか。
        /// IsMateSearch == trueなら詰将棋探索
        /// </summary>
        public bool IsMateSearch
        {
            get { return Engine.IsMateSearch; }
            set { Engine.IsMateSearch = value; }
        }

        /// <summary>
        /// このプレイヤーが指した指し手
        /// </summary>
        public Move BestMove
        {
            get { return Engine.BestMove; }
            set { /*実は実装してない*/ }
        }

        /// <summary>
        /// TIME_UPなどが積まれる。BestMoveより優先して解釈される。
        /// </summary>
        public Move SpecialMove { get; set; }

        /// <summary>
        /// このプレイヤーのponderの指し手
        /// </summary>
        public Move PonderMove { get { return Engine.PonderMove; } }

        /// <summary>
        /// 駒を動かして良いフェーズであるか？
        /// </summary>
        public bool CanMove { get; set; }

        /// <summary>
        /// "readyok"が返ってくるまでtrue
        /// </summary>
        public bool Initializing { get; set; }

        /// <summary>
        /// Engine本体
        /// </summary>
        public UsiEngine Engine;

        public void OnIdle()
        {
            // 思考するように命令が来ていれば、エンジンに対して思考を指示する。

            // 受信処理を行う。
            Engine.OnIdle();
        }

        public void Think(Kifu.KifuNode node, string usiPosition, UsiThinkLimit limit , Color sideToMove)
        {
            Engine.Think(node, usiPosition,limit,sideToMove);
        }

        public void Dispose()
        {
            // エンジンを解体する
            Engine.Dispose();
        }


        /// <summary>
        /// いますぐに指させる。
        /// Think()を呼び出してBestMoveはまだ得ていないものとする。
        /// </summary>
        public void MoveNow()
        {
            Engine.MoveNow();
        }

        /// <summary>
        /// "gameover"文字列をエンジン側に送信する。
        /// </summary>
        /// <param name="result"></param>
        public void SendGameOver(MoveGameResult result)
        {
            Engine.SendGameOver(result);
        }

        /// <summary>
        /// SendGameOver()のあと、"isready"を送信して"readyok"を待つ。(連続対局時用)
        /// </summary>
        public void SendIsReady()
        {
            Engine.SendIsReady();
        }

        // -- private member

        /// <summary>
        /// EngineのStateが変化したときに呼び出される。
        /// IsInitなど、必要なフラグを変更するのに用いる。
        /// </summary>
        /// <param name="args"></param>
        private void StateChanged(PropertyChangedEventArgs args)
        {
            var state = (UsiEngineState)args.value;
            switch(state)
            {
                case UsiEngineState.WaitUsiOk:
                case UsiEngineState.WaitReadyOk:
                case UsiEngineState.GameOver:
                    Initializing = true;
                    break;

                case UsiEngineState.UsiOk:
                    // エンジンの設定を取得したいだけの時はこのタイミングで初期化は終わっていると判定すべき。
                    if (Engine.EngineSetting)
                        Initializing = false;
                    break;

                //case UsiEngineState.ReadyOk:
                case UsiEngineState.InTheGame:
                    Initializing = false; // 少なくとも初期化は終わっている。
                    break;
            }

        }
    }
}
