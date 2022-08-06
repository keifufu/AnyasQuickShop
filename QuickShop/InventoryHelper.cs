using System;
using System.Collections.Generic;
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

        private class AdditionalItem
        {
            public string ItemID;
            public int amount;
            public int cost;
        }

        private int CalculatePrice(string ItemID, int itemAmount)
        {
            // Return 0 on rims and tires since their price calculation is handled in `BuyPart`
            if (ItemID.Contains("rim_") || ItemID.Contains("tire_")) return 0;
            int price = Singleton<GameInventory>.Instance.GetItemProperty(ItemID).Price;
            // License plate IDs are weird and their price is always 100
            if (ItemID.Contains("license_")) price = 100;
            return CalculateDiscount(price, itemAmount);
        }

        private int CalculateDiscount(int price, int itemAmount = 1)
        {
            decimal appliedDiscount = price * ((100m - _config.PriceDiscount) / 100m);
            decimal discounted = Math.Ceiling(appliedDiscount);
            return Convert.ToInt32(discounted) * itemAmount;
        }

        private bool TunedPartExists(string ItemID)
        {
            // Tuned break discs don't use t_(ItemID) and we know they exist
            if (ItemID.Contains("tarczaWentylowana")) return true;

            // `LocalizedName` defaults to `ItemID` if the Item does not exist.
            ItemID = $"t_{ItemID}";
            return Singleton<GameInventory>.Instance.GetItemProperty(ItemID).LocalizedName != ItemID;
        }

        private string GetTunedItemIDIfExists(string ItemID)
        {
            if (ItemID.StartsWith("t_")) ItemID = ItemID.Replace("t_", "");
            if (!TunedPartExists(ItemID)) return ItemID;

            if (ItemID.Contains("tarczaWentylowana")) return "tarczaWentylowana_5";
            return $"t_{ItemID}";
        }

        private Item ItemFromID(string ItemID)
        {
            Item item = new Item
            {
                ID = ItemID
            };
            item.MakeNewUID();

            return item;
        }

        public void BuyPart(string ItemID, bool buyTuned, int itemAmount)
        {
            if (ItemID == "" || ItemID == null) return;

            var inventory = Singleton<Inventory>.Instance;
            var uiManager = UIManager.Get();
            // List of additional items to buy (Ex: Piston Rings)
            List<AdditionalItem> additionalItems = new List<AdditionalItem>();

            if (buyTuned)
            {
                // If no tuned variant exists and we don't always buy tuned parts, notify the player.
                if (!TunedPartExists(ItemID) && !_config.AlwaysBuyTunedPart)
                {
                    // Notify the player that no tuned part could be found and return
                    if (!_config.BuyNormalPartIfTunedPartDoesntExist)
                    {
                        uiManager.ShowPopup(Config.ModName, "No tuned part was found", PopupType.Normal);
                        return;
                    }
                    // Notify the player that a normal part will be bought instead
                    else if (!_config.DisableWarningMessage)
                    {
                        uiManager.ShowPopup(Config.ModName, "No tuned part was found, buying normal part instead", PopupType.Buy);
                    }
                }

                // Change ItemID to a tuned ID if a tuned variant exists
                ItemID = GetTunedItemIDIfExists(ItemID);
            }

            // Calculate price of the main item
            // _MainPartPrice is used to show the amount in the popup
            int _MainPartPrice = CalculatePrice(ItemID, itemAmount);
            int FinalPrice = CalculatePrice(ItemID, itemAmount);
            Item item = ItemFromID(ItemID);

            // Calculate additional items and their price
            if (_config.BuyAdditionalParts)
            {
                for (int i = 0; i < _config.RequiredParts.Count; i++)
                {
                    RequiredPart part = _config.RequiredParts[i];
                    if (!part.Enabled) continue;
                    if (!ItemID.Equals(part.MainPart) && !ItemID.Replace("t_", "").Equals(part.MainPart)) continue;
                    if (part.AdditionalParts == "" || part.AdditionalParts == null) continue;
                    string[] AdditionalParts = part.AdditionalParts.Split(',');
                    foreach (string _additionalPartId in AdditionalParts)
                    {
                        if (_additionalPartId == "" || _additionalPartId == null) continue;
                        string additionalPartId = buyTuned ? GetTunedItemIDIfExists(_additionalPartId) : _additionalPartId;

                        // Calculate price
                        int partCost = CalculatePrice(additionalPartId, itemAmount);

                        // Add item to additionalItems
                        AdditionalItem additionalItem = additionalItems.Find(x => x.ItemID == additionalPartId);
                        if (additionalItem != null)
                        {
                           additionalItem.amount += itemAmount;
                           additionalItem.cost += partCost;
                        }
                        else
                        {
                            additionalItems.Add(new AdditionalItem { ItemID = additionalPartId, amount = itemAmount, cost = partCost });
                        }

                        // Calculate price
                        FinalPrice += partCost;
                    }
                }
            }

            // Create license plate
            if (ItemID.Contains("license_"))
            {
                LPData licensePlate = new LPData
                {
                    Name = _config.LicensePlateType,
                    Custom = _config.LicensePlateText
                };
                item.LPData = licensePlate;
                item.ID = "LicensePlate";
            }

            // Calculate rim data and price
            if (ItemID.Contains("rim_"))
            {
                string FactoryTireSize = GameScript.Get().GetIOMouseOverCarLoader2().GetFrontTireSize();
                bool isFrontTires = IsFocusedOnFrontWheel();

                string[] WheelSizes = (FactoryTireSize.Contains("|"))
                    ? FactoryTireSize.Split('|')[isFrontTires ? 0 : 1].Split('R')
                    : FactoryTireSize.Split('R');

                int WheelSize = int.Parse(WheelSizes[1]);

                WheelData wheel = new WheelData
                {
                    Size = WheelSize,
                    ET = _config.RimET
                };
                item.WheelData = wheel;
                int partCost = CalculateDiscount(Helper.GetRimPrice(ItemID, WheelSize, _config.RimET), itemAmount);
                FinalPrice += partCost;
                _MainPartPrice = partCost;
            }

            // Calculate tire data and price
            if (ItemID.Contains("tire_"))
            {
                string FactoryTireSize = GameScript.Get().GetIOMouseOverCarLoader2().GetFrontTireSize();
                bool isFrontTires = IsFocusedOnFrontWheel();

                string[] WheelSizes = (FactoryTireSize.Contains("|"))
                    ? FactoryTireSize.Split('|')[isFrontTires ? 0 : 1].Split('R')
                    : FactoryTireSize.Split('R');

                string[] WheelSizes2 = WheelSizes[0].Split('/');
                int WheelSize = int.Parse(WheelSizes[1]);
                int WheelWidth = int.Parse(WheelSizes2[0]);
                int WheelProfile = int.Parse(WheelSizes2[1]);

                WheelData wheel = new WheelData
                {
                    Size = WheelSize,
                    Width = WheelWidth,
                    Profile = WheelProfile
                };
                item.WheelData = wheel;
                int partCost = CalculateDiscount(Helper.GetTirePrice(ItemID, WheelWidth, WheelProfile, WheelSize), itemAmount);
                FinalPrice += partCost;
                _MainPartPrice = partCost;
            }

            // Check if the player can afford the parts
            if (GlobalData.PlayerMoney < FinalPrice)
            {
                uiManager.ShowPopup(Config.ModName, $"Not enought money! ({GlobalData.PlayerMoney} < {FinalPrice})", PopupType.Buy);
                return;
            }

            // Add additional items to inventory
            foreach (AdditionalItem _additionalItem in additionalItems)
            {
                Item additionalItem = ItemFromID(_additionalItem.ItemID);
                for (int i = 0; i < _additionalItem.amount; i++)
                    inventory.Add(additionalItem);

                string _itemAmountString = _additionalItem.amount > 1 ? $" x{_additionalItem.amount}" : "";
                string _costString = _config.ShowPartCost ? $" ({_additionalItem.cost} CR)" : "";
                uiManager.ShowPopup(Config.ModName, $"Added {additionalItem.GetLocalizedName()}{_itemAmountString}", PopupType.Buy);
            }

            // Add main item to inventory and charge player
            for (int i = 0; i < itemAmount; i++)
                inventory.Add(item);
            string itemAmountString = itemAmount > 1 ? $" x{itemAmount}" : "";
            string costString = _config.ShowPartCost ? $" ({_MainPartPrice} CR)" : "";
            uiManager.ShowPopup(Config.ModName, $"Added {item.GetLocalizedName()}{itemAmountString}{costString}", PopupType.Buy);
            GlobalData.AddPlayerMoney(-FinalPrice);
        }

        // This is most certainly not the best way to do this but I couldn't find a better way :shrug:
        // This function exists to purchase the correct rim and tire sizes for cars with multiple wheel sizes.
        // This compares coordinates to hard-coded ones, so I expect there to be issues in the future.
        bool IsFocusedOnFrontWheel()
        {
            float partX = GameScript.Get().GetPartMouseOver().GetNextMountObject().position.x;
            float partZ = GameScript.Get().GetPartMouseOver().GetNextMountObject().position.z;

            // Left here for debugging purposes
            // MelonLogger.Msg($"X: {partX}, Z: {partZ}");

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
