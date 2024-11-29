Scripts of 'Charge' project(Halted)

Unity version: 2022.3.20f1

Only scripts included.

2024.06.24 ~ 2024.8.10

Developed by individual.

Youtube Links:

[Map generation and Loading Next stage](https://youtu.be/upcm6MUBsDw)

[Playing Longsword and Dagger](https://youtu.be/Z2ogFtdLKyU)

[Playing Sword and Staff](https://youtu.be/-WOeSFszERE)

GameManager.cs
 - Game Manager instance(Singleton)

PoolManager.cs
 - Manage Enemy Object Pooling

Spawner.cs
 - Spawn enemy

TestWeaponChanger.cs
 - Test script for weapon changing by trigger

WeaponManager.cs
 - Manage Weapon UI

MapGenerator.cs
 - Generate random map


Characters/Player
 - Scripts for Player object action

Characters/Weapon
 - Scripts for Weapon object action

Characters/Enemy
 - Scripts for Enemy object action


Implemented Features

1. Player actions
 - 'wasd' to move.
 - 'Left click' to attack with main weapon.
 - 'Right click' to charge attack with main weapon. Require 100 stamina(base).
 - 'Shift + Left click' to attack with sub weapon.
 - 'Shift + Right click' to charge attack with sub weapon. Require 100 stamina(base).
 - 'Space' to dodge. Require 15 stamina(base).

2. Actions for Weapon
Weapons are objects, and be set active when attack or charge called.

2-1. Longsword
 - Attack: Swing from counter-clockwise 120 degree of mouse direction. Swing 180 degree.
 - Charge: Charge to mouse direction. Enemies are knockbacked to side.

2-2. Dagger
 - Attack: Increase move speed, stamina regeneration. Set player untrackable to make enemies can't update player's position(stealth).
 - Charge: Charge to mouse direction. If collision occured, step back.

2-3. Sword
 - Attack: Retain input for power charging. When input released, shoot sword aura(black triangle for test).
 - Charge: Retain input for power charging. When input released, Charge to mouse direction.

2-4. Staff
 - Attack: Shoot bullet.
 - Charge: Make area of effect that slows and damage enemies. After delay, teleport to mouse location or mouse direction with max teleport distance.

3. Enemy
 - Enemy tracks player's position and move to the position.
 - If player is not trackable, move to last updated position.
 - while staying in collision with player that can be damaged, damage player.

4. Map Generation
 - If stage loaded, Map Generator instantiates random number of rooms from pre-made prefabs, including 'start room', 'shop room', 'boss room' for sure. 'Start room' is on (0,0,0) for sure.
 - Instantiated rooms are randomly spread in a small ellipse overlapped.
 - Each rooms spread wide by pre-defined collider for each.
 - When every rooms stop moving, use delaunay triangulation to find closest rooms for each room, and find minimum spanning tree using kruskal algorithm.
 - Place tilemaps to connect rooms using datas of edges.

5. Room
 - Player character starts at (0,0,0), which is the center of 'start room'.
 - When PC enters room which is not 'start room', 'shop room', and room already cleared, create door object to block every exit from room, and start enemy waves.(Number and type of waves are random and enemy list for each wave can be pre-defined in scriptableObject.)
 - Enemies are summoned after warning sign disappears.
 - Enemies are summoned in order, and next wave starts only after every enemies in wave are dead.
 - After all the waves end, door object disappears.
 - If cleared boss room, gate to next stage enabled in boss room.

Test Scene
 - In test scene, there are 4 circles with different colors for weapon earning. From the most left one, set your weapon to Longsword, Dagger, Sword, Staff.
 - If one of your weapon is set by these circles, notification message will be pop up for 3 seconds.
 - If you have no weapon, main weapon will be set.
 - If you already have main weapon, sub weapon will be set.
 - If you already have two weapons, nothing happens.

Known bugs
- Spreading tilemaps doesn't work properly somtimes when new scene is loaded.
- Some rooms don't get connected if its shape is too far from rectangle.
