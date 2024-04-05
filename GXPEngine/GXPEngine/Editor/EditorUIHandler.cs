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
        Editor editor;

        GameObject activeSideMenu;
        SliderPanel selectedGameObjectMenu;
        HierarchyItem editorDisplay;

        TexturedButton AddObjectButton;
        Panel AddObjectMenu;

        Panel ObjectConstructorMenu;

        public int millisSinceButtonPressed = 0;

        public EditorUIHandler()
        {
            editor = (Editor)game;
            Button.AnyButtonOnClick += AnyButtonPressed;
        }
        public void SetupMainUI()
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
            game.uiManager.Add(new InputField(100, 20, false, 100, 100, 10));

            SetupAddObjectMenu();
        }

        public void SetActiveSideMenu(GameObject menuInQuestion)
        {
            if (activeSideMenu == null) activeSideMenu = menuInQuestion;
            else
            {
                game.uiManager.Remove(activeSideMenu);
                activeSideMenu = menuInQuestion;
            }
            //schizoid code
            if (activeSideMenu == null) return;
            menuInQuestion.x = 310;
            menuInQuestion.y = 85;
            game.uiManager.Add(menuInQuestion);
        }
        void CreateTypeConstructorMenu(object Type)
        {
            if (!(Type is Type)) return;
            Type inquestion = (Type)Type;

            ObjectConstructorMenu = new Panel(1, 1);
            ObjectConstructorMenu.AddChild(new TextPanel(game.width - 630, 15, "Pick constructor for " + inquestion.Name + ":", 10, false));
            ConstructorInfo[] constructors = inquestion.GetConstructors();

            foreach (ConstructorInfo consInfo in constructors)
            {
                Panel constructorPanel = null;
                bool allowConstructor = TypeHandler.IsValidConstructor(consInfo);
                string constructorText = TypeHandler.GetConstructorAsText(consInfo);
                if (allowConstructor)
                {
                    constructorPanel = new TextObjectButton(editor.width - 630, 14, constructorText + ")", consInfo, 7);
                    ((TextObjectButton)constructorPanel).ObjOnRelease += editor.AddGameObject;
                }
                else
                {
                    constructorPanel = new TextPanel(game.width - 630, 14, constructorText + ") (disabled, constructor contains invalid types)", 7) { alpha = .4f };
                }
                ObjectConstructorMenu.AddChild(constructorPanel);
            }
            ObjectConstructorMenu.OrganiseChildrenVertical();
            ObjectConstructorMenu.ResizeToContent();
            SetActiveSideMenu(ObjectConstructorMenu);
        }

        public void SetupAddObjectMenu()
        {
            Type[] gameObjectTypes = TypeHandler.GetInstantiableObjects();
            AddObjectMenu = new Panel(1, 1);
            AddObjectMenu.AddChild(new TextPanel(150, 15, "Add object of type:", 10, false));
            foreach (Type typ in gameObjectTypes)
            {
                TextObjectButton txtButton = new TextObjectButton(150, 15, typ.Name, typ, 10);
                AddObjectMenu.AddChild(txtButton);
                txtButton.ObjOnRelease += CreateTypeConstructorMenu;
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

        void CreateHierarchy()
        {
            Panel targetWindow = selectedGameObjectMenu;
            //change 'this' in the line below to whatever the main gameobject is
            editorDisplay = new HierarchyItem(editor.mainGameObject, 0, targetWindow.width - 30, 5, 5);
            editorDisplay.ReadChildren();
            targetWindow.AddChild(editorDisplay);
        }
        public void UpdateHierarchy()
        {
            millisSinceButtonPressed += Time.deltaTime;
            if (editorDisplay == null)
            {
                if (editor.mainGameObject == null) return;
                CreateHierarchy();
            }
            editorDisplay.UpdateChildren();
            editorDisplay.UpdateDisplay();
        }
        void AnyButtonPressed()
        {
            millisSinceButtonPressed = 0;
        }
    }
}
