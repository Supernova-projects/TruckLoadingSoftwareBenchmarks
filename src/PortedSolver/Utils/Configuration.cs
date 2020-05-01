using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PortedSolver.Utils
{
    public static class Configuration
    {
        private static readonly JObject _configurationObject;

        public static string INPUT_FILE => _configurationObject["input.file"].ToObject<string>();

        public static string OUTPUT_FILE => $"{_configurationObject["output.file"].ToObject<string>()} {DateTime.Now:yyyy-MM-dd hh-mm-ss} .csv";

        public static int PACKING_METHOD => _configurationObject["packing.method"].ToObject<int>();

        public static double CONTAINER_WIDTH => _configurationObject["container.width"].ToObject<double>();

        public static double CONTAINER_HEIGHT => _configurationObject["container.height"].ToObject<double>();

        public static double CONTAINER_DEPTH => _configurationObject["container.depth"].ToObject<double>();

        public static double CONTAINER_MAX_WEIGHT => _configurationObject["container.max.weight"].ToObject<double>();

        public static double WEIGHT_SURPLUS_PARAMETER => _configurationObject["container.weight.surplus.parameter"].ToObject<double>();

        public static bool CONTAINER_UNLOADABLE_FROM_SIDE => _configurationObject["container.unloadable.from.side"].ToObject<bool>();

        public static int MAX_ZONE1_WEIGHT => _configurationObject["max.zone1.weight"].ToObject<int>();

        public static int MAX_ZONE2_WEIGHT => _configurationObject["max.zone2.weight"].ToObject<int>();

        public static int MAX_ZONE3_WEIGHT => _configurationObject["max.zone3.weight"].ToObject<int>();

        public static int MAX_ZONE4_WEIGHT => _configurationObject["max.zone4.weight"].ToObject<int>();

        public static int MAX_MIXED_ZONES12_WEIGHT => _configurationObject["max.mixed.zones12.weight"].ToObject<int>();

        public static int MAX_MIXED_ZONES34_WEIGHT => _configurationObject["max.mixed.zones34.weight"].ToObject<int>();

        public static int TIMES_MULTIRUN => _configurationObject["times.multirun"].ToObject<int>();

        public static int READING_MULTIITEM_METHOD => _configurationObject["reading.multiitem.method"].ToObject<int>();

        public static double ITEM_EXCEEDING_PARAMETER => _configurationObject["item.exceeding.parameter"].ToObject<double>();

        static Configuration()
        {
            var config = File.ReadAllText("appsettings.json");
            _configurationObject = JsonConvert.DeserializeObject<JObject>(config);
        }
    }
}
