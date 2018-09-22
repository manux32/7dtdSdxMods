# Manux_BiggerBackPackMiniBikeContainersCraftingSlots

### This mod adds more slots for the following: 
- Backpack: 120 slots 
- Minibike storage: 180 slots
- Player crafted containers: 140 slots
- Crafting input/output slots: 5 crafting, 9 outputs
- A spawn entity window that can fit 792 entities. But the 2 bottom rows could get occluded by the toolbelt, so it's a bit less than that.
  - **Warning**: To achieve this I had to deactivate the outline effect on the text of all buttons, otherwise it can't fit so many without console errors. Text might be a little bit harder to read on buttons, but I didn't notice much of a difference.


The mod is in majority **stedman420**'s Simple UI Plus 120 mod packaged as an SDX mod, for easy merging with other SDX mods.  
It does not include his UI HUD, I packaged that in the mod [Manux_SimpleUI_HUD](../Manux_SimpleUI_HUD).

The backpack extra slots need a C# SDX PatchScript. I am using **sphereii**'s BiggerBackpack mod's c# script for that part, since I don't have stedman420's orignal patching code.

The mod also includes a combine feature on the minibike window. Thanks to **Anabella**'s ExpandedMinibike mod for the original code. I modified it and made it horizontal to fit with stedman420's minibike UI.

### Bug Fixes:
I fixed a couple of issues that were present in the Simple UI Plus 120 mod.
- The spawn entity window was also larger than the vanilla one, but the mod was not disabling the text outline effect, so it was generating errors in the console and no text was displayed on the spawning buttons.
  - Thanks to **Dust2Death** for showing me the solution to this problem.
- The map window had an issue where all waypoints tooltips were completely off from their waypoint icons on the map, so it was incredibly hard to find where to put your mouse to display the name of the waypoint. I fixed this by offsetting the map window until it aligned. It's not 100% perfect, but almost. If it feels like they are offset, just zoom out and in again and it should fix it.




