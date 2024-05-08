using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Neodymium : Game
    {
        bool showCursor;
        public RotateAroundLevelCamera Camera;
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(120, 75, .1f, 100)));
            AddChild(Camera);
            AddChild(Editor.GameObjectReader.ReadGameObjectTree("level1.gxp3d"));
            MainMenu sl = new MainMenu();
            uiManager.Add(sl);
            //sl.

        }
        void Update()
        {
        }
    }
}
