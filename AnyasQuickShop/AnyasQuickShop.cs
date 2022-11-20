using MelonLoader;
using AnyasQuickShop;
using UnityEngine;

[assembly: MelonInfo(typeof(AnyasQuickShop.AnyasQuickShop), "AnyasQuickShop", "0.4.5", "keifufu")]
[assembly: MelonGame("Red Dot Games", "Car Mechanic Simulator 2021")]
namespace AnyasQuickShop
{
    public class AnyasQuickShop : MelonMod
    {
        private Config _config;
        private InventoryHelper _inventoryHelper;
        private Helpers _helper;
        private int unmountAllBodyPartsConfirmationTimer = 0;
        private bool unmountAllBodyPartsConfirmed = false;

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Initializing...");
            _config = new Config();
            _inventoryHelper = new InventoryHelper(_config);
            _helper = new Helpers();
        }

        public override void OnUpdate()
        {
            if (unmountAllBodyPartsConfirmationTimer > 0 || unmountAllBodyPartsConfirmed)
            {
                unmountAllBodyPartsConfirmationTimer--;
                if (unmountAllBodyPartsConfirmationTimer == 0 && unmountAllBodyPartsConfirmed)
                    unmountAllBodyPartsConfirmed = false;
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(_config.BuyKeyCode))
            {
                _config.Reload();
                UIManager.Get().ShowPopup(Config.ModName, "Reloaded Config", PopupType.Normal);
                return;
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                string ItemID = GameScript.Get().GetRaycastOnItemID();
                if (ItemID == "" || ItemID == null) return; 
                UIManager.Get().ShowInfoWindow(ItemID);
                return;
            }

            if (Input.GetKeyDown(_config.BuyKeyCode) || Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                bool buyTuned = Input.GetKeyDown(_config.BuyTunedKeyCode) || _config.AlwaysBuyTunedPart;
                string ItemID = GameScript.Get().GetRaycastOnItemID();
                _inventoryHelper.BuyPart(ItemID, buyTuned, GetItemAmount());
            }

            if (Input.GetKeyDown(_config.UnmountAllBodyPartsKeyCode))
            {
                if (unmountAllBodyPartsConfirmed)
                {
                    _helper.UnmountAllBodyParts(GameScript.Get().GetIOMouseOverCarLoader2());
                    UIManager.Get().ShowPopup(Config.ModName, "Unmounted all body parts", PopupType.Buy);
                }
                else
                {
                    unmountAllBodyPartsConfirmationTimer = 100;
                    unmountAllBodyPartsConfirmed = true;
                    UIManager.Get().ShowPopup(Config.ModName, "Press again to unmount all body parts", PopupType.Buy);
                }
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