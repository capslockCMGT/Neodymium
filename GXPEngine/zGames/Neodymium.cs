using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.Physics;

namespace GXPEngine
{
    public class Neodymium : Game
    {
        bool showCursor;
        public RotateAroundLevelCamera Camera;
        GameObject scene;
        int currentScene = 1;
        MainMenu menu;
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(80, 60, .1f, 100)));
            AddChild(Camera);
            loadScene(currentScene);

            menu = new MainMenu();
            uiManager.Add(menu);
            //sl.

        }
        public void nextLevel()
        {
            currentScene++;
            loadScene(currentScene);
        }

        void loadScene(int n)
        {
            scene?.Destroy();
            scene = Editor.GameObjectReader.ReadGameObjectTree("neodymium/Level"+n+".gxp3d");
            AddChild(scene);
            Checkpoint.AddCheckpointsToPlayer(scene.FindObjectOfType<Player>());
            cleanupPhysicsObjects(scene);
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
        }
    }
}
