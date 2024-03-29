using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GXPEngine.UI;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class Editor : Game
    {
        EditorCamera mainCam;
        GameObject activeSideMenu;
        Type[] gameObjectTypes;
        Panel AddObjectMenu;
        Panel ObjectConstructorMenu;
        TexturedButton AddObjectButton;

        public Editor() : base(1200, 600, false, true, true, "GXP Editor")
        { 
            SetupCam();
            SetupMainUI();
            SetupAddObjectMenu();
            SetActiveSideMenu(AddObjectMenu);
        }

        void Update()
        {
            DrawEditorGrid();
        }

        void SetActiveSideMenu(GameObject menuInQuestion)
        {
            if (activeSideMenu == null) activeSideMenu = menuInQuestion;
            else
            {
                uiManager.Remove(activeSideMenu);
                activeSideMenu = menuInQuestion;
            }
            menuInQuestion.x = 310;
            menuInQuestion.y = 5;
            uiManager.Add(menuInQuestion);
        }

        void TypeConstructorMenu()
        {
            Type inquestion = null;
            for (int i = 1; i < AddObjectMenu.GetChildCount(); i++)
            {
                if (AddObjectMenu.GetChildren(false)[i] is TextButton)
                    if (((TextButton)AddObjectMenu.GetChildren(false)[i]).status == Button.Status.CLICKED) inquestion = gameObjectTypes[i-1];
            }
            if (inquestion == null) return;
            Panel constructorMenu = new Panel(1, 1);
            constructorMenu.AddChild(new TextPanel(300, 15, "Pick constructor for "+inquestion.Name+":", 10, false));
            ConstructorInfo[] constructors = inquestion.GetConstructors();
            foreach(ConstructorInfo consInfo in constructors)
            {
                string constructorText = "(";
                ParameterInfo[] parameters = consInfo.GetParameters();
                foreach(ParameterInfo paramInfo in parameters)
                {
                    constructorText += paramInfo.ParameterType.Name+" ";
                    constructorText += paramInfo.Name;
                    if(paramInfo.HasDefaultValue) constructorText += " = "+paramInfo.DefaultValue.ToString();
                    constructorText += ", ";
                }
                if(constructorText.Length > 1) constructorText = constructorText.Substring(0, constructorText.Length - 2);
                constructorMenu.AddChild(new TextButton(300, 14, constructorText+")", 7));
            }
            constructorMenu.OrganiseChildrenVertical();
            SetActiveSideMenu(constructorMenu.ResizedToContent());
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
                txtButton.OnClick += TypeConstructorMenu;
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
            buttonHolder.AddChild(new TexturedButton("editor/buttons/TranslateObject.png", "editor/buttons/TranslateObjectHover.png", "editor/buttons/TranslateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/RotateObject.png", "editor/buttons/RotateObjectHover.png", "editor/buttons/RotateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/ScaleObject.png", "editor/buttons/ScaleObjectHover.png", "editor/buttons/ScaleObjectClick.png"));
            buttonHolder.OrganiseChildrenHorizontal();
            uiManager.Add(leftPanel);
            leftPanel.AddChild(buttonHolder);
        }

        void DrawEditorGrid()
        {
            Gizmos.DrawLine(0, 0, 0, 1f, 0, 0, this, 0xFFFF0000, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 1f, 0, this, 0xFF00FF00, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 0, 1f, this, 0xFF0000FF, 5);
            for (int i = 0; i < 22; i++)
            {
                uint col = i == 5 || i == 16 ? 0xFFFFFFFF : 0x77FFFFFF;
                if (i < 11) Gizmos.DrawLine(-6, 0, i - 5, 6, 0, i - 5, null, col, 1);
                else Gizmos.DrawLine(i - 16, 0, -6, i - 16, 0, 6, null, col, 1);
            }
        }
    }
}