public static class GameParams
{
	public const int MATRIX_WIDTH = 9;  // Width of matrix containing grid
	public const int MATRIX_HEIGHT = 11;    // Height of matrix containing grid

	public const int GRID_WIDTH = 5;    // Width of hexadecimal grid
	public const int GRID_HEIGHT = 7;   // Height of hexadecimal grid

	public const int CELL_WIDTH = 31;   // Width of square surrounding hexagon
	public const int CELL_HEIGHT = 27;  // Height of square surrounding hexagon
	public const float HEX_SIZE = 16;   // Size of a side of the hexagon

	public const float ENTITY_3D_HEIGHT = 4.6f; // Y of entity on cell
	public const float ENTITY_3D_DEPTH = -3.8f; // Z of entity on cell

	public const int STARTING_PLAYER_HEARTS = 3;    // Starting player hearts
	public const int NUMBERS_OF_LEVELS = 5; // Number of levels in the game

	// constant table mapping level to number of enemies of each type
	public static readonly int[,] LEVEL_ENEMY_COUNT = new int[5, 3]
	{
		{ 1, 1, 0 },
		{ 3, 1, 0 },
		{ 3, 2, 1 },
		{ 4, 3, 2 },
		{ 5, 4, 3 }
	};

}
