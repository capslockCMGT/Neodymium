using GXPEngine.Core;
using GXPEngine.UI;
using System;

namespace GXPEngine
{
    public class ControlsTutorial : GameObject
    {
        bool enabled = false;
        Panel controlsPanel = new Panel("neodymium/controls.png");
        DSCFSprite QE = new DSCFSprite("amongus.png");
        DSCFSprite AD = new DSCFSprite("amongus.png");
        DSCFSprite WS = new DSCFSprite("amongus.png");
        DSCFSprite SPACE = new DSCFSprite("amongus.png");
        DSCFSprite ENTER = new DSCFSprite("amongus.png");
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
            if (Input.GetKeyDown(Key.Q) || Input.GetKeyDown(Key.E))
                QE.alpha = 0;
            if (Input.GetKeyDown(Key.A) || Input.GetKeyDown(Key.D))
                AD.alpha = 0;
            if (Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.S))
                WS.alpha = 0;
            if (Input.GetKeyDown(Key.SPACE))
                SPACE.alpha = 0;
            if (Input.GetKeyDown(Key.ENTER))
                ENTER.alpha = 0;
        }
        public void SetCraneHints (Crane crane)
        {
            crane.magnet.AddChild(SPACE);
            crane.hook.AddChild(WS);
            crane.cabin.AddChild(AD);
            crane.trunk.AddChild(QE);

            QE.position = new Vector3(1, 1, 0);
            QE.size = 0.001f;
            AD.position = new Vector3(1, 1, 0);
            AD.size = 0.001f;
            WS.position = new Vector3(1, 1, 0);
            WS.size = 0.001f;
            SPACE.position = new Vector3(1, 1, 0);
            SPACE.size = 0.001f;
        }
        public void SetPlayerHints(Player player)
        {
            player.parent.AddChild(ENTER);
            ENTER.position = player.position + new Vector3(0.2f, 0.2f, 0);
            ENTER.size = 0.001f;
        }
    }
}
