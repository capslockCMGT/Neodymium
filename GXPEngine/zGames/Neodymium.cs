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
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(80, 60, .1f, 100)));
            AddChild(Camera);
            scene = Editor.GameObjectReader.ReadGameObjectTree("neodymium/Level1test.gxp3d");
            AddChild(scene);
            Checkpoint.AddCheckpointsToPlayer(scene.FindObjectOfType<Player>());
            cleanupPhysicsObjects(scene);

            MainMenu sl = new MainMenu();
            uiManager.Add(sl);
            //sl.

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
        }
    }
}
