<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_WorkingOvenAndSink-->
# Working Oven And Sink
A modified/improved version of [**Valmar**](https://7daystodie.com/forums/showthread.php?32219-Valmar-s-Mod-Collection)'s working oven and sink.  

The main difference with Valmar's original features is that the workstations can be picked up to be placed somewhere else.  
Buckets can also be filled using the sink.  

If you don't know these Valmar features already, here's a quick description:  

||||
|:---:|:---:|:---:|
| ![img](Icons/ovenKit.png) | **Oven Repair Kit** | This kit can be used to upgrade broken ovens into new working variants with a wrench. |
| ![img](Icons/plumbingKit.png) | **Plumbing Kit** | This kit when used with a wrench can upgrade broken sinks into working variants for a fresh source of water. |
| ![img](https://manux32.github.io/7dtd_miscImages/oven.png) | **Working Ovens** | Can be used to cook recipes without using any fuel. <br/> Both regular ovens and wall ovens can be upgraded. |
| ![img](https://manux32.github.io/7dtd_miscImages/sink.png) | **Working Sinks** | Can be used as a source of clean water. Fill your jars with water by left-clicking with them. <br/> Both regular sinks and granite sinks can be upgraded. |  

### Progression
Requirements to be able to craft the items/blocks:
- **Oven Repair Kit**: craftSkillTools **2** / The Fixer **2**
- **Plumbing Kit**: craftSkillTools **3** / The Fixer **3**
- **Working Ovens**: craftSkillTools **5** / The Fixer **3**
- **Working Sinks**: craftSkillTools **6** / The Fixer **4**

### Potential conflicts
If you have other mods that modify the **Allowed_upgrade_items** value for the **wrench** item, there could be a small conflict.  
This mod sets that value to this: *```"wood,clayLump,dirtFragment,snowBall,scrapIron,forgedIron,forgedSteel,steelPolish,concreteMix,cobblestones,yuccaFibers,gasCan,plumbingKit,ovenKit"```*.  
It includes the items present in vanilla version, with the addition of *```gasCan,plumbingKit,ovenKit```*.  

You may need to modify that line in **WorkingOvenAndSink.xml** so that it also includes the items from your other mods.  
Or, depending of the order in which SDX deploys the mods, you may need to modify that line in the other mods to include: *```gasCan,plumbingKit,ovenKit```*. Or put everything in both to be safe. :smiley:  

The reason ```gascan``` is there is because my [Miner Robot](Manux_MinerRobot) mod needs it, and it gets deployed before this one. I included it here, so that it is still there when you deploy both mods.  
Mods deploy in alphabetical order of the mods root folders from what I understand.  
Sadly, SDX is not yet capable of inserting additional items in a multi-item string, it can only set the whole value to a new value.
