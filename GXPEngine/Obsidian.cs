using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.Physics;

namespace GXPEngine
{
    public class Obsidian : PhysicsBox
    {
        public Obsidian(Texture2D texture) : base(texture, true)
        {
            Disable();
            renderAs.visible = game is Editor.Exclusives.SceneEditor;
            simulated = false;
        }
    }
}
