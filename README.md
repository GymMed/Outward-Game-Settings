<h1 align="center">
    Outward Game Settings
</h1>
<br/>
<div align="center">
  <img src="https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/main/preview/images/0.png" alt="Outward game setting to require enchantment recipe when enchanting."/>
</div>

<div align="center">
	<a href="https://thunderstore.io/c/outward/p/GymMed/Game_Settings/">
		<img src="https://img.shields.io/thunderstore/dt/GymMed/Game_Settings" alt="Thunderstore Downloads">
	</a>
	<a href="https://github.com/GymMed/Outward-Game-Settings/releases/latest">
		<img src="https://img.shields.io/thunderstore/v/GymMed/Game_Settings" alt="Thunderstore Version">
	</a>
	<a href="https://github.com/GymMed/Outward-Mods-Communicator/releases/latest">
		<img src="https://img.shields.io/badge/Mods_Communicator-v1.2.0-D4BD00" alt="Min Mods Communicator Version">
	</a>
</div>

This Outward mod allows you to change the way game works.

## Available Settings

<details>
    <summary>Enchanting</summary>

<details>
    <summary>Require Enchantment Recipe To Enchant</summary>

_Attempting to enchant an item without the required enchantment recipe in your inventory (pocket or backpack) will display an error and cancel the process. Config setting: `RequireRecipeToAllowEnchant`._<br>
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/1.png)

</details>

<details>
    <summary>Consume Enchantment on Use</summary>

_Successfully enchanting an item consumes the enchantment recipe from your inventory. Recommended to use together with `Enchanting Requires Enchantment` setting. Config setting: `UseRecipeOnEnchanting`_<br>
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/2.png)

</details>

<details>
    <summary>Enchanting Success Chance</summary>

_Enchanting an item can fail based on a configurable success rate. Config setting: `EnchantingSuccessChance`_<br>
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/3.png)

</details>

<details>
    <summary>Play Audio on Enchanting Completion</summary>

_Plays custom sound effects when the EnchantmentTable.DoneEnchanting event occurs. Success and failure each trigger different audio clips. Enabled with config setting: `PlayAudioOnEnchantingDone`_<br>
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/4.png)

</details>
</details>

<details>
    <summary>Skills</summary>

<details>
    <summary>Chance to learn skills on enemy kill</summary>

When enemy dies by you, you have a chance to learn weapon type skill.<br>
If all weapon type skills a learned you will be able to learn other weapon types skills.<br>
Finally if all weapon type based skills a learned you can learn monster skills that could be broken.<br>

<div align="center">
	<video src="https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/videos/SkillReceived.mp4" width="640" controls></video>
</div>

</details>

</details>

<details>
    <summary>Enemy Scenarios</summary>
In game there are new events that happen randomly on world time change.<br><br>

Currently enemies don't drop loot. Loot can be added by other mods with 
<a href="https://thunderstore.io/c/outward/p/GymMed/Loot_Manager/">Loot Manager</a>.
Enemies can indeed cast spells and early on it is better to avoid them.

<details>
    <summary>War Scenario</summary>

Spawns random faction groups that start fighting. Only appears in open world.
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/war.png)

</details>

<details>
    <summary>Ambush Scenario</summary>

Spawns random faction group. The only scenario that can happen in towns but only from 20:00 to 04:00.
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/ambush.png)

</details>

<details>
    <summary>Wanderer Encounter Scenario</summary>

Spawns faction enemy.
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/wanderer.png)

</details>

<details>
    <summary>Enemy Factions</summary>

There are total of 4 factions that can spawn and fight. Troglodytes, Bandits,
Ghosts and Skeletons. They can use skills.

</details>

</details>

<details>
    <summary>Seasons</summary>

Additionally 3 seasons are added.

<details>
    <summary>Winter</summary>
    
Adds winter to other regions.

![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/winter-hallowed-marsh.png)
</details>

<details>
    <summary>Foggy spirits</summary>

Cloudy mist season that makes very hard to navigate and each hour changes
the fog density.

![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/foggy-spirits.png)
</details>

<details>
    <summary>Great war</summary>

Each hour there is a chance you will encounter additional war/ambush/wanderer scenario or nothing.
![Picture](https://raw.githubusercontent.com/GymMed/Outward-Game-Settings/refs/heads/complex/preview/images/great-war.png)
</details>

</details>

<details>
    <summary>Sideloader</summary>

I did add additional character AI classes. They work more as an example instead
of actually using them in your mods.
</details>

## How to change settings?

Currently all settings can be changed in `BepInEx\config\gymmed.outward_game_settings.cfg`. If you are mod pack creator you can use [outward mods communicator](https://github.com/GymMed/Outward-Mods-Communicator) to change values and rebalance gameplay.

## How to set up

To manually set up, do the following

1. Create the directory: `Outward\BepInEx\plugins\OutwardGameSettings\`.
2. Extract the archive into any directory(recommend empty).
3. Move the contents of the plugins\ directory from the archive into the `BepInEx\plugins\OutwardGameSettings\` directory you created.
4. It should look like `Outward\BepInEx\plugins\OutwardGameSettings\OutwardGameSettings.dll`
   Launch the game.

### If you liked the mod leave a star on [GitHub](https://github.com/GymMed/Outward-Game-Settings) it's free
