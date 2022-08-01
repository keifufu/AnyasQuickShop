using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(QuickShop.QuickShop), "QuickShop", "0.4.0", "keifufu")]
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
                UIManager.Get().ShowInfoWindow("[QuickShop] Config has been reloaded");
                return;
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                string PartId = GameScript.Get().GetPartMouseOver()?.GetID();
                string TunedId = GameScript.Get().GetPartMouseOver()?.GetTunedID();
                if ((PartId == "" || PartId == null) && (TunedId == "" || TunedId == null)) return; 
                UIManager.Get().ShowInfoWindow($"{PartId}\n{TunedId}");
                return;
            }

            if (Input.GetKeyDown(_config.BuyKeyCode) || Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                bool buyTuned = Input.GetKeyDown(_config.BuyTunedKeyCode) || _config.AlwaysBuyTunedPart;
                string PartId = GameScript.Get().GetPartMouseOver()?.GetID();
                string ItemId = (PartId == "" || PartId == null) ? GameScript.Get().GetRaycastOnItemID() : PartId;
                _inventoryHelper.BuyPart(ItemId, buyTuned, GetItemAmount());
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