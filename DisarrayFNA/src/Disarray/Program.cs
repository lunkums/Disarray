﻿namespace Disarray;


class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        using (Game1 game = new Game1())
        {
            game.Run();
        }
    }
}
