using MelonLoader;
using UnityEngine;

/**
 * TODO: Make purchasing body parts possible.
 * We can get the name of the part (ex: 'front_window', 'hood'...) by using
 * `MelonLogger.Msg(GameScript.Get().GetIOMouseOverCarLoader().GetIDWithTuned());`
 */

[assembly: MelonInfo(typeof(QuickShop.QuickShop), "QuickShop", "0.1.0", "keifufu")]
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
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(_config.BuyKeyCode))
            {
                _config.Reload();
                UIManager.Get().ShowInfoWindow("[QuickShop] Config has been reloaded");
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(_config.BuyTunedKeyCode))
            {
                UIManager.Get().ShowInfoWindow(GameScript.Get().GetPartMouseOver().GetID() + "\n" + GameScript.Get().GetPartMouseOver().GetTunedID());
                return;
            }

            if (Input.GetKeyDown(_config.BuyKeyCode))
            {
                string ItemId = GameScript.Get().GetPartMouseOver()?.GetID();
                _inventoryHelper.BuyPart(ItemId, _config.AlwaysBuyTunedPart);
            }

            if (Input.GetKeyDown(_config.BuyTunedKeyCode)) {
                string ItemId = GameScript.Get().GetPartMouseOver()?.GetID();
                _inventoryHelper.BuyPart(ItemId, true);
            }
        }
    }
}