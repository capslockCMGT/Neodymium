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
        public Neodymium() : base(1000, 750, false, gameName:"Neodymium") 
        {
            AddChild(new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(100, 75, .1f, 100)) ));
            AddChild(Editor.GameObjectReader.ReadGameObjectTree("level1.gxp3d"));
        }
        void Update()
        {
        }
    }
}
