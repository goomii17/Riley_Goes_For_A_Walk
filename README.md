![Riley](readme_data/RILEY_upscaled.png)

# Game Description

Hexagonal-grid turn-based strategy game created in Unity by **Sergio Herreros Pérez** and **José Ridruejo Tuñón**. It is inspired by [Hoplite](https://play.google.com/store/apps/details?id=com.magmafortress.hoplite&pcampaignid=web_share). The game features original sprites and soundtrack.

# Game Rules

## How to Play

In this turn-based strategy game on a hexagonal grid, the player controls Riley, a lab monster who has escaped from his incubator and is trying to get out of the lab.

## Game Levels

The game consists of 6 floors (levels), each randomly generated with a certain number of enemies that increases with each level.

## Enemies

There are three types of enemies:

- **Hazmat**: Melee attack (6 adjacent cells).
- **Swat**: Shoots up to a distance of 5 in their line of sight, but has no close range.
- **FireWarden**: Throws flames to adjacent cells and up to a distance of 2 in their line of sight.

## Player's Attacks

Riley has two types of attacks:

- **Melee**: Attacks adjacent enemies that were adjacent in the previous turn.
- **Frontal**: Attacks the enemy if you move towards them frontally.

## Special Tiles

- **Incubator**: Upon reaching the incubator tile, Riley gains 1 life.
- **Elevator**: To finish the level, you need to reach the elevator.

## Gameplay

Touching Riley or an enemy shows the cells that are in their attack range. Touching an empty cell shows the shortest path from Riley to the cell.
