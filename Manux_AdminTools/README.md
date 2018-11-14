<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_AdminTools-->
# <a href="#"><img src="https://manux32.github.io/7dtd_miscImages/repair_icon.png" width="10%" height="10%" align="left"></a>Admin/Creative Tools<br/>  
A couple of simple useful Admin/Creative Tools.  
Some of them are pieces I took from the [**Bad Company**](https://7daystodie.com/forums/showthread.php?52099-Bad-Company) mod and adapted.  

- **Black Pill**: Cures pretty much everything, and fills Health, Stamina, Wellness, Food and Water.
- **Super Digger**: The already existing commented Super Digger.
- **Grass Bomb**: A throwable that gets rid of grass and plants quickly.
- **Terrain Cleaner Bomb**: A throwable that cleans a terrain quickly (grass, plants, wood, rocks).
- **Get skill points**: 4 "fake" quests to give yourself skill points:
	- adminskill5
	- adminskill15
	- adminskill25
	- adminskill50  

## New Console Commands
You need **StompyNZ**'s [**ConsoleCmd**](https://github.com/7D2DSDX/Mods/tree/master/ConsoleCmd) mod for these new commands to work.

- **getentityinfos**: lists infos on en entity. first param is the id of the entity:
	- Command: *getentityinfos* or *gei*

- **getentityprefabinfos**: lists infos on en entity prefab. first param is the id of the entity: 
	- Command: *getentityprefabinfos* or *gepi*

- **listspawnableentities**: list all spawnable entities, with an optional "contains(string)" parameter:
	- Command: *listspawnableentities* or *lse*
	- *lse* : lists all spawnable entities.
	- *lse yoursearchstring* : to list all spawnable entities that contain the specified string.
- **setentitystat**: Sets an Entity Stat:
	- Command: *setentitystat* or *ses*  
	- *ses 'entity id' 'entity stat name' 'new entity stat value'*  
	- Supported Entity Stats:  
		- health
		- stamina
		- sickness
		- gassiness
		- speedmodifier
		- wellness
		- coretemp
		- food
		- water
		- waterlevel

	
