using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;
using GXPEngine.UI;

namespace GXPEngine
{
    public class MainMenu : Panel
    {
        bool inSubMenu = false;
        Panel mainButtons;
        Sprite transition;
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

            Button StartGame = new Button("neodymium/buttons/START.png");
            StartGame.scale = 0.3f;
            StartGame.OnRelease += delegate () {
                visible = false;
                (Game.main as Neodymium).StartGame();
            };

            Button QuitGame = new Button("neodymium/buttons/QUIT.png");
            QuitGame.scale = 0.3f;
            QuitGame.OnRelease += delegate () {
                visible = false;
                GL.glfwCloseWindow();
            };

            mainButtons.AddChild(StartGame);
            mainButtons.AddChild(QuitGame);

            mainButtons.OrganiseChildrenVertical(centerHorizontal: CenterMode.Center);

            transition = new Sprite("editor/whitePixel.png");
            transition.width = game.width;
            transition.height = game.height;
            game.uiManager.Add(transition);
            transition.alpha = 0;
        }
        public override void Update()
        {
            base.Update();
            if (inLevelTransitionAnim) AnimateLevelTransition();
        }

        private float timeTransition;
        bool loaded = true;
        bool inLevelTransitionAnim;
        public void NextLevelTransition()
        {
            loaded = false;
            inLevelTransitionAnim = true;
        }
        void AnimateLevelTransition()
        {
            timeTransition += Time.deltaTimeS;
            if(loaded)
            {
                transition.alpha = (4-timeTransition)*.7f;
                if(timeTransition > 4)
                {
                    inLevelTransitionAnim = false;
                    timeTransition = 0;
                }
            }
            else
            {
                transition.alpha = timeTransition*.7f;
                if(timeTransition > 2)
                {
                    (game as Neodymium).nextLevel();
                    loaded = true;
                }
            }
            transition.alpha = Mathf.Clamp(transition.alpha, 0, 1);
        }
    }
}
