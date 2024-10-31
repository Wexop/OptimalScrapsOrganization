using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalLevelLoader;
using OptimalScrapsOrganization.Patches;

namespace OptimalScrapsOrganization
{
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid)]
    [BepInDependency(Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OptimalScrapsOrganizationPlugin : BaseUnityPlugin
    {
        public const string GUID = "wexop.ship_item_reorder";
        public const string NAME = "ShipScrapReorder";
        public const string VERSION = "1.0.0";
        
        public ConfigEntry<float> distanceBewteenObjects;
        public ConfigEntry<int> organiseDefaultValueRange;

        public static OptimalScrapsOrganizationPlugin instance;

        private void Awake()
        {
            instance = this;

            Logger.LogInfo("OptimalScrapsOrganization starting....");

            LoadConfig();
                
            Harmony.CreateAndPatchAll(typeof(PatchTerminal));

            Logger.LogInfo("OptimalScrapsOrganization Patched !!");
        }

        private void LoadConfig()
        {
            
            //GENERAL
            distanceBewteenObjects = Config.Bind(
                "General", "distanceBewteenObjects", 
                0.6f, 
                "Distance between each stacks of objects. No need to restart the game :)"
            );
            CreateFloatConfig(distanceBewteenObjects,0f, 5f);
            
            organiseDefaultValueRange = Config.Bind(
                "General", "organiseDefaultValueRange", 
                20, 
                "Distance between each stacks of objects. No need to restart the game :)"
            );
            CreateIntConfig(organiseDefaultValueRange,1, 200);
        }

        private void CreateFloatConfig(ConfigEntry<float> configEntry, float min = 0f, float max = 100f)
        {
            var exampleSlider = new FloatSliderConfigItem(configEntry, new FloatSliderOptions
            {
                Min = min,
                Max = max,
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        private void CreateIntConfig(ConfigEntry<int> configEntry, int min = 0, int max = 100)
        {
            var exampleSlider = new IntSliderConfigItem(configEntry, new IntSliderOptions()
            {
                Min = min,
                Max = max,
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        private void CreateStringConfig(ConfigEntry<string> configEntry)
        {
            var exampleSlider = new TextInputFieldConfigItem(configEntry, new TextInputFieldOptions
            {
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        private void CreateBoolConfig(ConfigEntry<bool> configEntry)
        {
            var exampleSlider = new BoolCheckBoxConfigItem(configEntry, new BoolCheckBoxOptions
            {
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        public bool StringContain(string name, string verifiedName)
        {
            var name1 = name.ToLower();
            while (name1.Contains(" ")) name1 = name1.Replace(" ", "");

            var name2 = verifiedName.ToLower();
            while (name2.Contains(" ")) name2 = name2.Replace(" ", "");

            return name1.Contains(name2);
        }
    }
}