using System;
using MelonLoader;

namespace QuickShop
{
    public class InventoryHelper
    {
        private readonly Config _config;
        public InventoryHelper(Config config)
        {
            _config = config;
        }

        private int CalculatePrice(string ItemId)
        {
            if (ItemId.Contains("rim_") || ItemId.Contains("tire_")) return 0;
            int price = Singleton<GameInventory>.Instance.GetItemProperty(ItemId).Price;
            return CalculateDiscount(price);
        }

        private int CalculateDiscount(int price)
        {
            decimal appliedDiscount = price * ((100m - _config.PriceDiscount) / 100m);
            decimal discounted = Math.Ceiling(appliedDiscount);
            return Convert.ToInt32(discounted);
        }

        private bool TunedPartExists(string ItemId)
        {
            if (ItemId == "" || ItemId == null || ItemId == "t_") return false;
            if (!ItemId.StartsWith("t_"))
                ItemId = $"t_{ItemId}";

            // `LocalizedName` defaults to `ItemId` if the Item does not exist.
            var Exists = Singleton<GameInventory>.Instance.GetItemProperty(ItemId).LocalizedName != ItemId;
            return Exists;
        }

        public static Item ItemFromID(string ItemId)
        {
            Item item = new Item
            {
                ID = ItemId
            };
            item.MakeNewUID();

            return item;
        }

        public void BuyPart(string ItemId, bool buyTuned = false)
        {
            if (ItemId == "" || ItemId == "t_" || ItemId == null) return;

            var inventory = Singleton<Inventory>.Instance;
            var uiManager = UIManager.Get();

            // Check if a tuned variant exists and change itemId accordingly
            if (buyTuned)
            {
                if (!TunedPartExists(ItemId))
                {
                    if (!_config.BuyNormalPartIfTunedPartDoesntExist && !_config.AlwaysBuyTunedPart)
                    {
                        uiManager.ShowPopup(Config.ModName, "No tuned part was found.", PopupType.Normal);
                        return;
                    }
                    else if (!_config.DisableWarningMessage && !_config.AlwaysBuyTunedPart)
                    {
                        uiManager.ShowPopup(Config.ModName, "No tuned part was found, buying normal part instead.", PopupType.Buy);
                    }
                }
                else
                {
                    ItemId = $"t_{ItemId}";
                }
            }

            int FinalPrice = CalculatePrice(ItemId);
            Item item = ItemFromID(ItemId);

            // Calculate Price of additional items
            if (_config.BuyAdditionalParts)
            {
                for (int i = 0; i < _config.RequiredParts.Count; i++)
                {
                    RequiredPart part = _config.RequiredParts[i];
                    if (!part.Enabled) continue;
                    if (!ItemId.Equals(part.MainPart) && !ItemId.Replace("t_", "").Equals(part.MainPart)) continue;
                    if (part.AdditionalParts == "" || part.AdditionalParts == null) continue;
                    string[] AdditionalParts = part.AdditionalParts.Split(',');
                    foreach (string additionalPart in AdditionalParts)
                    {
                        string additionalPartId = additionalPart;
                        if (additionalPartId == "" || additionalPartId == null) continue;
                        if (buyTuned && TunedPartExists(additionalPartId) && !additionalPartId.StartsWith("t_"))
                            additionalPartId = $"t_{additionalPartId}";
                        FinalPrice += CalculatePrice(additionalPartId);
                    }
                }
            }

            // Create license plate
            if (ItemId.Contains("license_"))
            {
                LPData licensePlate = new LPData
                {
                    Name = _config.LicensePlateType,
                    Custom = _config.LicensePlateText
                };
                item.LPData = licensePlate;
                item.ID = "LicensePlate";
                // Apply Fixed price for License Plate
                FinalPrice += CalculateDiscount(100);
            }

            // Calculate rim data and price
            if (ItemId.Contains("rim_"))
            {
                string FactoryTireSize = GameScript.Get().GetIOMouseOverCarLoader2().GetFrontTireSize();
                bool isFrontTires = IsFocusedWheelFront();

                string[] WheelSizes = (FactoryTireSize.Contains("|"))
                    ? FactoryTireSize.Split('|')[isFrontTires ? 0 : 1].Split('R')
                    : FactoryTireSize.Split('R');

                int WheelSize = Int32.Parse(WheelSizes[1]);

                WheelData wheel = new WheelData
                {
                    Size = WheelSize
                };
                item.WheelData = wheel;
                FinalPrice += CalculateDiscount(Helper.GetRimPrice(ItemId, WheelSize, 0));
            }

            // Calculate tire data and price
            if (ItemId.Contains("tire_"))
            {
                string FactoryTireSize = GameScript.Get().GetIOMouseOverCarLoader2().GetFrontTireSize();
                bool isFrontTires = IsFocusedWheelFront();

                string[] WheelSizes = (FactoryTireSize.Contains("|"))
                    ? FactoryTireSize.Split('|')[isFrontTires ? 0 : 1].Split('R')
                    : FactoryTireSize.Split('R');

                string[] WheelSizes2 = WheelSizes[0].Split('/');
                int WheelSize = Int32.Parse(WheelSizes[1]);
                int WheelWidth = Int32.Parse(WheelSizes2[0]);
                int WheelProfile = Int32.Parse(WheelSizes2[1]);

                WheelData wheel = new WheelData
                {
                    Size = WheelSize,
                    Width = WheelWidth,
                    Profile = WheelProfile
                };
                item.WheelData = wheel;
                FinalPrice += CalculateDiscount(Helper.GetTirePrice(ItemId, WheelWidth, WheelProfile, WheelSize));
            }

            // Check if Player has enough money
            if (GlobalData.PlayerMoney < FinalPrice)
            {
                uiManager.ShowPopup(Config.ModName, $"Not enought money! ({GlobalData.PlayerMoney} < {FinalPrice})", PopupType.Buy);
                return;
            }

            // Buy additional items
            if (_config.BuyAdditionalParts)
            {
                for (int i = 0; i < _config.RequiredParts.Count; i++)
                {
                    RequiredPart part = _config.RequiredParts[i];
                    if (!part.Enabled) continue;
                    if (!ItemId.Equals(part.MainPart) && !ItemId.Replace("t_", "").Equals(part.MainPart)) continue;
                    if (part.AdditionalParts == "" || part.AdditionalParts == null) continue;
                    string[] AdditionalParts = part.AdditionalParts.Split(',');
                    foreach (string additionalPart in AdditionalParts)
                    {
                        string additionalPartId = additionalPart;
                        if (additionalPartId == "" || additionalPartId == null) continue;
                        if (buyTuned && TunedPartExists(additionalPartId) && !additionalPartId.StartsWith("t_"))
                            additionalPartId = $"t_{additionalPartId}";
                        Item additionalItem = ItemFromID(additionalPartId);
                        inventory.Add(additionalItem, true);
                    }
                }
            }

            // Buy item and charge player
            inventory.Add(item, true);
            GlobalData.AddPlayerMoney(-FinalPrice);
        }

