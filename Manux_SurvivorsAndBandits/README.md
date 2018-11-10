<!--Read this in github to have all the visuals and formatting: https://github.com/manux32/7dtdSdxMods/tree/master/Manux_SurvivorsAndBandits-->
# Bandits and Survivors

Bandits are your enemies and Survivors are your allies. As it is set right now, they will all spawn equally in each biome. There is more Survivor spawns than Bandits spawns. But you can change all of that by editing the **entitygroups.xml** and **spawning.xml** files of the mod.  
</br>
Theres is 4 Bandit clans, and 4 Survivors clans. All Bandit clans are allies and all Survivor clans are allies.  
Each clan has regular soldiers (male/female), a leader, and a Brute.  
I made warm weather and cold weather versions of each entity ([see images below](#survivor-adventurers)).  
The Brutes are actually much taller in-game than what they look like in the images. They are like giants.  
</br>
Regular soldiers can spawn with all types of guns (including rocket launcher), or with any melee weapon(including a chainsaw).   Leaders usually spawn with an AK-47, but some of them have other weapons.  
Brutes only use melee weapons. I made bigger versions of melee weapons with custom meshes, so they don't look like toothpicks in their giants hands.  
Rocket Launchers have been set to not damage blocks, only entities, because I was finding it annoying to have them make big holes everywhere, but you can totally change that if you want.  
</br>
I tried to make the Bandits not too tough. If you want it more hardcore, you can edit all the xmls to your liking.  
You can kill survivors for quick and dirty loot, but it's better to keep them alive to protect you from roaming bandits.  
Bandits give you better loot thand Survivors.  
</br>
I learned how to make all of these by playing with the [**Bad Company**](https://7daystodie.com/forums/showthread.php?52099-Bad-Company) bandits. **Special thanks to them!**

## Potential conflicts:
This mod adds **42** new lootcontainers in **loot.xml**. IDs **199-240**.  
Yes, it's a lot! This is because there is a different corpse block for each type of weapon that the entities can carry. And Survivors and Bandits also have different corpses because the loot is different (Bandits have better loot).  
I assigned them to the highest lootcontainer IDs possible in order to try to avoid conflicts with other mods.  
Sadly, the maximum loot ID number for lootcontainers is 254, so I could not really use higher numbers than the ones I chose. And I left a bit of room at the end for future additions.  

If you are using mods that already use those lootcontainers IDs, you will need to change them either in this mod or in the other mods. You need to change both the ID of the lootcontainer in **loot.xml**, but also the ID on the block that points to it in **blocks.xml** (the value of the property **LootList** of the block).

## Entities specs:
| **```Max Health```** | **```Approach Speed```** | **```Max View Angle```** | **```Sight Range```** |
|:---:|:---:|:---:|:---:|
| **Ranged Soldier** = 150 </br> **Melee Soldier** = 350 </br> **Leader** = 1750 </br> **Brute** = 3000 | **Ranged Soldier** = 1 </br> **Melee Soldier** = 1.1 </br> **Leader** = 1 </br> **Brute** = 0.7 | **Ranged Soldier** = 300 </br> **Melee Soldier** = 300 </br> **Leader** = 360 </br> **Brute** = 300 | **Bandits** = 80 </br> **Survivors** = 120 |

## Survivor Adventurers:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerMaleWarm.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerFemaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerBrutewarm.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (Spiked Club) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerMaleCold.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerFemaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/AdventurerBruteCold.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (Spiked Club) |

## Bandit Punks:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkMaleWarm.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkFemaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkBruteWarm.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (Chainsaw) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkMaleCold.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkFemaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/PunkBruteCold.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (Chainsaw) |

## Survivor Hillbillies:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyMaleWarm.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyFemaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyBruteWarm.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader (Rocket Launcher) | Brute (Axe) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyMaleCold.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyFemaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/HillbillyBruteCold.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader (Rocket Launcher) | Brute (Axe) |

## Bandit Nazis:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziMaleWarm.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziFemaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziBruteWarm.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader (Rocket Launcher) | Brute (Spiked Club) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziMaleCold.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziFemaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/NaziBruteCold.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader (Rocket Launcher) | Brute (Spiked Club) |

## Survivor Rastas:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaMaleWarm.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaFemaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaBruteWarm.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (SledgeHammer) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaMaleCold.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaFemaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RastaBruteCold.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (SledgeHammer) |

## Bandit Barbarians:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianMaleWarm.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianFemaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianBruteWarm.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (SledgeHammer) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianMaleCold.jpg) | ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianFemaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/BarbarianBruteCold.jpg) |
|:---:|:---:|:---:|:---:|
| Male | Female | Leader | Brute (SledgeHammer) |

## Survivor Rebel Teen Girls:
### warm weather
| ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RebelTeenGirlWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RebelTeensLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RebelTeensBruteWarm.jpg) |
|:---:|:---:|:---:|
| Female only | Female Leader | Female Brute (Axe) |
### cold weather
| ![female](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RebelTeenGirlCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RebelTeensLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/RebelTeensBruteCold.jpg) |
|:---:|:---:|:---:|
| Female only | Female Leader | Female Brute (Axe) |

## Bandit Clowns:
### warm weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/ClownMaleWarm.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/ClownLeaderWarm.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/ClownBruteWarm.jpg) |
|:---:|:---:|:---:|
| Male only | Leader | Brute (Chainsaw) |
### cold weather
| ![male](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/ClownMaleCold.jpg) | ![leader](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/ClownLeaderCold.jpg) | ![brute](https://manux32.github.io/7dtd_SurvivorsAndBanditsModImages/ClownBruteCold.jpg) |
|:---:|:---:|:---:|
| Male only | Leader | Brute (Chainsaw) |




