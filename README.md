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
- **CTRL + B** - Reload Config
- **CTRL + N** - Show ID of hovered part

## Config
| Name | Default | Description |
| --- | --- | --- |
| BuyKeyCode | B | [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html)  to buy a part |
| BuyTunedKeyCode | N | [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html) to buy a tuned part |
| PriceDiscount | 15 | Discounts the price by x% |
| BuyAdditionalParts | true | Whether to buy required parts (Ex: When buying a Piston, it buys Piston Rings) |
| BuyNormalPartIfTunedPartDoesntExist  | false | Self explanatory |
| DisableWarningMessage | false | Disables "No tuned part was found, buying normal part instead." Message for the setting above |
| AlwaysBuyTunedPart  | false | Buys a tuned part if possible by using either Hotkey |

## TODO
 - [ ] Make purchasing body parts possible

## QuickShop_parts.json:
Json file that holds information about which parts to buy if `BuyAdditionalParts` is enabled.  
Id's can be figured out by using `CTRL + N`

## Credits
- Inspired by RIDI's [QuickShop](https://www.nexusmods.com/carmechanicsimulator2021/mods/15)
