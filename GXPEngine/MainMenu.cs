using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine
{
    public class MainMenu : Panel
    {
        bool inSubMenu = false;
        Panel mainButtons;
        public MainMenu() : base(500, 700, Game.main.width * .5f - 250, Game.main.height * .5f - 350)
        {
            Clear(0, 0, 0, 100);
            alpha = 0;
            SetupMain();
        }

        public void SetupMain()
        {
            mainButtons = new Panel(1, 1, invisible: true);
            mainButtons.width = width;
            mainButtons.y = 500;
            AddChild(mainButtons);

            TextButton StartGame = new TextButton(300, 75, "Start Game", 50);
            StartGame.OnRelease += delegate () {
                visible = false;
                Console.WriteLine("RAAA");
                (Game.main as Neodymium).Camera.CamEnabled = true;
            };
            mainButtons.AddChild(StartGame);
            mainButtons.OrganiseChildrenVertical(centerHorizontal: CenterMode.Center);
        }
    }
}
