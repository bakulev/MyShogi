using System;
using System.IO;
using MyShogi.Model.Common.ObjectModel;
using MyShogi.Model.Common.Utility;
using MyShogi.Model.Shogi.Kifu;

namespace MyShogi.Model.Shogi.LocalServer
{
    /// <summary>
    /// Settings for saving game results
    ///
    /// Use with GameResultWindowSettingDialog and data bind.
    /// </summary>
    public class GameResultSetting : NotifyObject
    {
        public GameResultSetting()
        {
            // By default, it is automatically saved and enabled.
            AutomaticSaveKifu = true;

            // KIF format by default
            KifuFileType = KifuFileType.KIF;

            // By default, "Documents / YaneuraOuKifu" is set as the save destination.
            var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolderPath2 = "D"; appDataFolderPath2 += appDataFolderPath.Substring(1);
            if (Directory.Exists(appDataFolderPath2))
                appDataFolderPath = appDataFolderPath2;
            KifuSaveFolder = Path.Combine(appDataFolderPath, "YaneuraOuKifu");


            // By default, it creates a subfolder.
            CreateSubfolderOnContinuousGame = true;

            try
            {
                FileIO.CreateDirectory(KifuSaveFolder);
            }
            catch { }
        }

        /// <summary>
        /// Automatically save the game record.
        /// </summary>
        public bool AutomaticSaveKifu
        {
            get { return GetValue<bool>("AutomaticSaveKifu"); }
            set { SetValue("AutomaticSaveKifu", value); }
        }

        /// <summary>
        /// 連続対局のときに自動的にサブフォルダを作成する。
        /// </summary>
        public bool CreateSubfolderOnContinuousGame
        {
            get { return GetValue<bool>("CreateSubfolderOnContinuousGame"); }
            set { SetValue("CreateSubfolderOnContinuousGame", value); }
        }

        /// <summary>
        /// 保存する棋譜のファイル形式
        /// </summary>
        public KifuFileType KifuFileType
        {
            get { return GetValue<KifuFileType>("KifuFileType"); }
            set { SetValue("KifuFileType", value); }
        }

        /// <summary>
        /// 棋譜の自動保存先のフォルダ
        /// </summary>
        public string KifuSaveFolder
        {
            get { return GetValue<string>("KifuSaveFolder"); }
            set { SetValue("KifuSaveFolder", value); }
        }

        /// <summary>
        /// csvファイルの保存pathを返す。
        /// これは棋譜保存フォルダに"game_result.csv"というファイル名で存在するものとする。
        /// </summary>
        /// <returns></returns>
        public string CsvFilePath()
        {
            return Path.Combine(KifuSaveFolder, "game_result.csv");
        }
    }
}
