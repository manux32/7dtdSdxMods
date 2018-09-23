<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_BasicFishing-->
# Basic Fishing
A simple Fishing mod, inspired by [**Clockwork Orange**](https://7daystodie.com/forums/showthread.php?68123-ACP-Fishing) and [**Carzilla**](https://7daystodie.com/forums/showthread.php?37132-Carlzilla-s-Fishing-Mod&highlight=fish+recipes)'s Fishing mods.  
I'm also using many icons from their mods.  
The mods were adapted to what I wanted, especially making it easier to bait the Fishing Rod, without using a recipe each time.  
Read the items in the localization file or in-game to know how to fish.  

## Items
| ![Fishing Rod](Icons/fishingRod.png) | ![Fishing Trap](Icons/fishingTrap.png) | ![Fish Bait](Icons/fishBait.png) | ![Fish bait baskets](Icons/fishBaitBasket.png) | ![Raw Fish](Icons/rawFish.png) |
|:---:|:---:|:---:|:---:|:---:| 
| Fishing Rod | Fishing Trap | Fish Bait | Fish bait baskets | Raw Fish |


## Fish recipes
| ![Grilled Fish](Icons/grilledFish.png) | ![Boiled Fish](Icons/boiledFish.png) | ![Charred Fish](Icons/charredFish.png) | ![Fish Stew](Icons/fishStew.png) | ![Fish And Chips](Icons/fishAndChips.png) |
|:---:|:---:|:---:|:---:|:---:| 
| Grilled Fish | Boiled Fish | Charred Fish | Fish Stew | Fish And Chips |

## Potential conflicts:
This mod adds 2 new lootcontainers in **loot.xml**, IDs **145-146**.  
If you are using mods that already use those lootcontainers IDs, you will need to change them either in this mod or in the other mods. You need to change both the ID of the lootcontainer in **loot.xml**, but also the ID on the block that points to it in **blocks.xml** (the value of the property **LootList** of the block).
