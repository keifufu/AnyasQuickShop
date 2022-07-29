using System;

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
            int discount = _config.PriceDiscount;
            if (discount < 0) discount = 0;
            if (discount > 100) discount = 100;
            int origPrice = Singleton<GameInventory>.Instance.GetItemProperty(ItemId).Price;
            decimal appliedDiscount = origPrice * ((100m - discount) / 100m);
            decimal price = Math.Ceiling(appliedDiscount);
            return Convert.ToInt32(price);
        }

        private bool TuningPartExists(string ItemId)
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

        public void BuyPart(string ItemId, bool buyTuning = false)
        {
            if (ItemId == "" || ItemId == "t_" || ItemId == null) return;

            var inventory = Singleton<Inventory>.Instance;
            var uiManager = UIManager.Get();

            // Check if a tuning variant exists and change itemId accordingly
            if (buyTuning)
            {
                if (!TuningPartExists(ItemId))
                {
                    if (!_config.BuyNormalPartIfTunedPartDoesntExist && !_config.AlwaysBuyTunedPart)
                    {
                        uiManager.ShowPopup(Config.ModName, "No tuning part was found.", PopupType.Normal);
                        return;
                    }
                    else if (!_config.DisableWarningMessage && !_config.AlwaysBuyTunedPart)
                    {
                        uiManager.ShowPopup(Config.ModName, "No tuning part was found, buying normal part instead.", PopupType.Buy);
                    }
                }
                else
                {
                    ItemId = $"t_{ItemId}";
                }
            }

            // Calculate price of main item
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
                        if (buyTuning && TuningPartExists(additionalPartId) && !additionalPartId.StartsWith("t_"))
                            additionalPartId = $"t_{additionalPartId}";
                        FinalPrice += CalculatePrice(additionalPartId);
                    }
                }
            }

            // Calculate rim data
            if (ItemId.Contains("rim_"))
            {
                string CarDefaultWheel = GameScript.Get().GetIOMouseOverCarLoader2().GetFrontTireSize();
                if (CarDefaultWheel.Contains("|"))
                {
                    string[] WheelSizes = CarDefaultWheel.Split('|')[0].Split('R');
                    WheelData wheel = new WheelData
                    {
                        Size = Int32.Parse(WheelSizes[1])
                    };
                    item.WheelData = wheel;
                }
                else
                {
                    string[] WheelSizes = CarDefaultWheel.Split('R');
                    WheelData wheel = new WheelData
                    {
                        Size = Int32.Parse(WheelSizes[1])
                    };
                    item.WheelData = wheel;
                }
            }

            // Calculate tire data
            if (ItemId.Contains("tire_"))
            {
                string CarDefaultWheel = GameScript.Get().GetIOMouseOverCarLoader2().GetFrontTireSize();
                if (CarDefaultWheel.Contains("|"))
                {
                    string[] WheelSizes = CarDefaultWheel.Split('|')[0].Split('R');

                    WheelData wheel = new WheelData
                    {
                        Size = Int32.Parse(WheelSizes[1])
                    };
                    WheelSizes = CarDefaultWheel.Split('/');
                    wheel.Width = Int32.Parse(WheelSizes[0]);

                    string[] wheelsizes2 = CarDefaultWheel.Split('|');
                    wheelsizes2 = wheelsizes2[0].Split('R');
                    wheelsizes2 = wheelsizes2[0].Split('/');

                    wheel.Profile = Int32.Parse(wheelsizes2[1]);
                    item.WheelData = wheel;
                }
                else
                {
                    string[] wheelsizes = CarDefaultWheel.Split('R');
                    WheelData wheel = new WheelData
                    {
                        Size = Int32.Parse(wheelsizes[1])
                    };
                    wheelsizes = CarDefaultWheel.Split('/');
                    wheel.Width = Int32.Parse(wheelsizes[0]);
                    string[] wheelsizes2 = wheelsizes[1].Split('R');
                    wheel.Profile = Int32.Parse(wheelsizes2[0]);
                    item.WheelData = wheel;
                }
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
                        if (buyTuning && TuningPartExists(additionalPartId) && !additionalPartId.StartsWith("t_"))
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
    }
}
