using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine.Editor;

namespace GXPEngine.Editor
{
    public class Editor : Game
    {
        EditorCamera mainCam;
        EditorGameObject mainGameObject;
        public EditorGameObject selectedGameobject;

        EditorUIHandler uiHandler;

        Type[] gameObjectTypes;
        ConstructorInfo[] constructors;

        public Editor() : base(1200, 600, false, true, true, "GXP Editor")
        { 
            SetupCam();
            uiHandler = new EditorUIHandler();

            SetupMainUI();
            SetupAddObjectMenu();
        }

        void AddGameObject()
        {
            uiHandler.SetActiveSideMenu(null);
            ConstructorInfo inquestion = null;
            int selected = 0;
            for (int i = 0; i < uiHandler.ObjectConstructorMenu.GetChildCount(); i++)
            {
                if (!(uiHandler.ObjectConstructorMenu.GetChildren(false)[i] is TextButton)) continue;
                if (((TextButton)uiHandler.ObjectConstructorMenu.GetChildren(false)[i]).status == Button.Status.CLICKED)
                    inquestion = constructors[selected];
                selected++;
            }
            if (inquestion == null) return;

            Type gameObjectType = inquestion.DeclaringType;
            EditorGameObject newObject = new EditorGameObject(gameObjectType, inquestion);
            if (mainGameObject == null)
            {
                mainGameObject = newObject;
                AddChild(newObject);
            }
            if (selectedGameobject != null)
            {
                selectedGameobject.AddChild(newObject);
            }
            selectedGameobject = newObject;
            Console.WriteLine("added object");
        }

        void Update()
        {
            DrawEditorGizmos();
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 start = mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 0);
                Vector3 end = mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 1);
            }
            UpdateHierarchy();
        }

        

        void CreateHierarchy()
        {
            Panel targetWindow = selectedGameObjectMenu;
            //change 'this' in the line below to whatever the main gameobject is
            editorDisplay = new HierarchyItem(mainGameObject,0,targetWindow.width - 30, 5,5);
            editorDisplay.ReadChildren();
            targetWindow.AddChild(editorDisplay);
        }
        void UpdateHierarchy()
        {
            if(editorDisplay == null)
            {
                if (mainGameObject == null) return;
                CreateHierarchy();
            }
            editorDisplay.UpdateChildren();
            editorDisplay.UpdateDisplay();
        }
        

        void SetupCam()
        {
            RenderMain = false;
            mainCam = new EditorCamera();
            mainCam.position = new Vector3(1, 1, 3);
            AddChild(mainCam);
        }


        void DrawEditorGizmos()
        {
            Gizmos.DrawLine(0, 0, 0, 1f, 0, 0, this, 0xFFFF0000, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 1f, 0, this, 0xFF00FF00, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 0, 1f, this, 0xFF0000FF, 5);
            for (int i = 0; i < 22; i++)
            {
                //dw abt it
                uint col = i == 5 || i == 16 ? 0xFFFFFFFF : 0x77FFFFFF;
                if (i < 11) Gizmos.DrawLine(-6, 0, i - 5, 6, 0, i - 5, null, col, 1);
                else Gizmos.DrawLine(i - 16, 0, -6, i - 16, 0, 6, null, col, 1);
            }
            if (selectedGameobject != null)
                Gizmos.DrawBox(0, 0, 0, 2, 2, 2, selectedGameobject, 0xFFFF9900, 8);
        }

        public override void Add(GameObject gameObject)
        {
            if(gameObject.GetType().Namespace == typeof(Editor).Namespace)
            base.Add(gameObject);
        }
    }
}