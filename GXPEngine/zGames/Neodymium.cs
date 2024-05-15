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
        MainMenu menu;
        LevelTransitioner transRights;
        ControlsTutorial controlsTutorial;
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(80, 50, .1f, 100)));
            AddChild(Camera);

            transRights = new LevelTransitioner();
            AddChild(transRights);
            loadScene(transRights.CurrentScene);

            menu = new MainMenu();
            uiManager.Add(menu);
            //sl.
            controlsTutorial = new ControlsTutorial();
            game.AddChild(controlsTutorial);
            controlsTutorial.SetCraneHints(scene?.FindObjectOfType<Crane>());
            controlsTutorial.SetPlayerHints(scene?.FindObjectOfType<Player>());

        }

        public void loadScene(int n)
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
            if (Input.GetKeyDown(Key.N))
                transRights.LevelTransition(transRights.CurrentScene + 1);
            if (Input.GetKeyDown(Key.R))
                transRights.Reload();
            if (Input.GetKeyDown(Key.MINUS_UNDERSCORE))
                Camera.distance += 3;
            if (Input.GetKeyDown(Key.EQUALS))
                Camera.distance -= 3;
        }
    }
}
