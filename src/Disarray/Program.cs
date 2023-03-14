using Disarray.Engine;

namespace Disarray;


class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        using (Main game = Data.LoadFromFilePath<Main>("game_settings.json"))
        {
            game.Run();
        }
    }
}
