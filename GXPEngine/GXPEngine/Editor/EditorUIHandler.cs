using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine;

namespace GXPEngine.Editor
{
    public class EditorUIHandler : GameObject
    {
        GameObject activeSideMenu;
        SliderPanel selectedGameObjectMenu;
        HierarchyItem editorDisplay;

        public TexturedButton AddObjectButton;
        public Panel AddObjectMenu;

        public Panel ObjectConstructorMenu;
        public void SetActiveSideMenu(GameObject menuInQuestion)
        {
            if (activeSideMenu == null) activeSideMenu = menuInQuestion;
            else
            {
                game.uiManager.Remove(activeSideMenu);
                activeSideMenu = menuInQuestion;
            }
            if (activeSideMenu == null) return;
            menuInQuestion.x = 310;
            menuInQuestion.y = 85;
            game.uiManager.Add(menuInQuestion);
        }
        void CreateTypeConstructorMenu(object Type)
        {
            if (Type.GetType() != typeof(Type)) return;
            Type inquestion = (Type)Type;

            ObjectConstructorMenu = new Panel(1, 1);
            ObjectConstructorMenu.AddChild(new TextPanel(game.width - 630, 15, "Pick constructor for " + inquestion.Name + ":", 10, false));
            ConstructorInfo[] constructors = inquestion.GetConstructors();

            foreach (ConstructorInfo consInfo in constructors)
            {
                string constructorText = "(";
                ParameterInfo[] parameters = consInfo.GetParameters();
                bool allowConstructor = true;
                foreach (ParameterInfo paramInfo in parameters)
                {
                    Type paramtype = paramInfo.ParameterType;
                    //whitelisted types for the constructor (cannot input a straight bitmap in the editor!)
                    allowConstructor &= paramInfo.HasDefaultValue || (paramtype == typeof(string)) || (paramtype == typeof(float)) || (paramtype == typeof(int)) || (paramtype == typeof(uint)) || (paramtype == typeof(bool) || paramtype == typeof(Texture2D));
                    constructorText += paramtype.Name + " ";
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
                if (constructorText.Length > 1) constructorText = constructorText.Substring(0, constructorText.Length - 2);
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
            ObjectConstructorMenu.ResizeToContent();
            SetActiveSideMenu(ObjectConstructorMenu);
        }

        void SetupAddObjectMenu()
        {
            var type = typeof(GameObject);
            var assembly = type.Assembly;
            //filtering out the ones i dont really like being able to add (game derivative classes, editor classes, ui classes etc)
            var gameObjectTypes = assembly.GetTypes().Where(testc => testc.IsSubclassOf(type) && (testc.Namespace != this.GetType().Namespace) && (testc.Namespace != typeof(Button).Namespace) && (testc.Name != typeof(MyGame).Name) && (testc.Name != typeof(Game).Name)).ToArray();
            AddObjectMenu = new Panel(1, 1);
            AddObjectMenu.AddChild(new TextPanel(150, 15, "Add object of type:", 10, false));
            foreach (Type typ in gameObjectTypes)
            {
                TextObjectButton txtButton = new TextObjectButton(150, 15, typ.Name, typ, 10);
                AddObjectMenu.AddChild(txtButton);
                txtButton.ObjOnClick += CreateTypeConstructorMenu;
                //Console.WriteLine(typ);
            }
            AddObjectMenu.OrganiseChildrenVertical();
            AddObjectMenu.ResizeToContent();
            //AddObjectMenu = AddObjectMenu.ResizedToContent();
        }

        void SetAddObjectMenuActive()
        {
            SetActiveSideMenu(AddObjectMenu);
        }

        void SetupMainUI()
        {
            Panel leftPanel = new Panel(300, game.height - 10, 5, 5);
            Panel buttonHolder = new Panel(1, 1, 310, 5, invisible: true);
            buttonHolder.scale = 3;

            AddObjectButton = new TexturedButton("editor/buttons/AddObject.png", "editor/buttons/AddObjectHover.png", "editor/buttons/AddObjectClick.png");
            buttonHolder.AddChild(AddObjectButton);
            AddObjectButton.OnClick += SetAddObjectMenuActive;

            buttonHolder.AddChild(new TexturedButton("editor/buttons/TranslateObject.png", "editor/buttons/TranslateObjectHover.png", "editor/buttons/TranslateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/RotateObject.png", "editor/buttons/RotateObjectHover.png", "editor/buttons/RotateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/ScaleObject.png", "editor/buttons/ScaleObjectHover.png", "editor/buttons/ScaleObjectClick.png"));
            buttonHolder.OrganiseChildrenHorizontal();
            game.uiManager.Add(leftPanel);
            game.uiManager.Add(buttonHolder);

            selectedGameObjectMenu = new SliderPanel(300, game.height - 10, game.width - 305, 5);
            selectedGameObjectMenu.SetSliderBar(20, game.height - 10);
            selectedGameObjectMenu.OrganiseChildrenVertical();
            game.uiManager.Add(selectedGameObjectMenu);
            game.uiManager.Add(new InputField(100, 20, 100, 100, 10));
        }
    }
}
