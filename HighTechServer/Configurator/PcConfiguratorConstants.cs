namespace HighTech.Configurator
{
    public static class PcConfiguratorConstants
    {
        public static class Categories
        {
            public const string Cpu = "CPU";
            public const string Motherboard = "Motherboard";
            public const string Ram = "RAM";
            public const string Gpu = "GPU";
            public const string Storage = "Storage";
            public const string Psu = "PSU";
            public const string Case = "Case";
            public const string CpuCooler = "CPU Cooler";
            public const string Assembly = "Assembly";

            public static readonly IReadOnlyList<string> All = new[]
            {
                Cpu, Motherboard, Ram, Gpu, Storage, Psu, Case, CpuCooler, Assembly
            };

            public static readonly IReadOnlySet<string> ConfiguratorOnly = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Assembly
            };
        }

        public static class AssemblyProduct
        {
            public const string Manufacturer = "HighTech";
            public const string Model = "Standard PC Assembly";
        }

        public static class RuleCodes
        {
            public const string CompleteBuild = "COMPLETE_BUILD";
            public const string AssemblyService = "ASSEMBLY_SERVICE";
            public const string R1 = "R1";
            public const string R2 = "R2";
            public const string R3 = "R3";
            public const string R4 = "R4";
            public const string R5 = "R5";
            public const string R6 = "R6";
            public const string R7 = "R7";
            public const string R8 = "R8";
            public const string R9 = "R9";
        }

        public static class Fields
        {
            public const string SocketType = "Socket Type";
            public const string TdpW = "TDP (W)";
            public const string GraphicCore = "Graphic Core";
            public const string RamType = "RAM Type";
            public const string MotherboardFormFactor = "Motherboard Form Factor";
            public const string SupportedMotherboardFormFactors = "Supported Motherboard Form Factors";
            public const string MaxRamSpeedMhz = "Max RAM Speed (MHz)";
            public const string RamSpeedMhz = "RAM Speed (MHz)";
            public const string LengthMm = "Length (mm)";
            public const string MaxGpuLengthMm = "Max GPU Length (mm)";
            public const string CaseType = "Case Type";
            public const string CaseLengthMm = "Case Length (mm)";
            public const string CaseWidthMm = "Case Width (mm)";
            public const string CaseHeightMm = "Case Height (mm)";
            public const string RecommendedPsuW = "Recommended PSU (W)";
            public const string WattageW = "Wattage (W)";
            public const string HeightMm = "Height (mm)";
            public const string MaxCpuCoolerHeightMm = "Max CPU Cooler Height (mm)";
            public const string StorageInterface = "Storage Interface";
            public const string M2Slots = "M.2 Slots";
            public const string SataPorts = "SATA Ports";

            public static readonly IReadOnlyList<string> All = new[]
            {
                SocketType, TdpW, GraphicCore, RamType, MotherboardFormFactor, SupportedMotherboardFormFactors,
                MaxRamSpeedMhz, RamSpeedMhz, LengthMm, MaxGpuLengthMm, CaseLengthMm, CaseWidthMm, CaseHeightMm, RecommendedPsuW,
                WattageW, HeightMm, MaxCpuCoolerHeightMm, StorageInterface, M2Slots, SataPorts
            };
        }

        // (categoryName, fieldName) pairs that must always exist; blocking admin deletion
        public static readonly IReadOnlyList<(string Category, string Field)> RequiredPairings = new[]
        {
            (Categories.Cpu,          Fields.SocketType),
            (Categories.Cpu,          Fields.TdpW),
            (Categories.Cpu,          Fields.GraphicCore),
            (Categories.Motherboard,  Fields.SocketType),
            (Categories.Motherboard,  Fields.RamType),
            (Categories.Motherboard,  Fields.MotherboardFormFactor),
            (Categories.Motherboard,  Fields.MaxRamSpeedMhz),
            (Categories.Motherboard,  Fields.M2Slots),
            (Categories.Motherboard,  Fields.SataPorts),
            (Categories.Ram,          Fields.RamType),
            (Categories.Ram,          Fields.RamSpeedMhz),
            (Categories.Gpu,          Fields.LengthMm),
            (Categories.Gpu,          Fields.RecommendedPsuW),
            (Categories.Storage,      Fields.StorageInterface),
            (Categories.Psu,          Fields.WattageW),
            (Categories.Case,         Fields.SupportedMotherboardFormFactors),
            (Categories.Case,         Fields.MaxGpuLengthMm),
            (Categories.Case,         Fields.CaseLengthMm),
            (Categories.Case,         Fields.CaseWidthMm),
            (Categories.Case,         Fields.CaseHeightMm),
            (Categories.Case,         Fields.MaxCpuCoolerHeightMm),
            (Categories.CpuCooler,   Fields.SocketType),
            (Categories.CpuCooler,   Fields.HeightMm),
        };

        // ATX hierarchy: which form factors fit inside which case form factor
        public static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> FormFactorCompatibility =
            new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["EATX"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "EATX", "ATX", "mATX", "ITX" },
                ["ATX"]  = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ATX", "mATX", "ITX" },
                ["mATX"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "mATX", "ITX" },
                ["ITX"]  = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ITX" },
            };

        public const string LockReason = "Required by PC Configurator";
    }
}
