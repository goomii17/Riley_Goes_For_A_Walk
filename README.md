<p align="center">
  <img src="readme_data/RILEY_upscaled.png" alt="Riley" />
</p>

# Game Description

Hexagonal-grid turn-based strategy game created in Unity by **Sergio Herreros Pérez** and **José Ridruejo Tuñón**. It is inspired by [Hoplite](https://play.google.com/store/apps/details?id=com.magmafortress.hoplite&pcampaignid=web_share). The game features original lore, sprites and soundtrack.

# Game Rules

The player controls Riley, a lab monster who has escaped from his incubator and is trying to get out.

The game consists of 5 floors (levels), each randomly generated with a certain number of enemies that increases with each level.

Riley has two types of attacks:

- **Melee**: Attacks adjacent enemies that were adjacent in the previous turn.
- **Frontal**: Attacks the enemy if you move towards them frontally.

There are three types of enemies:

- **Hazmat**: Melee attack (6 adjacent cells).
- **Swat**: Shoots up to a distance of 5 in their line of sight, but cannot attack adjacent cells.
- **FireWarden**: Throws flames to adjacent cells and up to a distance of 2 in their line of sight.

There are two special structures:

- **Incubator**: Upon reaching the incubator tile, Riley drains the mutagenic fluid, evolving and gaining one heart.
- **Elevator**: You can take the elevator to the next floor if you reach the elevator tile.

## Gameplay

- Touching Riley or an enemy shows the cells that are in their attack range.
- Touching an empty cell or a structure highlights the shortest path from Riley to the cell.
- Touching a cell where riley can move automatically moves her to that cell.
- Clicking a highlighted path cell moves Riley one step following the path.