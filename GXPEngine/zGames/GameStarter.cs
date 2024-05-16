using GXPEngine;                                // GXPEngine contains the engine

public class GameStarter {
	public static Game activeGame;
    static void Main()                          // Main() is the first method that's called when the program is run
	{
		new Neodymium().Start();
	}
}