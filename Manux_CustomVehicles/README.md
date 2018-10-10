<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_HPMiniBike-->

# Custom Vehicles 
| ![img](Icons/roadHogChassis.png) | ![img](Icons/hellGoatBikeChassis.png) | ![img](Icons/quadChassis.png) | ![img](Icons/cicadaCarChassis.png) | ![img](Icons/loaderChassis.png) |
|:---:|:---:|:---:|:---:|:---:|  
| Dust2Death's Road Hog | Hell Goat Bike | Quad | Cicada Car | Killer Loader |

The Custom Vehicles mod is there to help you build your own custom vehicles, from custom Bikes to custom Cars, Quads, Trucks.  
You can make all-terrain vehicles that can climb small and medium objects, and control the camera distance, and player position/orientation on the vehicle.  

It contains multiple different examples of already functionnal bikes and cars/trucks.  
It also contains examples of custom vehicle parts. Check-out the [**Custom Vehicle Parts section**](#custom-vehicle-parts).  
I will eventually try to make tutorials or youtube vids to help you build your own, if there is enough demand for it.  

I you want to build your own, I recommend starting with something simple like a custom bike. The Car, Quad and Loader are more complex on the rigging side.  
I can provide Unity template scenes for each vehicle, just ask me on the [**forum thread**](https://7daystodie.com/forums/showthread.php?87828-Custom-Vehicles-by-Manux-SDX).

The mod can be SDX compiled for dedicated servers, the code is functionnal for multiplayer.
Some of the vehicles may still be a bit heavy performance wise, depending on your machine's performance. I reduced the polycount and texture size of all assets, but it may still require more reduction, if this becomes a thing, and people want tons of vehicles in multiplayer games. We will then potentially need to start optimizing vehicles a lot more...  

Special Thanks to **DUST2DEATH** for making the ball roll on this one.  
See the complete ***Special Thanks*** section [**here**](#special-thanks). 

## Dependencies
This mod has dependencies on [**Hal's DLL Fixes**](https://github.com/7D2DSDX/Mods/tree/master/HalDllUpdates) mod.  
Make sure you also deploy that mod for this one to work.  
It shouldn't let you build if you don't have it or don't have it enabled.  


## Vids
| [![driveable cars](http://img.youtube.com/vi/jd1xWsgqwCg/0.jpg)](https://www.youtube.com/watch?v=jd1xWsgqwCg "Custom Driveable Cars") | [![all terrain vehicles](http://img.youtube.com/vi/au5lZz8cKmQ/0.jpg)](https://www.youtube.com/watch?v=au5lZz8cKmQ "All-Terrain Vehicles") | [![LoaderA](http://img.youtube.com/vi/MXkOzT_1-nM/0.jpg)](https://youtu.be/MXkOzT_1-nM "Custom Loader that destroys everything on its path") | [![LoaderB](http://img.youtube.com/vi/OehnLqXRZIU/0.jpg)](https://youtu.be/OehnLqXRZIU "Custom Loader vehicle: a horde killer") |
|:---:|:---:|:---:|:---:|
|Custom Driveable Cars|All-Terrain Vehicles|Custom Loader that destroys everything on its path|Custom Loader vehicle: a horde killer|  

## Custom Vehicle Parts
You don't need this mod to make your own custom parts for vehicles, it can all be done through xml.  
But I have examples in here of custom parts for all the slot types of a vehicle.  
Each vehicle always has a custom chassis item for items.xml, to be able to spawn that specific type of vehicle.  
I made the icons show the whole vehicle so you know what you are spawning:  

| ![img](Icons/roadHogChassis.png) | ![img](Icons/hellGoatBikeChassis.png) | ![img](Icons/quadChassis.png) | ![img](Icons/cicadaCarChassis.png) | ![img](Icons/loaderChassis.png) |
|:---:|:---:|:---:|:---:|:---:|  

### Professional High-Powered Bike Parts
Professional High-Powered versions of the minibike parts, in order to build High-Powered Bikes or other Vehicles.  
I intitially wanted a faster bike especially for Random Gen maps where cities are pretty far from each other., so I did these before knowing how to make fully custom vehicles. I was just using them to have a HP minibike.  

Only the Hell Goat bike currently uses them. The new Loader also has a couple of custom parts, it's engine xml class extends from the bigEngine class below.
And based on that example, you can use them for other vehicles and make new versions of some of the parts for different types of vehicles.
I did not yet add any progression gates for being able to craft these items.  

#### ![bigEngine](Icons/bigEngine.png) Big Engine
A big, robust, and powerful engine for your bigger vehicles. It consumes a bit more gas but features a bigger gas tank. You can accellerate faster and reach very high speeds, but be careful, it also jumps way higher!
#### ![robustBikeChassis](Icons/robustBikeChassis.png) Robust Bike Chassis (Power suspension)
A robust bike chassis equipped with power suspension. You almost don't get hurt when jumping too high. 
#### ![proBikeHandlebars](Icons/proBikeHandlebars.png) Professional Bike Handlebars
Have better control of your driving with these professional bike handlebars. They will also degrade slower than regular ones.  
#### ![proBikeWheels](Icons/proBikeWheels.png) High-Performance Bike Wheels
Have better traction and reduced drag with these beautiful High Performance tires. They will also degrade slower than regular ones.  
#### ![proBikeSeat](Icons/proBikeSeat.png) Professional Bike Seat
A beautiful deluxe comfy seat that drains less stamina when the bike is damaged. The seat is also more robust and will degrade slower.

### Loader Parts
#### ![loaderHandlebars](Icons/loaderHandlebars.png) Loader Handlebars
I did this one to be able to have the Loader steering wheel turn much slower than Bikes.  
#### ![loaderEngine](Icons/loaderEngine.png) Loader engine
I did this one mainly to control the speed at which the loader moves, the gas consumption, the size of the gas tank, but also for things like the DegradationMax and VehicleNoise.  

## Special Thanks

### Dust2Death: 
For doing all the ground work to find how to make a custom bike using the game unused Road Hog asset.
See his Road Hog forum thread here: [**Road Hog**](https://7daystodie.com/forums/showthread.php?87828-Road-Hog-SDX)  

### TormentedEmu:
For making the Horse vehicle of the Medieval Mod, which has been a great code reference for me when building this mod. And also for helping demistify some of the more obscur parts of coding something like this.
And thanks also for her great [**MinibikeImpact**](https://github.com/TormentedEmu/7DTD-SDX-Mods/tree/master/MinibikeImpact) mod that I am using in the first video above to run over zombies with the Cicada car.

### [chervinin](https://sketchfab.com/chervinin)
A great artist who did the amazing [Sketchfab **Hell Motogoat** asset](https://skfb.ly/TCEV) that I am using for the Hell Goat Bike vehicle.  
His **Hell Motogoat asset** is free but is under [**Creative Commons Attribution 4.0**](https://creativecommons.org/licenses/by/4.0/) **licensing terms**.  
***Make sure to read these terms before using this asset for other purposes...***  

I made the changes listed below to the Asset in order to be able to integrate it in this mod for the 7 days to die game.  
This mod is public and free, the derived asset is not used in any commercial way.  
- polygon reduction on all meshes.
- textures reduction from 2048x2048 to 512x512
- combine the textures maps to be compatible with the Unity Shaders (Glossiness maps as the alpha channel of Specular maps, Opacity maps as the alpha channel of Diffuse maps)
- skinning of the meshes on a simple bone structure to be able to animate it in the game.
- Addition of lightbulbs in the eyes of the Goat skull, for when the lights of the vehicle are turned on. Looks awesome!

### JaxTeller718  
For doing great sounds for Dust2Death's Road Hog. I am also using those sounds on the Hell Goat Bike and the Loader.

### [Renafox](https://sketchfab.com/kryik1023)
A very talented artist that created the [**Wheel Loader**](https://skfb.ly/6toGS) asset I used for making the Killer Loader.  
Check-out all the other [great models](https://sketchfab.com/kryik1023) she has for sale.

### [Retro Valorem](https://assetstore.unity.com/publishers/22495)
For his great, cute, and free Unity Asset Store [**Cicada** cartoon car](https://assetstore.unity.com/packages/3d/vehicles/land/retro-cartoon-cars-cicada-96158).  
This asset has been used to make my fist custom car vehicle. A bit too cartoony for the style of the game, but I kept it because I like it a lot, it's like the minibike of cars. And the player looks so funny when driving it.

### [Mark Bai](https://sketchfab.com/bcfbox)
For his very nice and cheap Sketchfab [**ATV model practice**](https://skfb.ly/6x9oT) asset, that I use for the Quad vehicle.