        // This is certainly not the best way to do this but I can't find a better way to do this :shrug:
        bool IsFocusedWheelFront()
        {
            float partX = GameScript.Get().GetPartMouseOver().GetNextMountObject().position.x;
            float partZ = GameScript.Get().GetPartMouseOver().GetNextMountObject().position.z;

            //MelonLogger.Msg($"X: {partX}, Z: {partZ}");

            // Car Lift A, B
            if (partZ < 2 && partZ > -1 && partX < 7 && partX > -1)
            {
                if (partZ < 0) return true;
                else return false;
            }

            // Garage Entrance A, B
            if (partZ < 16 && partZ > 12 && partX < 7 && partX > -1)
            {
                if (partZ < 13) return true;
                else return false;
            }

            // Garage Entrance C
            if (partZ < 14 && partZ > 11 && partX < 16 && partX > 11)
            {
                if (partX < 13) return true;
                else return false;
            }

            // Car Wash
            if (partZ < -33 && partZ > -38 && partX < 11 && partX > 8)
            {
                if (partZ < -36) return true;
                else return false;
            }

            // Dyno
            if (partZ < -30 && partZ > -34 && partX < 23 && partX > 20)
            {
                if (partZ < -32) return true;
                else return false;
            }

            // Paintshop
            if (partZ < -36 && partZ > -41 && partX < -6 && partX > -9)
            {
                if (partZ < -38) return true;
                else return false;
            }

            // Test Path
            if (partZ < -28 && partZ > -33 && partX < -19 && partX > -22)
            {
                if (partZ < -31) return true;
                else return false;
            }

            return true;
        }
    }
}
