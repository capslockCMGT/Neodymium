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
        SliderPanel HierarchyMenu;
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
            AddObjectButton.OnRelease += delegate () { SetActiveSideMenu(AddObjectMenu); };

            DestroyObjectButton = new TexturedButton("editor/buttons/DestroyObject.png", "editor/buttons/DestroyObjectHover.png", "editor/buttons/DestroyObjectClick.png");
            buttonHolder.AddChild(DestroyObjectButton);
            DestroyObjectButton.OnRelease += delegate () {
                if (editor.selectedGameobject == editor.mainGameObject) editor.DestroyCurrentTree();
                editor.selectedGameobject?.Destroy();
                editor.selectedGameobject = null;
            };

            TexturedButton translate = new TexturedButton("editor/buttons/TranslateObject.png", "editor/buttons/TranslateObjectHover.png", "editor/buttons/TranslateObjectClick.png");
            buttonHolder.AddChild(translate);
            translate.OnRelease += delegate () { editor.TransformGiz.transformMode = 0; };

            TexturedButton rotate = new TexturedButton("editor/buttons/RotateObject.png", "editor/buttons/RotateObjectHover.png", "editor/buttons/RotateObjectClick.png");
            buttonHolder.AddChild(rotate);
            rotate.OnRelease += delegate () { editor.TransformGiz.transformMode = 1; };

            TexturedButton scale = new TexturedButton("editor/buttons/ScaleObject.png", "editor/buttons/ScaleObjectHover.png", "editor/buttons/ScaleObjectClick.png");
            buttonHolder.AddChild(scale);
            scale.OnRelease += delegate () { editor.TransformGiz.transformMode = 2; };

            buttonHolder.OrganiseChildrenHorizontal();
            game.uiManager.Add(SelectedObjectPropertyPanel);
            game.uiManager.Add(buttonHolder);

            HierarchyMenu = new SliderPanel(300, game.height - 210, game.width - 305, 205);
            HierarchyMenu.SetSliderBar(20, game.height - 210);
            HierarchyMenu.OrganiseChildrenVertical();
            game.uiManager.Add(HierarchyMenu);

            SetupAddObjectMenu();
        }
        public void UpdateGameObjectPropertyMenu()
        {
            ObjectPropertyListContainer?.Destroy();
            if (editor.selectedGameobject == null) return;
            if (editor.selectedGameobject.ObjectType == null) return;
            ObjectPropertyListContainer = new Panel(1, 1, invisible:true);

            ObjectPropertyListContainer.AddChild(new TextPanel(276, 30, "Transform:", 10, false));

            EditorPropertyInput pos = new EditorPropertyInput(typeof(Vector3), "Position", editor.selectedGameobject.position);
            ObjectPropertyListContainer.AddChild(pos);
            pos.onValueChanged += delegate (object value) { editor.selectedGameobject.position = (Vector3)value; };
            EditorPropertyInput rot = new EditorPropertyInput(typeof(Quaternion), "Rotation", editor.selectedGameobject.rotation);
            ObjectPropertyListContainer.AddChild(rot);
            rot.onValueChanged += delegate (object value) { editor.selectedGameobject.rotation = (Quaternion)value; };
            EditorPropertyInput scale = new EditorPropertyInput(typeof(Vector3), "Scale", editor.selectedGameobject.scaleXYZ);
            ObjectPropertyListContainer.AddChild(scale);
            scale.onValueChanged += delegate (object value) { editor.selectedGameobject.scaleXYZ = (Vector3)value; };

            ObjectPropertyListContainer.AddChild(new TextPanel(276, 30, "Constructor:", 10, false));

            for (int i = 0; i < editor.selectedGameobject.ConstructorParameters.Length; i++)
            {
                ParameterInfo field = editor.selectedGameobject.ConstructorParams[i];
                var value = editor.selectedGameobject.ConstructorParameters[i];
                EditorPropertyInput property = new EditorPropertyInput(field.ParameterType, field.Name, value);
                int index = i; //i remembered :D
                property.onValueChanged += delegate (object val) { editor.selectedGameobject.ConstructorParameters[index] = val; editor.selectedGameobject.BuildObject(); UpdateGameObjectPropertyMenu(); };
                ObjectPropertyListContainer.AddChild(property);
            }

            if (editor.selectedGameobject.EditorDisplayObject == null)
            {
                TextPanel warning = new TextPanel(250, 30, "Constructor invalid! Check values", 12, false);
                warning.color = 0xFFDD0000;
                ObjectPropertyListContainer.AddChild (warning);
                ObjectPropertyListContainer.OrganiseChildrenVertical();
                ObjectPropertyListContainer.ResizeToContent();
                SelectedObjectPropertyPanel.AddChild(ObjectPropertyListContainer);
                SelectedObjectPropertyPanel.SetSliderBar(15, SelectedObjectPropertyPanel.height, 0, 0); 
                return; };

            ObjectPropertyListContainer.AddChild(new TextPanel(150, 30, "Public fields:", 10, false));

            for (int i = 0; i < editor.selectedGameobject.fields.Length; i++)
            {
                FieldInfo prop = editor.selectedGameobject.fields[i];
                var value = prop.GetValue(editor.selectedGameobject.EditorDisplayObject);
                EditorPropertyInput field = new EditorPropertyInput(prop.FieldType, prop.Name, value);
                field.onValueChanged += delegate (object val) { prop.SetValue(editor.selectedGameobject.EditorDisplayObject, val); };
                ObjectPropertyListContainer.AddChild(field);
            }

            ObjectPropertyListContainer.AddChild(new TextPanel(150,30,"Public properties:", 10, false));

            for (int i = 0; i<editor.selectedGameobject.properties.Length; i++)
            {
                PropertyInfo prop = editor.selectedGameobject.properties[i];
                var value = prop.GetValue(editor.selectedGameobject.EditorDisplayObject);
                EditorPropertyInput field = new EditorPropertyInput(prop.PropertyType, prop.Name, value);
                field.onValueChanged += delegate (object val) { prop.SetValue(editor.selectedGameobject.EditorDisplayObject, val); };
                ObjectPropertyListContainer.AddChild(field);
            }

            ObjectPropertyListContainer.OrganiseChildrenVertical();
            ObjectPropertyListContainer.ResizeToContent();
            SelectedObjectPropertyPanel.AddChild(ObjectPropertyListContainer);
            SelectedObjectPropertyPanel.SetSliderBar(15, SelectedObjectPropertyPanel.height, 0, 0);
            SelectedObjectPropertyPanel.OrganiseChildrenVertical();
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
            }
            AddObjectMenu.OrganiseChildrenVertical();
            AddObjectMenu.ResizeToContent();
        }

        void CreateHierarchy()
        {
            Panel targetWindow = HierarchyMenu;
            //change 'this' in the line below to whatever the main gameobject is
            //uh huh
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
