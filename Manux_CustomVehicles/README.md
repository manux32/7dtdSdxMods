<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_HPMiniBike-->

# Custom Vehicles 
| ![img](Icons/roadHogChassis.png) | ![img](Icons/hellGoatBikeChassis.png) | ![img](Icons/quadChassis.png) | ![img](Icons/cicadaCarChassis.png) |
|:---:|:---:|:---:|:---:|  
| Dust2Death's Road Hog | Hell Goat Bike | Quad | Cicada Car |

The Custom Vehicles mod is there to help you build your own custom vehicles, from custom Bikes to custom Cars, Quads, Trucks.  
You can make all-terrain vehicles that can climb small and medium objects, and control the camera distance, and player position/orientation on the vehicle.  

It contains multiple different examples of already functionnal bikes and cars.  
I will eventually try to make tutorials or youtube vids to help you build your own, if there is enough demand for it.  

The mod can be SDX compiled for dedicated servers, the code is functionnal for multiplayer.
Some of the vehicles may still be a bit heavy performance wise, depending on your machine's performance. I reduced the polycount and texture size of all assets, but it may still require more reduction, if this becomes a thing, and people want tons of vehicles in multiplayer games. We will then potentially need to start optimizing vehicles a lot more...  

## Dependencies
This mod has dependencies on [**Hal's DLL Fixes**](https://github.com/7D2DSDX/Mods/tree/master/HalDllUpdates) mod.  
Make sure you also deploy that mod for this one to work. It shouldn't let you build if you don't have it or don't have it enabled.

## Vids
[![Fishing Mod demo](http://img.youtube.com/vi/jd1xWsgqwCg/0.jpg)](https://www.youtube.com/watch?v=jd1xWsgqwCg "Custom Car")  
[![Fishing Mod demo](http://img.youtube.com/vi/au5lZz8cKmQ/0.jpg)](https://www.youtube.com/watch?v=au5lZz8cKmQ "Custom All Terrain Vehicles")  

## Special Thanks
### Dust2Death: 
For doing all the ground work to find how to make a custom bike using the game unused Road Hog asset.
See his Road Hog forum thread here: [**Road Hog**](https://7daystodie.com/forums/showthread.php?87828-Road-Hog-SDX)  
### TormentedEmu:
For making the Horse vehicle of the Medieval Mod, which has been a great code reference for me when building this mod. And also for helping demistify some of the more obscur parts of coding something like this.
And thanks also for her great [**MinibikeImpact**](https://github.com/TormentedEmu/7DTD-SDX-Mods/tree/master/MinibikeImpact) mod that I am using in the first video above to run over zombies with the Cicada car.
### [chervinin](https://sketchfab.com/chervinin)
A great artist who did the amazing [Sketchfab **Hell Motogoat** asset](https://skfb.ly/TCEV) that I am using for the Hell Goat Bike vehicle.  
His **Hell Motogoat asset** is under [Creative Commons Attribution 4.0](https://creativecommons.org/licenses/by/4.0/) licensing terms.  
I made the following changes to the Asset in order to be able to integrate it in the 7 days to die game:
- polygon reduction on all meshes.
- textures reduction from 2048x2048 to 512x512
- combine the textures maps to be compatible with the Unity Shaders (Glossiness maps as the alpha channel of Specular maps, Opacity maps as the alpha channel of Diffuse maps)
- skinning of the meshes on a simple bone structure to be able to animate it in the game.
- Addition of lightbulbs in the eyes of the Goat skull, for when the lights of the vehicle are turned on. Looks awesome!
### JaxTeller718: 
For doing great sounds for Dust2Death's Road Hog. I am also using those sounds on the Hell Goat Bike.
### [Retro Valorem](https://assetstore.unity.com/publishers/22495)
For his great, cute, and free Unity Asset Store [**Cicada** cartoon car](https://assetstore.unity.com/packages/3d/vehicles/land/retro-cartoon-cars-cicada-96158).  
This asset has been used to make my fist custom car vehicle. A bit too cartoony for the style of the game, but I kept it because I like it a lot, it's like the minibike of cars. And the player looks so funny when driving it.

### [Mark Bai](https://sketchfab.com/bcfbox)
For his very nice and cheap Sketchfab [**ATV model practice**](https://skfb.ly/6x9oT) asset, that I use for the Quad vehicle.

## Professional High-Powered Parts:
This mod also adds Professional High-Powered versions of the minibike parts, in order to build High-Powered Vehicles.  
I wanted a faster bike especially for Random Gen maps where cities are pretty far from each other.  
Only the Hell Goat bike currently uses them, but based on that example, you can use them for other vehicles and make new versions of some of the parts for different types of vehicles.
I did not yet add any progression gates for being able to craft these items.  

### ![bigEngine](Icons/bigEngine.png) Big Engine
A big, robust, and powerful engine for your bigger vehicles. It consumes a bit more gas but features a bigger gas tank. You can accellerate faster and reach very high speeds, but be careful, it also jumps way higher!
### ![robustBikeChassis](Icons/robustBikeChassis.png) Robust Bike Chassis (Power suspension)
A robust bike chassis equipped with power suspension. You almost don't get hurt when jumping too high. 
### ![proBikeHandlebars](Icons/proBikeHandlebars.png) Professional Bike Handlebars
Have better control of your driving with these professional bike handlebars. They will also degrade slower than regular ones.  
### ![proBikeWheels](Icons/proBikeWheels.png) High-Performance Bike Wheels
Have better traction and reduced drag with these beautiful High Performance tires. They will also degrade slower than regular ones.  
### ![proBikeSeat](Icons/proBikeSeat.png) Professional Bike Seat
A beautiful deluxe comfy seat that drains less stamina when the bike is damaged. The seat is also more robust and will degrade slower.
