using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace QuickShop
{
    public class Config
    {
        public const string ModName = "QuickShop";
        private const string ConfigCategoryName = "QuickShop";

        // Config
        public KeyCode BuyKeyCode => _buyKeyCode.Value;
        public KeyCode BuyTunedKeyCode => _buyTunedKeyCode.Value;
        public int PriceDiscount
        {
            get
            {
                if (_priceDiscount.Value < 0) return 0;
                if (_priceDiscount.Value > 100) return 100;
                return _priceDiscount.Value;
            }
        }
        public bool BuyAdditionalParts => _buyAdditionalParts.Value;
        public bool BuyNormalPartIfTunedPartDoesntExist => _buyNormalPartIfTunedPartDoesntExist.Value;
        public bool DisableWarningMessage => _disableWarningMessage.Value;
        public bool AlwaysBuyTunedPart => _alwaysBuyTunedPart.Value;
        public string LicensePlateType => _licensePlateType.Value;
        public string LicensePlateText
        {
            get
            {
                if (_licensePlateText.Value.Length < 1) return "Quick123";
                if (_licensePlateText.Value.Length > 8) return "Quick123";
                return _licensePlateText.Value;
            }
        }

        // Parts
        public List<RequiredPart> RequiredParts => _requiredParts;

        // Config
        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Entry<KeyCode> _buyKeyCode;
        private readonly MelonPreferences_Entry<KeyCode> _buyTunedKeyCode;
        private readonly MelonPreferences_Entry<int> _priceDiscount;
        private readonly MelonPreferences_Entry<bool> _buyAdditionalParts;
        private readonly MelonPreferences_Entry<bool> _buyNormalPartIfTunedPartDoesntExist;
        private readonly MelonPreferences_Entry<bool> _disableWarningMessage;
        private readonly MelonPreferences_Entry<bool> _alwaysBuyTunedPart;
        private readonly MelonPreferences_Entry<string> _licensePlateType;
        private readonly MelonPreferences_Entry<string> _licensePlateText;

        // Parts
        private List<RequiredPart> _requiredParts;

        public Config()
        {
            // Config
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/QuickShop.cfg");

            _buyKeyCode = _config.CreateEntry(nameof(BuyKeyCode), KeyCode.B);
            _buyTunedKeyCode = _config.CreateEntry(nameof(BuyTunedKeyCode), KeyCode.N);
            _priceDiscount = _config.CreateEntry(nameof(PriceDiscount), 15);
            _buyAdditionalParts = _config.CreateEntry(nameof(BuyAdditionalParts), true);
            _buyNormalPartIfTunedPartDoesntExist = _config.CreateEntry(nameof(BuyNormalPartIfTunedPartDoesntExist), false);
            _disableWarningMessage = _config.CreateEntry(nameof(DisableWarningMessage), false);
            _alwaysBuyTunedPart = _config.CreateEntry(nameof(AlwaysBuyTunedPart), false);
            _licensePlateType = _config.CreateEntry(nameof(LicensePlateType), "Standard");
            _licensePlateText = _config.CreateEntry(nameof(LicensePlateText), "Quick123");

            // Parts
            if (!File.Exists("Mods/QuickShop_parts.json"))
                CreatePartsFile();
            string jsonString = File.ReadAllText("Mods/QuickShop_parts.json");
            _requiredParts = JsonConvert.DeserializeObject<ListRequiredParts>(jsonString).RequiredParts;
        }

        public void Reload()
        {
            _config.LoadFromFile();
            string jsonString = File.ReadAllText("Mods/QuickShop_parts.json");
            _requiredParts = JsonConvert.DeserializeObject<ListRequiredParts>(jsonString).RequiredParts;
        }

        private void CreatePartsFile()
        {
            List<RequiredPart> listparts = new List<RequiredPart>
            {
                new RequiredPart
                {
                    _ = "Front Shock Absorber A => Front Spring, Front Shock Absorber Cap",
                    MainPart = "amortyzatorPrzod_1",
                    AdditionalParts = "sprezynnaPrzod_1,czapkaAmorPrzod_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Double Wishbone Shock Absorber => Front Spring, Front Shock Absorber Cap",
                    MainPart = "amortyzator_double",
                    AdditionalParts = "sprezynnaPrzod_1,czapkaAmorPrzod_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Double Wishbone Shock Absorber Rear => Rear Spring, Rear Shock Absorber Cap",
                    MainPart = "amortyzator_double_rear",
                    AdditionalParts = "sprezynaTyl_1,czapkaSprezynyTyl_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Radiator Fan Housing B => Radiator Fan Housing B Fan 1, Radiator Fan Housing B Fan 2",
                    MainPart = "wentylatorChlodnicy_2",
                    AdditionalParts = "wentylatorChlodnicy_2_fan_1,wentylatorChlodnicy_2_fan_2",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Radiator Fan Housing => Radiator Fan Housing Fan",
                    MainPart = "wentylatorChlodnicy_1",
                    AdditionalParts = "wentylatorChlodnicy_1_fan_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_1",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember B => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_2",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember C => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_4",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember D => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_5",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember E => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_6",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember F => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_7",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember G => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_8",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Front Suspension Crossmember H => Rubber Bushing (x4)",
                    MainPart = "sankiPrzod_9",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Rear Suspension Crossmember => Rubber Bushing (x4)",
                    MainPart = "sankiTyl_1",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Rear Suspension Crossmember A => Rubber Bushing (x6)",
                    MainPart = "sankiTyl_gtr",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Rear Suspension Crossmember (RWD) => Rubber Bushing (x4)",
                    MainPart = "sankiTyl_2",
                    AdditionalParts = "tuleja_1,tuleja_1,tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Upper Suspension Arm => Rubber Bushing (x2)",
                    MainPart = "wahaczGora_double",
                    AdditionalParts = "tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Upper Suspension Arm B => Rubber Bushing (x2)",
                    MainPart = "wahaczGora_double_2",
                    AdditionalParts = "tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Bottom Suspension Arm A => Rubber Bushing (x2)",
                    MainPart = "wahaczDol_double",
                    AdditionalParts = "tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Bottom Suspension Arm B => Rubber Bushing (x2)",
                    MainPart = "wahaczDol_double_2",
                    AdditionalParts = "tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Bottom Suspension Arm C => Rubber Bushing (x2)",
                    MainPart = "wahaczDol_double_3",
                    AdditionalParts = "tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Lower Suspension Arm => Rubber Bushing (x2)",
                    MainPart = "wahaczDolny_1",
                    AdditionalParts = "tuleja_1,tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Rear Suspension Arm => Rubber Bushing",
                    MainPart = "wahaczDlugiTyl_1",
                    AdditionalParts = "tuleja_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Rear Suspension Arm B => Rubber Bushing, Small Rubber Bushing",
                    MainPart = "wahaczTyl_2",
                    AdditionalParts = "tuleja_1,tulejaMala_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Rear Suspension Upper Arm => Rubber Bushing, Small Rubber Bushing",
                    MainPart = "wahaczKrotkiTyl_1",
                    AdditionalParts = "tuleja_1,tulejaMala_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Leaf Spring => Small Rubber Bushing (x2)",
                    MainPart = "pioraTyl_1",
                    AdditionalParts = "tulejaMala_1,tulejaMala_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Brake Caliper => Brake Caliper Cylinder",
                    MainPart = "zaciskHamulcowy_1",
                    AdditionalParts = "zaciskHamulcowy_tloczek_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Piston with Conrod => Piston Rings",
                    MainPart = "tlok_1",
                    AdditionalParts = "tlok_1_pierscienie_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Piston with Conrod (V8 OHV B) => Piston Rings (V8 OHV B)",
                    MainPart = "v8_409_tlok",
                    AdditionalParts = "v8_409_tlok_pierscienie_1",
                    Enabled = true
                },
                new RequiredPart
                {
                    _ = "Piston with Conrod (V8 OHV C) => Piston Rings (V8 OHV C)",
                    MainPart = "v8_hemi_tlok",
                    AdditionalParts = "v8_hemi_tlok_pierscienie_1",
                    Enabled = true
                }
            };

            ListRequiredParts jsonparts = new ListRequiredParts
            {
                RequiredParts = listparts
            };

            using (var file = File.CreateText("Mods/QuickShop_parts.json"))
            {
                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, jsonparts);
            }
        }
    }

    public class RequiredPart
    {
        public string _ { get; set; }
        public string MainPart { get; set; }
        public string AdditionalParts { get; set; }
        public bool Enabled { get; set; }
    }

    public class ListRequiredParts
    {
        public List<RequiredPart> RequiredParts { get; set; }
    }
}