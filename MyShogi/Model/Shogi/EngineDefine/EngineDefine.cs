using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyShogi.Model.Shogi.EngineDefine
{
    /// <summary>
    /// Engine configuration file specified by USI 2.0.
    /// Place this serialized in xml format in the folder of the thought engine executable file.
    /// 
    /// On the GUI side, this file is read at startup and used.
    /// </summary>
    [DataContract]
    public class EngineDefine
    {
        /// <summary>
        /// Engine description in one line
        /// </summary>
        [DataMember]
        public string DescriptionSimple { get; set; } = "Engine description";

        /// <summary>
        /// About 5 lines of engine explanation.
        /// Displayed when the engine is selected.
        /// </summary>
        [DataMember]
        public string Description { get; set; } = "Engine description";

        /// <summary>
        /// Engine banner: 512px horizontal x 160px vertical png format recommended.
        /// Relative to the folder where this file is located
        /// </summary>
        [DataMember]
        public string BannerFileName { get; set; } = "banner.png";

        /// <summary>
        /// Engine display name
        /// This name will be displayed on the screen.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; } = "Thinking engine";

        /// <summary>
        /// The executable file name of the engine.
        /// 
        /// For example, if this value is set to "engine", when using EngineExeFileName (), it will be "engine_avx2.exe" for AVX2.
        /// 
        /// example)
        ///     "engine_nosse.exe"  : 32bit Edition
        ///     "engine_sse2.exe"   : 64bit Version sse2 compatible
        ///     "engine_sse41.exe"  : 64bit Version sse4.1 compatible
        ///     "engine_sse42.exe"  : 64bit Version sse4.2 compatible
        ///     "engine_avx2.exe"   : 64bit Version avx2 compatible
        ///     "engine_avx512.exe" : 64bit Version avx512 compatible
        /// </summary>
        [DataMember]
        public string EngineExeName { get; set; } = "engine";

        /// <summary>
        /// List the CPUs supported by the engine.
        /// 
        /// For example, the thinking engine supports SSE2 but not SSE4.1,
        /// If the operating environment is SSE4.1, you can see that you should call the SSE2 executable file.
        /// 
        /// * EngineUtility.EngineExeFileName () performs such processing.
        /// </summary>
        [DataMember]
        public List<CpuType> SupportedCpus { get; set; } = new List<CpuType>(new []
        { CpuType.NO_SSE, CpuType.SSE2 , CpuType.SSE41 , CpuType.SSE42 , CpuType.AVX2 });

        /// <summary>
        /// Memory used Memory used in search (excluding HASH) Unit is [MB]
        ///
        /// Since EvalHash is about 130MB, it is for USI standby thread (25MB) + book reading (about 50MB) ≒ 200MB.
        /// Eval Hash is invalid at NO SSE, but isn't it a problem if you evaluate it a little more?
        /// Larger joseki files require more memory than this.
        /// </summary>
        [DataMember]
        public Int64 WorkingMemory { get; set; } = 200;

        /// <summary>
        /// The size per thread. The unit is [MB]
        /// Threads setting × It is assumed that extra physical memory is consumed by this value.
        /// </summary>
        [DataMember]
        public Int64 StackPerThread { get; set; } = 25;

        /// <summary>
        /// Memory for merit function. The unit is [MB]
        /// (If EvalShare is turned on for memory sharing, the second engine will be deducted by this amount.)
        /// </summary>
        [DataMember]
        public Int64 EvalMemory { get; set; } = 470;

        /// <summary>
        /// Minimum memory for replacement table (HASH). A line that doesn't work properly without this much.
        /// The unit is [MB]
        /// 
        /// * On the GUI side, a warning is issued when the engine is selected if there is no free physical memory for RequiredMemory + MinimumHashMemory.
        /// </summary>
        [DataMember]
        public Int64 MinimumHashMemory { get; set; } = 128;

        /// <summary>
        /// Display order on the engine selection screen. This is supposed to be displayed in descending order.
        /// Default 0. (Displayed at the very end)
        /// 
        /// Book 10,000-19999 for commercial version.
        /// Commercial version (2018) uses 10000-10099.
        /// You who love Komatoku use 9000.
        /// The engine added by the user of the commercial version (2018) uses 8000. (Because I want it to be added at the end)
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Random setting collection
        /// </summary>
        [DataMember]
        public List<EnginePreset> Presets;

        /// <summary>
        /// EngineType == 0 : Normal search engine. (You can send a go command)
        /// EngineType == 1 : Does it correspond to Tsume Shogi? (Send go mate command)
        /// EngineType == 2 : Engine for normal search + Tsume shogi
        ///
        /// In addition, special engines such as those for checkered shogi will be added here in the future.
        /// </summary>
        [DataMember]
        public int EngineType { get; set; } = 0;

        /// <summary>
        /// A description of the options that can be set with setoption.
        /// Even if it is null, send "usi" to the engine and return what was returned at that time
        /// There is no problem because it is displayed.
        /// </summary>
        [DataMember]
        public List<EngineOptionDescription> EngineOptionDescriptions;

        /// <summary>
        /// List the supported USI extended protocols.
        /// 
        /// You don't have to.
        /// </summary>
        [DataMember]
        public List<ExtendedProtocol> SupportedExtendedProtocol;
    }

}
