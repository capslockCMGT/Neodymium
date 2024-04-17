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
        TexturedButton DestroyObjectButton;

        Panel AddObjectMenu;
        Panel ObjectConstructorMenu;
        SliderPanel SelectedObjectPropertyPanel;
        Panel ObjectPropertyListContainer;

        public int millisSinceButtonPressed = 0;

        public EditorUIHandler()
        {
            editor = (Editor)game;
            Button.AnyButtonOnClick += AnyButtonPressed;
        }
        public void SetupMainUI()
        {
            SelectedObjectPropertyPanel = new SliderPanel(300, game.height - 10, 5, 5);
            Panel buttonHolder = new Panel(1, 1, 310, 5, invisible: true);
            buttonHolder.scale = 3;

            AddObjectButton = new TexturedButton("editor/buttons/AddObject.png", "editor/buttons/AddObjectHover.png", "editor/buttons/AddObjectClick.png");
            buttonHolder.AddChild(AddObjectButton);
            AddObjectButton.OnClick += delegate () { SetActiveSideMenu(AddObjectMenu); };

            DestroyObjectButton = new TexturedButton("editor/buttons/DestroyObject.png", "editor/buttons/DestroyObjectHover.png", "editor/buttons/DestroyObjectClick.png");
            buttonHolder.AddChild(DestroyObjectButton);
            DestroyObjectButton.OnClick += destroySelectedObject;

            buttonHolder.AddChild(new TexturedButton("editor/buttons/TranslateObject.png", "editor/buttons/TranslateObjectHover.png", "editor/buttons/TranslateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/RotateObject.png", "editor/buttons/RotateObjectHover.png", "editor/buttons/RotateObjectClick.png"));
            buttonHolder.AddChild(new TexturedButton("editor/buttons/ScaleObject.png", "editor/buttons/ScaleObjectHover.png", "editor/buttons/ScaleObjectClick.png"));
            buttonHolder.OrganiseChildrenHorizontal();
            game.uiManager.Add(SelectedObjectPropertyPanel);
            game.uiManager.Add(buttonHolder);

            selectedGameObjectMenu = new SliderPanel(300, game.height - 410, game.width - 305, 5);
            selectedGameObjectMenu.SetSliderBar(20, game.height - 410);
            selectedGameObjectMenu.OrganiseChildrenVertical();
            game.uiManager.Add(selectedGameObjectMenu);
            //game.uiManager.Add(new InputField(100, 20, false, 100, 100, 10));

            SetupAddObjectMenu();
        }
        public void UpdateGameObjectPropertyMenu()
        {
            ObjectPropertyListContainer?.Destroy();
            if (editor.selectedGameobject == null) return;
            if (editor.selectedGameobject.ObjectType == null) return;
            if (editor.selectedGameobject.EditorDisplayObject == null) return;
            ObjectPropertyListContainer = new Panel(1, 1, invisible:true);

            ObjectPropertyListContainer.AddChild(new TextPanel(276, 15, "Transform:", 10, false));
            TextPanel pos = new TextPanel(110, 15, "Vector3 position:", 8, false);
            ObjectPropertyListContainer.AddChild(pos);
            InputField input = new InputField(150, 15, 125, 0, editor.selectedGameobject.position.ToString(), 10);
            pos.AddChild(input);
            TextPanel rot = new TextPanel(110, 15, "Quaternion rotation:", 8, false);
            ObjectPropertyListContainer.AddChild(rot);
            input = new InputField(150, 15, 125, 0, editor.selectedGameobject.rotation.ToString(), 10);
            rot.AddChild(input);
            TextPanel scale = new TextPanel(110, 15, "Vector3 scale:", 8, false);
            ObjectPropertyListContainer.AddChild(scale);
            input = new InputField(150, 15, 125, 0, editor.selectedGameobject.scaleXYZ.ToString(), 10);
            scale.AddChild(input);

            ObjectPropertyListContainer.AddChild(new TextPanel(276, 15, "Constructor:", 10, false));
            for (int i = 0; i < editor.selectedGameobject.ConstructorParameters.Length; i++)
            {
                ParameterInfo field = editor.selectedGameobject.ConstructorParams[i];
                TextPanel fieldname = new TextPanel(110, 15, field.ParameterType.Name + " " + field.Name, 8, false);
                ObjectPropertyListContainer.AddChild(fieldname);
                var value = editor.selectedGameobject.ConstructorParameters[i];
                input = new InputField(150, 15, 125, 0, value == null ? "null" : value.ToString(), 10);
                fieldname.AddChild(input);
            }

            ObjectPropertyListContainer.AddChild(new TextPanel(150,15,"Public variables:", 10, false));
            for (int i = 0; i<editor.selectedGameobject.fields.Length; i++)
            {
                FieldInfo field = editor.selectedGameobject.fields[i];
                TextPanel fieldname = new TextPanel(110,15,field.FieldType.Name +" "+ field.Name,8, false);
                ObjectPropertyListContainer.AddChild(fieldname);
                var value = field.GetValue(editor.selectedGameobject.EditorDisplayObject);
                input = new InputField(150, 15, 125, 0, value == null ? "null" : value.ToString(), 10);
                fieldname.AddChild(input);
            }
            ObjectPropertyListContainer.OrganiseChildrenVertical();
            ObjectPropertyListContainer.ResizeToContent();
            SelectedObjectPropertyPanel.AddChild(ObjectPropertyListContainer);
            SelectedObjectPropertyPanel.SetSliderBar(15, SelectedObjectPropertyPanel.height, 0, 0);
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
        void CreateTypeConstructorMenu(Type type)
        {
            ObjectConstructorMenu = new Panel(1, 1);
            ObjectConstructorMenu.AddChild(new TextPanel(game.width - 630, 15, "Pick constructor for " + type.Name + ":", 10, false));
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (ConstructorInfo consInfo in constructors)
            {
                Panel constructorPanel = null;
                bool allowConstructor = TypeHandler.IsValidConstructor(consInfo);
                string constructorText = TypeHandler.GetConstructorAsText(consInfo);
                if (allowConstructor)
                {
                    constructorPanel = new TextButton(editor.width - 630, 14, constructorText + ")", 7);
                    ((TextButton)constructorPanel).OnRelease += delegate() { editor.AddGameObject(consInfo); } ;
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
                TextButton txtButton = new TextButton(150, 15, typ.Name, 10);
                AddObjectMenu.AddChild(txtButton);
                txtButton.OnRelease += delegate() { CreateTypeConstructorMenu(typ); };
                //Console.WriteLine(typ);
            }
            AddObjectMenu.OrganiseChildrenVertical();
            AddObjectMenu.ResizeToContent();
            //AddObjectMenu = AddObjectMenu.ResizedToContent();
        }

        void destroySelectedObject()
        {
            if (editor.selectedGameobject == editor.mainGameObject) return;
            editor.selectedGameobject?.Destroy();
            editor.selectedGameobject = null;
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
