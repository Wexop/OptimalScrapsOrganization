using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalLevelLoader;
using OptimalScrapsOrganization.Patches;
using OptimalScrapsOrganization.Scripts;

namespace OptimalScrapsOrganization
{
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid)]
    [BepInDependency(Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OptimalScrapsOrganizationPlugin : BaseUnityPlugin
    {
        public const string GUID = "wexop.ship_item_reorder";
        public const string NAME = "ShipScrapReorder";
        public const string VERSION = "1.0.5";

        public bool hasPatchedStartTerminal;
        
        public ConfigEntry<float> distanceBetweenScraps;
        public ConfigEntry<float> rotationBetweenScraps;
        public ConfigEntry<int> organiseDefaultValueRange;
        public ConfigEntry<bool> autoReorderOnLeftMoon;
        public ConfigEntry<bool> rotateScraps;
        public ConfigEntry<bool> orderShopItems;
        public ConfigEntry<bool> orderPlacedItems;
        public ConfigEntry<bool> hideHelpMessage;
        public ConfigEntry<string> exclusionList;
        public ConfigEntry<OrganizeBy> defaultReorderType;

        public static OptimalScrapsOrganizationPlugin instance;

        private void Awake()
        {
            instance = this;

            Logger.LogInfo("OptimalScrapsOrganization starting....");

            LoadConfig();
                
            Harmony.CreateAndPatchAll(typeof(PatchTerminal));
            Harmony.CreateAndPatchAll(typeof(PatchStartOfRound));

            Logger.LogInfo("OptimalScrapsOrganization Patched !!");
        }

        private void LoadConfig()
        {
            
            //GENERAL
            exclusionList = Config.Bind(
                "General", "exclusionList", 
                "clipboard,stickynote", 
                "List of items not affected by reorder. Example : shovel,airhorn. No need to restart the game :)"
            );
            CreateStringConfig(exclusionList);
            
            distanceBetweenScraps = Config.Bind(
                "General", "distanceBetweenScraps", 
                1.1f, 
                "Distance between each stacks of scraps. No need to restart the game :)"
            );
            CreateFloatConfig(distanceBetweenScraps,0f, 5f);
            
            organiseDefaultValueRange = Config.Bind(
                "General", "organiseDefaultValueRange", 
                20, 
                "Distance between each stacks of objects. No need to restart the game :)"
            );
            CreateIntConfig(organiseDefaultValueRange,1, 200);
            
            defaultReorderType = Config.Bind(
                "General", "defaultReorderType", 
                OrganizeBy.VALUE, 
                "Default reorder type. No need to restart the game :)"
            );
            CreateOrganiseByConfig(defaultReorderType);
            
            autoReorderOnLeftMoon = Config.Bind(
                "General", "autoReorderOnLeftMoon", 
                true, 
                "Auto reorder scraps when leaving the moon. No need to restart the game :)"
            );
            CreateBoolConfig(autoReorderOnLeftMoon);
            
            rotateScraps = Config.Bind(
                "General", "rotateScraps", 
                true, 
                "Rotate scraps so you can see how many of them are in a stack. No need to restart the game :)"
            );
            CreateBoolConfig(rotateScraps);
            
            rotationBetweenScraps = Config.Bind(
                "General", "rotationBetweenScraps", 
                10f, 
                "Rotate scraps so you can see how many of them are in a stack. No need to restart the game :)"
            );
            CreateFloatConfig(rotationBetweenScraps);
            
            orderShopItems = Config.Bind(
                "General", "orderShopItems", 
                true, 
                "Reorder affect shop items (flashlights, shovel, jetpack...). No need to restart the game :)"
            );
            CreateBoolConfig(orderShopItems);
            
            orderPlacedItems = Config.Bind(
                "General", "orderPlacedItems", 
                true, 
                "Reorder placed items, for example in locker. No need to restart the game :)"
            );
            CreateBoolConfig(orderPlacedItems);
            
            hideHelpMessage = Config.Bind(
                "General", "hideHelpMessage", 
                false, 
                "Hide help command message. No need to restart the game :)"
            );
            CreateBoolConfig(hideHelpMessage);
            

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

        private void CreateOrganiseByConfig(ConfigEntry<OrganizeBy> configEntry)
        {
            var exampleSlider = new EnumDropDownConfigItem<OrganizeBy>(configEntry, false);
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