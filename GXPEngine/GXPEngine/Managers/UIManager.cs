using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static GXPEngine.UI.Button;

namespace GXPEngine.UI
{
    public class UIManager
    {
        //boop
        private Window main;
        private delegate void UpdateDelegate();

        private UpdateDelegate _updateDelegates;
        private Dictionary<GameObject, UpdateDelegate> _updateReferences = new Dictionary<GameObject, UpdateDelegate>();
        public UIManager()
        {

        }
        public void AssignWindow(Window window)
        {
            main = window;
        }
        public void Render(GLContext glContext)
        {
            glContext.PushMatrix(main.window.matrix);
            foreach (GameObject panel in _updateReferences.Keys)
            {
                panel.Render(glContext);
            }
            glContext.PopMatrix();
        }
        public void Step()
        {
            if (_updateDelegates != null)
                _updateDelegates();
        }
        public void Add(GameObject gameObject)
        {
            MethodInfo info = gameObject.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (info != null)
            {
                UpdateDelegate onUpdate = (UpdateDelegate)Delegate.CreateDelegate(typeof(UpdateDelegate), gameObject, info, false);
                if (onUpdate != null && !_updateReferences.ContainsKey(gameObject))
                {
                    _updateReferences[gameObject] = onUpdate;
                    _updateDelegates += onUpdate;
                }
            }
        }

        public void Remove(GameObject gameObject)
        {
            if (_updateReferences.ContainsKey(gameObject))
            {
                UpdateDelegate onUpdate = _updateReferences[gameObject];
                if (onUpdate != null) _updateDelegates -= onUpdate;
                _updateReferences.Remove(gameObject);
            }
        }
    }
}
