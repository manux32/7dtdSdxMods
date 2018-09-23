<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_MinerRobot-->
# Miner Robot ![miner](Icons/minerGeneric.png)

This was inspired by **YoShIWoZ**'s [Miner mod](https://7daystodie.com/forums/showthread.php?47379-Miner).  
A Miner Robot is a device that mines ressources for you.  
The device does not actually move arround to mine real blocks. It will just spawn a loot container on top of it when it's done "mining".

### Blocks:
- **Miner Robot (Generic Device)**: A non-Active Generic version of the Miner Robot. Convert it to a specific ore group version of the device to activate it.
- **Miner Robot (Regular Ores)**: Harvests regular Ores (rockSmall, dirtFragment, crushedSand, clayLump)
- **Miner Robot (Rare Ores)**: Harvests rare Ores (ironFragment, coal, oilShale, scrapLead, potassiumNitratePowder), and more rarely, very rare ores (silverNugget, goldNugget, rawDiamond)

### Progression
- You need craftSkillTools 6 and craftSkillScience 6 to craft the Generic Miner Robot.
- CraftSkillTools 4 and craftSkillScience 3 to convert a generic device to the Regular Ores version.
- CraftSkillTools 7 and craftSkillScience 7 to convert a generic device to the Rare Ores version.

### Usage:
- Buy, craft or find the generic device.
- Convert it to either the Regular Ores or the Rare Ores version using the appropriate workbench recipe.
- Place the device somewhere.
- Upgrade it with gas using a clawhammer or a wrench to start the Robot (Regular Ores: 1000 gas / Rare Ores: 1500 gas). 
- You will see it's lights turn on, the auger blade will start to turn and you will hear its noise.
- The mining time is currently arround 60 real-time minutes (random).
- A lootable chest will appear on top of it when its done. The device will also turn off. 

### Potential conflicts
If you have other mods that modify the **Allowed_upgrade_items** value for the **wrench** or the **clawHammer** item, there could be a small conflict.  
This mod sets that value to the following for both items: *```"wood,clayLump,dirtFragment,snowBall,scrapIron,forgedIron,forgedSteel,steelPolish,concreteMix,cobblestones,yuccaFibers,gasCan"```*.  
It includes the items present in vanilla version, with the addition of *```gasCan```*.  

You may need to modify that line in **MinerRobot.xml** so that it also includes the items from your other mods.  
Or, depending of the order in which SDX deploys the mods, you may need to modify that line in the other mods to include: *```gasCan```*. Or put everything in both to be safe. :smiley:  

Mods deploy in alphabetical order of the mods root folders from what I understand.  
Sadly, SDX is not yet capable of inserting additional items in a multi-item string, it can only set the whole value to a new value.
