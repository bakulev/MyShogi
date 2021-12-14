using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MyShogi.App;
using MyShogi.Model.Common.String;
using MyShogi.Model.Common.Utility;
using MyShogi.Model.Shogi.Usi;

namespace MyShogi.Model.Shogi.EngineDefine
{
    /// <summary>
    /// EngineDefine utility
    /// </summary>
    public static class EngineDefineUtility
    {
        /// <summary>
        /// "engine_define.xml"Read the file and deserialize it.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static EngineDefine ReadFile(string filepath)
        {
            var def = Serializer.Deserialize<EngineDefine>(filepath);

            // I couldn't read it, so I created a new one.
            if (def == null)
                def = new EngineDefine();

            // Convert the banner file name, executable file name, etc. to full path.

            var current = Path.GetDirectoryName(filepath);
            def.BannerFileName = Path.Combine(current, def.BannerFileName);
            def.EngineExeName = Path.Combine(current, def.EngineExeName);

            // Insert "custom" in the first preset.

            var custom_preset = new EnginePreset("custom",
                "Custom tuning. Follow the contents of \"Detailed settings\".\r\n" +
                "If you are concerned about the CPU load factor, adjust it in \"Number of threads\" in \"Detailed settings\".");
            if (def.Presets == null) def.Presets = new List<EnginePreset>();
            def.Presets.Insert(0, custom_preset);

            return def;
        }

        /// <summary>
        /// Serialize and export EngineDefine to a file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="engine_define"></param>
        public static void WriteFile(string filepath, EngineDefine engine_define)
        {
            Serializer.Serialize(filepath, engine_define);
        }

        /// <summary>
        /// The executable file name that matches the current environment is returned.
        /// 
        /// example) "EngineExeName_avx2.exe"
        /// </summary>
        /// <param name="engine_define"></param>
        /// <returns></returns>
        public static string EngineExeFileName(this EngineDefine engine_define)
        {
            // Establish the current environment.
            var current_cpu = CpuUtil.GetCurrentCpu();

            // Make it the best executable file it supports.
            // If there is no supported one, it will be XXX_unknown.exe and then
            // The file doesn't exist and an exception is thrown, but is this a good thing?
            var cpu = CpuType.UNKNOWN;
            foreach (var c in engine_define.SupportedCpus)
                if (c <= current_cpu /* Works on current CPU */ && cpu < c /* The best guy */)
                    cpu = c;

            return $"{ engine_define.EngineExeName }_{ cpu.ToSuffix()}.exe";
        }

        /// <summary>
        /// Check the engine folder under the executable file and return all the paths to "engine_define.xml".
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEngineDefineFiles()
        {
            var result = new List<string>();

            var current = Path.GetDirectoryName(Application.ExecutablePath);
            var engine_folder = Path.Combine(current, "engine");
            var folders = Directory.GetDirectories(engine_folder);
            foreach(var f in folders)
            {
                // If there is "engine_define.xml" in this folder, add it.
                var path = Path.Combine(f, "engine_define.xml");
                if (File.Exists(path))
                    result.Add(path);
            }
            return result;
        }

        /// <summary>
        /// Check the engine folder under the executable file and read each "engine_define.xml"
        /// EngineDefineのListを返す。
        /// </summary>
        /// <returns></returns>
        public static List<EngineDefineEx> GetEngineDefines()
        {
            // List "engine_define.xml" under the engine / folder under the executable file.
            var def_files = GetEngineDefineFiles();

            // Execution folder of MyShogi. Remove this from filename to get the relative path.
            var current_path = Path.GetDirectoryName(Application.ExecutablePath);

            var list = new List<EngineDefineEx>();
            foreach (var filename in def_files)
            {
                try
                {
                    var engine_define = ReadFile(filename);
                    var relative_path = Path.GetDirectoryName(filename).Substring(current_path.Length);

                    var engine_define_ex = new EngineDefineEx()
                    {
                        EngineDefine = engine_define,
                        FolderPath = relative_path,
                    };
                    list.Add(engine_define_ex);
                } catch (Exception ex)
                {
                    TheApp.app.MessageShow($"{filename}Failed to analyze. \nException name" + ex.Message , MessageShowType.Error);
                }
            }

            // EngineDefine.EngineOrder Sort in the order of.
            list.Sort((x ,y) => y.EngineDefine.DisplayOrder - x.EngineDefine.DisplayOrder);

            return list;
        }

