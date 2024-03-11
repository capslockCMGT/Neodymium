using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Core
{
    public class BoxCollider : Collider
    {
        protected Box _owner;
        //------------------------------------------------------------------------------------------------------------------------
        //														BoxCollider()
        //------------------------------------------------------------------------------------------------------------------------		
        public BoxCollider(Box owner) 
        {
            _owner = owner;
        }
    }
}
