using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(QuickShop.QuickShop), "QuickShop", "0.4.1", "keifufu")]
[assembly: MelonGame("Red Dot Games", "Car Mechanic Simulator 2021")]
namespace QuickShop
{
    public class QuickShop : MelonMod
    {
        private Config _config;
        private InventoryHelper _inventoryHelper;

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Initializing...");
            _config = new Config();
            _inventoryHelper = new InventoryHelper(_config);
        }

        public override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(_config.BuyKeyCode))
            {
                _config.Reload();
                UIManager.Get().ShowPopup(Config.ModName, "Reloaded Config", PopupType.Normal);
                return;
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                string PartID = GameScript.Get().GetPartMouseOver()?.GetID();
                string TunedID = GameScript.Get().GetPartMouseOver()?.GetTunedID();
                if ((PartID == "" || PartID == null) && (TunedID == "" || TunedID == null)) return; 
                UIManager.Get().ShowInfoWindow($"{PartID}\n{TunedID}");
                return;
            }

            if (Input.GetKeyDown(_config.BuyKeyCode) || Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                bool buyTuned = Input.GetKeyDown(_config.BuyTunedKeyCode) || _config.AlwaysBuyTunedPart;
                string ItemID = GameScript.Get().GetRaycastOnItemID();
                _inventoryHelper.BuyPart(ItemID, buyTuned, GetItemAmount());
            }
        }

        private int GetItemAmount()
        {
            if (Input.GetKey(KeyCode.LeftControl)) return _config.PurchasePresetCtrl;
            if (Input.GetKey(KeyCode.LeftShift)) return _config.PurchasePresetShift;
            if (Input.GetKey(KeyCode.Space)) return _config.PurchasePresetSpace;
            return 1;
        }
    }
}