        /// <summary>
        /// このエンジンが、あるExtenedProtocolをサポートしているかを判定する。
        /// </summary>
        /// <param name="engineDefine"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public static bool IsSupported(this EngineDefine engineDefine , ExtendedProtocol protocol)
        {
            return engineDefine.SupportedExtendedProtocol == null ? false :
                engineDefine.SupportedExtendedProtocol.Contains(protocol);
        }

        /// <summary>
        /// UsiEngineに渡すEngineOptionsを生成する。
        /// 
        /// ・エンジン共通設定
        /// ・エンジン個別設定
        /// 
        /// ・エンジン側から送られてきた"option"の列
        /// →　これは送られて来る前に、OptionListを設定しないといけないので使えない？
        /// →　エンジン共通設定にあるものしか設定できないので、エンジンからオプションリストをもらわないと
        /// どうにもならないのでは…。
        /// 
        /// </summary>
        /// <param name="optionList">これが改変される</param>
        /// <param name="engineDefineEx">エンジン定義</param>
        /// <param name="selectedPresetIndex">プリセットの番号</param>
        /// <param name="config">エンジン共通設定、個別設定</param>
        /// <param name="hashSize">hashサイズ[MB] 0を指定するとoption設定に従う。AutoHashの時に呼び出し元のほうで設定する。</param>
        /// <param name="threads">スレッド数。エンジンオプションのThreadsの値は、この値で設定される。</param>
        /// <returns></returns>
        public static void SetDefaultOption(List<UsiOption> optionList, EngineDefineEx engineDefineEx, int selectedPresetIndex ,
            EngineConfig config , long hashSize , int threads , bool ponder)
        {
            var engineDefine = engineDefineEx.EngineDefine;
            var folderPath = engineDefineEx.FolderPath;

            // EnginePreset
            var index = selectedPresetIndex /* - 1 */; // EngineDefineのデシリアライズ時に0番目に「カスタム」を自動挿入している。ゆえに、このまま対応する。
            List<EngineOption> preset = null;
            if (0 <= index && index < engineDefine.Presets.Count)
            {
                preset = engineDefine.Presets[index].Options;
            }

            // 共通設定
            var commonSetting = config.CommonOptions;
            // 個別設定
            var indSetting = config.IndivisualEnginesOptions.Find(x => x.FolderPath == folderPath);

            bool set_hash = false;

            foreach (var option in optionList)
            {
                var value = config.GetOptionValue(option.Name , commonSetting , indSetting , preset);

                // 値を変更したい場合は、この変数valueを上書きする。(最後にSetDefault(value)しているので)

                // Hashサイズの自動マネージメント
                if (option.Name == "USI_Hash" || option.Name == "Hash")
                {
                    // "USI_Hash","Hash"のうち、エンジン側が持っているほうのオプション名に対して設定すれば良い
                    // どちらも持っていない場合は、"USI_Hash"オプションを強制的に生成しなければならない。
                    if (hashSize != 0)
                        value = hashSize.ToString();
                    set_hash = true;
                }
                // Threadsの自動マネージメント
                else if (option.Name == "Threads")
                {
                    value = threads.ToString();
                }
                // Ponder設定の反映。
                else if (option.Name == "USI_Ponder")
                {
                    value = ponder ? "true" : "false";
                }

                if (value != null)
                    option.SetDefault(value);
            }

            if (!set_hash)
            {
                // hashの設定がなかったので"USI_Hash"を強制追加。

                var option = UsiOption.USI_Hash.Clone();
                option.SetDefault(hashSize.ToString());
                optionList.Add(option);
            }

            // スレッド数の自動マネージメントについて..
            // ponderの自動マネージメントについて..

        }

    }
}
