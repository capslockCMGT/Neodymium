using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine.GXPEngine.UI;
using GXPEngine.GXPEngine.Editor;

namespace GXPEngine.Editor
{
    public class Editor : Game
    {
        EditorCamera mainCam;
        GameObject mainGameObject;
        public GameObject selectedGameobject;

        GameObject activeSideMenu;
        SliderPanel selectedGameObjectMenu;
        HierarchyItem editorDisplay;

        TexturedButton AddObjectButton;
        Panel AddObjectMenu;
        Type[] gameObjectTypes;

        Panel ObjectConstructorMenu;
        ConstructorInfo[] constructors;

        public Editor() : base(1200, 600, false, true, true, "GXP Editor")
        { 
            SetupCam();
            SetupMainUI();
            SetupAddObjectMenu();
        }

        void Update()
        {
            DrawEditorGrid();
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 start = mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 0);
                Vector3 end = mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 1);
            }
            UpdateHierarchy();
        }


        void SetActiveSideMenu(GameObject menuInQuestion)
        {
            if (activeSideMenu == null) activeSideMenu = menuInQuestion;
            else
            {
                uiManager.Remove(activeSideMenu);
                activeSideMenu = menuInQuestion;
            }
            if (activeSideMenu == null) return; 
            menuInQuestion.x = 310;
            menuInQuestion.y = 5;
            uiManager.Add(menuInQuestion);
        }

        void AddGameObject()
        {
            SetActiveSideMenu(null);
            ConstructorInfo inquestion = null;
            int selected = 0;
            for (int i = 0; i < ObjectConstructorMenu.GetChildCount(); i++)
            {
                if (!(ObjectConstructorMenu.GetChildren(false)[i] is TextButton)) continue;
                if (((TextButton)ObjectConstructorMenu.GetChildren(false)[i]).status == Button.Status.CLICKED)
                    inquestion = constructors[selected];
                selected++;
            }
            if (inquestion == null) return;

            Type gameObjectType = inquestion.DeclaringType;
            if (gameObjectType.IsSubclassOf(typeof(Box)) || gameObjectType == typeof(Box))
            {
                BoxProxy newObject = new BoxProxy();
                newObject.Constructor = inquestion;
                newObject.ObjectType = gameObjectType;
                selectedGameobject = newObject;
                if(mainGameObject == null)
                {
                    mainGameObject = newObject;
                    AddChild(newObject);
                }
                Console.WriteLine("added box");
            }
            else
            {
                GameobjectProxy newObject = new GameobjectProxy();
                newObject.Constructor = inquestion;
                newObject.ObjectType = gameObjectType;
                selectedGameobject = newObject;
                if (mainGameObject == null)
                {
                    mainGameObject = newObject;
                    AddChild(newObject);
                }
                Console.WriteLine("added object");
            }
        }

        void CreateTypeConstructorMenu()
        {
            Type inquestion = null;
            int selected = 0;
            for (int i = 0; i < AddObjectMenu.GetChildCount(); i++)
            {
                if (!(AddObjectMenu.GetChildren(false)[i] is TextButton)) continue;
                selected++;
                if (((TextButton)AddObjectMenu.GetChildren(false)[selected]).status == Button.Status.CLICKED) 
                    inquestion = gameObjectTypes[i-1];
            }
            if (inquestion == null) return;

            ObjectConstructorMenu = new Panel(1, 1);
            ObjectConstructorMenu.AddChild(new TextPanel(width-630, 15, "Pick constructor for "+inquestion.Name+":", 10, false));
            constructors = inquestion.GetConstructors();

            foreach(ConstructorInfo consInfo in constructors)
            {
                string constructorText = "(";
                ParameterInfo[] parameters = consInfo.GetParameters();
                bool allowConstructor = true;
                foreach(ParameterInfo paramInfo in parameters)
                {
                    Type paramtype = paramInfo.ParameterType;
                    //whitelisted types for the constructor (cannot input a straight bitmap in the editor!)
                    allowConstructor &= paramInfo.HasDefaultValue || (paramtype == typeof(string)) || (paramtype == typeof(float)) || (paramtype == typeof(int)) || (paramtype == typeof(uint)) || (paramtype == typeof(bool));
                    constructorText += paramtype.Name+" ";
                    constructorText += paramInfo.Name;
                    if (paramInfo.HasDefaultValue)
                    {
                        object value = paramInfo.DefaultValue;
                        if (value != null)
                            constructorText += " = " + value.ToString();
                        else constructorText += " = null";
                    }
                    constructorText += ", ";
                }
                if(constructorText.Length > 1) constructorText = constructorText.Substring(0, constructorText.Length - 2);
                Panel constructorPanel = null;
                if (allowConstructor)
                {
                    constructorPanel = new TextButton(width - 630, 14, constructorText + ")", 7);
                    ((TextButton)constructorPanel).OnClick += AddGameObject;
                }
                else
                { 
                    constructorPanel = new TextPanel(width - 630, 14, constructorText + ") (disabled, constructor contains invalid types)", 7) { alpha = .4f };
                    constructors = constructors.Where(testc => testc != consInfo).ToArray();
                }
                ObjectConstructorMenu.AddChild(constructorPanel);
            }
            ObjectConstructorMenu.OrganiseChildrenVertical();
            ObjectConstructorMenu = ObjectConstructorMenu.ResizedToContent();
            SetActiveSideMenu(ObjectConstructorMenu);
        }
        void SetAddObjectMenuActive()
        {
            SetActiveSideMenu(AddObjectMenu);
        }

        void CreateHierarchy()
        {
            Panel targetWindow = selectedGameObjectMenu;
            //change 'this' in the line below to whatever the main gameobject is
            editorDisplay = new HierarchyItem(this,0,targetWindow.width - 30, 5,5);
            editorDisplay.ReadChildren();
            targetWindow.AddChild(editorDisplay);
        }
        void UpdateHierarchy()
        {
            editorDisplay.UpdateChildren();
            editorDisplay.UpdateDisplay();
        }
        
        void SetupAddObjectMenu()
        {
            var type = typeof(GameObject);
            var assembly = type.Assembly;
            //filtering out the ones i dont really like being able to add (game derivative classes, editor classes, ui classes etc)
            gameObjectTypes = assembly.GetTypes().Where(testc => testc.IsSubclassOf(type) && (testc.Namespace != this.GetType().Namespace) && (testc.Namespace != typeof(Button).Namespace) && (testc.Name != typeof(MyGame).Name) && (testc.Name != typeof(Game).Name)).ToArray();
            AddObjectMenu = new Panel(1, 1);
            AddObjectMenu.AddChild(new TextPanel(150, 15, "Add object of type:", 10, false));
            foreach (Type typ in gameObjectTypes)
            {
                TextButton txtButton = new TextButton(150, 15, typ.Name, 10);
                AddObjectMenu.AddChild(txtButton);
                txtButton.OnClick += CreateTypeConstructorMenu;
                //Console.WriteLine(typ);
            }
            AddObjectMenu.OrganiseChildrenVertical();
            AddObjectMenu = AddObjectMenu.ResizedToContent();
        }

        void SetupCam()
        {
            RenderMain = false;
            mainCam = new EditorCamera();
            mainCam.position = new Vector3(1, 1, 3);
            AddChild(mainCam);
        }

        void SetupMainUI()
        {
            Panel leftPanel = new Panel(300,height-10, 5, 5);
            Panel buttonHolder = new Panel(1, 1, invisible: true);
            buttonHolder.scale = 3;

            AddObjectButton = new TexturedButton("editor/buttons/AddObject.png", "editor/buttons/AddObjectHover.png", "editor/buttons/AddObjectClick.png");
            buttonHolder.AddChild(AddObjectButton);
            AddObjectButton.OnClick += SetAddObjectMenuActive;

            buttonHolder.AddChild(new TexturedButton("editor/buttons/TranslateObject.png", "editor/buttons/TranslateObjectHover.png", "editor/buttons/TranslateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/RotateObject.png", "editor/buttons/RotateObjectHover.png", "editor/buttons/RotateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/ScaleObject.png", "editor/buttons/ScaleObjectHover.png", "editor/buttons/ScaleObjectClick.png"));
            buttonHolder.OrganiseChildrenHorizontal();
            uiManager.Add(leftPanel);
            leftPanel.AddChild(buttonHolder);

            selectedGameObjectMenu = new SliderPanel(300, height - 10, width - 305, 5);
            selectedGameObjectMenu.SetSliderBar(20, height - 10);
            selectedGameObjectMenu.OrganiseChildrenVertical();
            uiManager.Add(selectedGameObjectMenu);

            //also see UpdateHierarchy() in the Update()
            CreateHierarchy();
        }

        void DrawEditorGrid()
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
        }
    }
}