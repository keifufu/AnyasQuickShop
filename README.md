# Anya's QuickShop
A [Car Mechanic Simulator 2021](https://store.steampowered.com/app/1190000/Car_Mechanic_Simulator_2021/) Mod for [Melon Loader](https://melonwiki.xyz) which allows players to purchase parts with the press of a button.

## Installation
- Install Melon Loader (using [Melon Loader Automated Installer](https://melonwiki.xyz/#/?id=automated-installation))
- Download the .dll [here](https://github.com/keifufu/QuickShop/releases/latest)
- Move the .dll into your Mods directory (GameFolder/Mods/)
- (optional) Configure file created at (Mods/QuickShop.cfg)

## Keybinds
- **B** - Buy Part
- **N** - Buy Tuned Part
- **Ctrl + (Key)** - Buy Part x4
- **Shift + (Key)** - Buy Part x8
- **Space + (Key)** - Buy Part x16
- **Alt + B** - Reload Config
- **Alt + N** - Show ID of hovered part

## Config
| Name | Default | Description |
| --- | --- | --- |
| BuyKeyCode | B | [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html)  to buy a part |
| BuyTunedKeyCode | N | [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html) to buy a tuned part |
| PriceDiscount | 15 | Discounts the price by x% |
| BuyAdditionalParts | true | Whether to buy required parts (Ex: When buying a Piston, it buys Piston Rings) |
| BuyNormalPartIfTunedPartDoesntExist  | false | Self explanatory |
| DisableWarningMessage | false | Disables "No tuned part was found, buying normal part instead." Message for the setting above |
| AlwaysBuyTunedPart | false | Buys a tuned part if possible by using either Hotkey |
| LicensePlateType | Standard | Type of the License Plate (Ex: Alaska, Greece, etc.) |
| LicensePlateText | Quick123 | Text on the License Plate |
| RimET | 0 | ET on purchased Rims |
| PurchasePresetCtrl | 4 | How many parts to buy when holding Ctrl |
| PurchasePresetShift | 8 | How many parts to buy when holding Shift |
| PurchasePresetSpace | 16 | How many parts to buy when holding Space |
| ShowPartCost | true | Add the cost to the end of the popups |

## TODO
 - [x] Make purchasing body parts possible
 - [x] Add price calculation for rims and tires
 - [x] Purchase correct rim and tire size on cars with multiple wheel sizes
 - [x] Add Rim ET option to config
 - [x] Fix not being able to buy tuned brake disc
 - [x] Add modifier keys to purchase multiple items at once (Thanks to [DefilerOfFate](https://www.nexusmods.com/users/82238008) for the suggestion)
 - [x] Fix mod randomly stuck buying the wrong part
 - [x] Add cost of parts to the end of the popups

## QuickShop_parts.json
- Json file that holds information about which parts to buy if `BuyAdditionalParts` is enabled.  
- Ids can be figured out by using `ALT + N`
- If you're messing with this you probably know what you're doing anyway

## Credits
- Inspired by RIDI's [QuickShop](https://www.nexusmods.com/carmechanicsimulator2021/mods/15)
