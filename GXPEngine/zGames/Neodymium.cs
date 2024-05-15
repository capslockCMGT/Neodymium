using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.Physics;
using GXPEngine.UI;

namespace GXPEngine
{
    public class Neodymium : Game
    {
        bool showCursor;
        public RotateAroundLevelCamera Camera;
        GameObject scene;
        int currentScene = 1;
        MainMenu menu;
        Player player;
        ControlsTutorial controlsTutorial;
        Panel HUD;
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(80, 50, .1f, 100)));
            AddChild(Camera);
            loadScene(currentScene);

            player = scene?.FindObjectOfType<Player>();
            menu = new MainMenu();
            uiManager.Add(menu);
            //sl.
            controlsTutorial = new ControlsTutorial();
            game.AddChild(controlsTutorial);
            controlsTutorial.SetCraneHints(scene?.FindObjectOfType<Crane>());
            controlsTutorial.SetPlayerHints(player);
            player.finished += menu.NextLevelTransition;

            SetupHud();
        }
        public void nextLevel()
        {
            currentScene++;
            loadScene(currentScene);
        }
        public void resetLevel()
        {
            loadScene(currentScene);
        }

        void loadScene(int n)
        {
            scene?.Destroy();
            scene = Editor.GameObjectReader.ReadGameObjectTree("neodymium/Level"+n+".gxp3d");
            AddChild(scene);
            Checkpoint.AddCheckpointsToPlayer(scene.FindObjectOfType<Player>());
            cleanupPhysicsObjects(scene);
            scene.FindObjectOfType<Magnet>()?.DetectAttractable();
        }
        void cleanupPhysicsObjects(GameObject p)
        {
            foreach(GameObject c in p.GetChildren())
            {
                cleanupPhysicsObjects(c);
                if(!(c is PhysicsObject)) continue;
                PhysicsObject obj = (PhysicsObject)c;
                obj.pos = obj.globalPosition;
                obj.prevPos = obj.globalPosition;
            }
        }
        void Update()
        {
            PhysicsObject.UpdateAll();
            if(Input.GetKeyDown(Key.N))
                menu.NextLevelTransition();
            if (Input.GetKeyDown(Key.R))
                resetLevel();
            if (Input.GetKeyDown(Key.MINUS_UNDERSCORE))
                Camera.distance += 3;
            if (Input.GetKeyDown(Key.EQUALS))
                Camera.distance -= 3;
        }
        void SetupHud()
        {
            HUD = new Panel(game.width, game.height, invisible:true);
            uiManager.Add(HUD);
            Panel controls = new Panel("neodymium/buttons/MENU.png");
            controls.scale = 0.2f;
            controls.SetOrigin(controls.width,controls.height);
            controls.position = new Vector3(HUD.width - 20, HUD.height - 20,0);
            HUD.AddChild(controls);
        }
    }
}
