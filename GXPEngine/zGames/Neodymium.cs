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
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(80, 60, .1f, 100)));
            AddChild(Camera);
            AddChild(Editor.GameObjectReader.ReadGameObjectTree("neodymium/Level1.gxp3d"));
            MainMenu sl = new MainMenu();
            uiManager.Add(sl);
            //sl.

        }
        void Update()
        {
            PhysicsObject.UpdateAll();
        }
    }
}
