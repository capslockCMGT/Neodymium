using GXPEngine.Core;
using GXPEngine.UI;
using System;

namespace GXPEngine
{
    public class ControlsTutorial : GameObject
    {
        bool enabled = false;
        Panel controlsPanel = new Panel("neodymium/controls.png");
        ControlsHint QE = new ControlsHint("neodymium/buttons/E.png", Key.Q, Key.E);
        ControlsHint AD = new ControlsHint("neodymium/buttons/A.png", Key.A, Key.D);
        ControlsHint WS = new ControlsHint("neodymium/buttons/W.png", Key.W, Key.S);
        ControlsHint SPACE = new ControlsHint("neodymium/buttons/SPACE.png", Key.SPACE);
        ControlsHint ENTER = new ControlsHint("neodymium/buttons/ENTER.png", Key.ENTER);
        public ControlsTutorial()
        {
            game.uiManager.Add(controlsPanel);
            controlsPanel.alpha = 0;
            controlsPanel.width = controlsPanel.game.width;
            controlsPanel.height = controlsPanel.game.height;
        }
        public void Update()
        {
            if (Input.GetKeyDown(Key.BACKSPACE))
            {
                enabled = !enabled;
            }
            if (enabled)
            {
                if (controlsPanel.alpha < 0.98)
                    controlsPanel.alpha += 0.02f;
                else
                    controlsPanel.alpha = 1;
            }
            else
            {
                if (controlsPanel.alpha > 0.02)
                    controlsPanel.alpha -= 0.02f;
                else
                    controlsPanel.alpha = 0;
            }
        }
        public void SetCraneHints (Crane crane)
        {
            crane.magnet.AddChild(SPACE);
            crane.hook.AddChild(WS);
            crane.cabin.AddChild(AD);
            crane.trunk.AddChild(QE);

            QE.position = new Vector3(1, 1, 0);
            QE.size = 0.002f;
            DSCFSprite Q = new DSCFSprite("neodymium/buttons/Q.png");
            Q.size = QE.size;
            Q.position = new Vector3(-3, 0, 0);
            QE.AddChild(Q);

            AD.position = new Vector3(0, -1, 0);
            AD.size = 0.002f;
            DSCFSprite D = new DSCFSprite("neodymium/buttons/D.png");
            D.size = QE.size;
            D.position = new Vector3(0, 0, 1);
            AD.AddChild(D);

            WS.position = new Vector3(1, 1, 0);
            WS.size = 0.002f;
            DSCFSprite S = new DSCFSprite("neodymium/buttons/S.png");
            S.size = QE.size;
            S.position = new Vector3(0, -1, 0);
            WS.AddChild(S);

            SPACE.position = new Vector3(1, 1, 0);
            SPACE.size = 0.002f;
        }
        public void SetPlayerHints(Player player)
        {
            player.parent.AddChild(ENTER);
            ENTER.position = player.position + new Vector3(0.2f, 0.2f, 0);
            ENTER.size = 0.002f;
        }
    }
}
