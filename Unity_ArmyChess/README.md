Scripts of 'ArmyChess' project(WIP)

Unity version: 2022.3.20f1

Only scripts included.

2024.09.04 ~ 2024.9.14 / 2024.9.26 ~ WIP

[Playing movie](https://youtu.be/c_92I434w34)

GameManager.cs
 - Game Manager instance(Singleton).
PlayerControlHandler.cs
 - Get user input(click).

Board.cs
 - Create chess board and manage squares.
GetMovement.cs
 - Compute possible moves and attacking square for each piece.
PiecePlacer.cs
 - Manage object pooling for pieces.
PieceManager.cs
 - Manage Piece access by 2d List.

CombatManager.cs
 - Manage turn-based battle between pieces.
SoldierLocation.cs
 - Manage soldier-placeable locations and activate object.

RewardManager.cs
 - Manage Reward and UI after stage.
SkillManager.cs
 - Manage Skill level and UI after reward.

Square.cs
 - Manage informations of square.
Piece.cs
 - Manage informations of piece.

Soldier.cs
 - Manage soldier status.(not monobehaviour)
Skill.cs
 - Stores informations of skill.(Scriptable Object)
Reward.cs
 - Stores informations of Stage Clear Reward.(Scriptable Object)


No menu implemented yet.

In test play, Player plays both side.

Implemented Features

1. As start game, Create board and pieces.
2. Get user input. If user clicks square which has own piece, Squares that the piece can move to highlighted with green square.
2-1. If user clicks highlighted square, move selected piece to square. If enemy piece already exists on square, Initiate turn-based battle.
2-2. If user clicks else, unhighlight square and wait for selecting piece again.
3. If turn-based battle(combat) initiated, activate camera for combat. 
4. Get user input. If user clicks piece, skill UI buttons shows skills of the piece. Click piece, skill button, then target, command added to list.
5. After every commands for every pieces are added to List, play commands according to speed order. For pieces with same speed, attacking team plays first. For pieces with same speed and team, plays random order.
6. After combat, handle piece location according to the result of combat.
7. If Checkmate occurred or king is taken, Game end.
8. If player win, Choose reward.
9. If skill point earned, manage skill level for each piece.
10. Call next stage.
