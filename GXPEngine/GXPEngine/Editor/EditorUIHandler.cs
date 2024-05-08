using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine;

namespace GXPEngine.Editor.Exclusives
{
    public class EditorUIHandler : GameObject
    {
        SceneEditor editor;

        GameObject activeSideMenu;

        SliderPanel HierarchyMenu;
        HierarchyItem editorDisplay;

        Panel AddObjectMenu;
        Panel ObjectConstructorMenu;

        SliderPanel SelectedObjectPropertyPanel;
        Panel ObjectPropertyListContainer;

        Panel SceneMenu;
        TextPanel sceneName;

        public int millisSinceButtonPressed = 0;

        public EditorUIHandler()
        {
            editor = (SceneEditor)game;
            Button.AnyButtonOnClick += delegate () { millisSinceButtonPressed = 0; };
        }
        public void SetupMainUI()
        {
            SelectedObjectPropertyPanel = new SliderPanel(300, game.height - 10, 5, 5, true);
            SelectedObjectPropertyPanel.SetSliderBar(15, (int)SelectedObjectPropertyPanel.height, 0, 0);
            Panel buttonHolder = new Panel(1, 1, 310, 5, invisible: true);
            buttonHolder.scale = 3;

            TexturedButton AddObjectButton = new TexturedButton("editor/buttons/AddObject.png", "editor/buttons/AddObjectHover.png", "editor/buttons/AddObjectClick.png");
            buttonHolder.AddChild(AddObjectButton);
            AddObjectButton.OnRelease += delegate () { SetActiveSideMenu(AddObjectMenu == activeSideMenu ? null : AddObjectMenu); };

            TexturedButton DestroyObjectButton = new TexturedButton("editor/buttons/DestroyObject.png", "editor/buttons/DestroyObjectHover.png", "editor/buttons/DestroyObjectClick.png");
            buttonHolder.AddChild(DestroyObjectButton);
            DestroyObjectButton.OnRelease += delegate () {
                if (editor.selectedGameobject == editor.mainGameObject) { editor.DestroyCurrentTree(null); return; }
                //editor.selectedGameobject?.Destroy();
                //editor.selectedGameobject = null;
                EditorActionRegister.RemoveObject(editor.selectedGameobject);
            };

            //this sucks
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

            HierarchyMenu = new SliderPanel(300, game.height - 105, game.width - 305, 100, true);
            HierarchyMenu.SetSliderBar(20, game.height - 105);
            HierarchyMenu.OrganiseChildrenVertical();
            game.uiManager.Add(HierarchyMenu);

            SceneMenu = new Panel(300, 90, game.width - 305, 5);
            if (string.IsNullOrEmpty(editor.loadedScene))
                sceneName = new TextPanel(200, 19, "Unsaved scene", 13, false);
            else sceneName = new TextPanel(200, 19, editor.loadedScene, 13, false);
            SceneMenu.AddChild(sceneName);
            TextButton save = new TextButton(200, 15, "Save scene", 13);
            SceneMenu.AddChild(save);
            save.OnRelease += delegate () 
            {
                if (string.IsNullOrEmpty(editor.loadedScene))
                {
                    editor.SaveSceneAs();
                    if(!string.IsNullOrEmpty(editor.loadedScene))
                    {
                        sceneName.ClearTransparent();
                        sceneName.Text(editor.loadedScene);
                    }
                }
                else GameObjectWriter.WriteEditorGameObjectTree(editor.mainGameObject, editor.loadedScene);
            };
            TextButton saveAs = new TextButton(200, 15, "Save scene as...", 13);
            SceneMenu.AddChild(saveAs);
            saveAs.OnRelease += delegate () { editor.SaveSceneAs(); };
            TextButton load = new TextButton(200, 15, "Load scene...", 13);
            SceneMenu.AddChild(load);
            load.OnRelease += delegate () { editor.LoadScene(); };
            game.uiManager.Add(SceneMenu);
            SceneMenu.OrganiseChildrenVertical(centerHorizontal:CenterMode.Center);

            SetupAddObjectMenu();
        }
        public void DestroySceneConfirmationDialog(Button.NoArgs dewit)
        {
            Panel background = new Panel(1, 1);
            TextPanel txt = new TextPanel(300, 12, "Are you sure you want to delete the current scene?",10,false);
            TextPanel txt2 = new TextPanel(200, 12, "This cannot be undone.",10,false);
            TextButton bt = new TextButton(200, 15, "Continue",13);
            TextButton bt2 = new TextButton(200, 15, "Nevermind",13);
            background.AddChild(txt);
            background.AddChild(txt2);
            background.AddChild(bt);
            background.AddChild(bt2);
            bt2.OnRelease += delegate () { SetActiveSideMenu(null); background.LateDestroy(); };
            bt.OnRelease += dewit;
            background.OrganiseChildrenVertical();
            background.ResizeToContent();
            SetActiveSideMenu(background);
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
            pos.onValueChanged += delegate (object value) { EditorActionRegister.SetPositionValue((Vector3)value, editor.selectedGameobject); };
            EditorPropertyInput rot = new EditorPropertyInput(typeof(Quaternion), "Rotation", editor.selectedGameobject.rotation);
            ObjectPropertyListContainer.AddChild(rot);
            rot.onValueChanged += delegate (object value) { EditorActionRegister.SetRotationValue((Quaternion)value, editor.selectedGameobject); };
            EditorPropertyInput scale = new EditorPropertyInput(typeof(Vector3), "Scale", editor.selectedGameobject.scaleXYZ);
            ObjectPropertyListContainer.AddChild(scale);
            scale.onValueChanged += delegate (object value) { EditorActionRegister.SetScaleValue((Vector3)value, editor.selectedGameobject); };

            ObjectPropertyListContainer.AddChild(new TextPanel(276, 30, "Constructor:", 10, false));

            for (int i = 0; i < editor.selectedGameobject.ConstructorParameters.Length; i++)
            {
                ParameterInfo field = editor.selectedGameobject.ConstructorParams[i];
                var value = editor.selectedGameobject.ConstructorParameters[i];
                EditorPropertyInput property = new EditorPropertyInput(field.ParameterType, field.Name, value);
                int index = i; //i remembered :D
                property.onValueChanged += delegate (object val) { EditorActionRegister.SetConstructorValue(val, index); };
                ObjectPropertyListContainer.AddChild(property);
            }

            if (editor.selectedGameobject.EditorDisplayObject == null)
            {
                TextPanel warning = new TextPanel(250, 30, "Constructor invalid! Check values", 12, false);
                warning.color = 0xFFDD0000;
                ObjectPropertyListContainer.AddChild (warning);
                Finalize();
                return; };

            ObjectPropertyListContainer.AddChild(new TextPanel(150, 30, "Public fields:", 10, false));

            for (int i = 0; i < editor.selectedGameobject.fields.Length; i++)
            {
                FieldInfo prop = editor.selectedGameobject.fields[i];
                var value = prop.GetValue(editor.selectedGameobject.EditorDisplayObject);
                EditorPropertyInput field = new EditorPropertyInput(prop.FieldType, prop.Name, value);
                field.onValueChanged += delegate (object val) { EditorActionRegister.SetFieldValue(val, prop, editor.selectedGameobject); };
                ObjectPropertyListContainer.AddChild(field);
            }

            ObjectPropertyListContainer.AddChild(new TextPanel(150,30,"Public properties:", 10, false));

            for (int i = 0; i<editor.selectedGameobject.properties.Length; i++)
            {
                PropertyInfo prop = editor.selectedGameobject.properties[i];
                var value = prop.GetValue(editor.selectedGameobject.EditorDisplayObject);
                EditorPropertyInput field = new EditorPropertyInput(prop.PropertyType, prop.Name, value);
                field.onValueChanged += delegate (object val) { EditorActionRegister.SetPropertyValue(val, prop, editor.selectedGameobject);/*  prop.SetValue(editor.selectedGameobject.EditorDisplayObject, val); */};
                ObjectPropertyListContainer.AddChild(field);
            }

            Finalize();
            SelectedObjectPropertyPanel.OrganiseChildrenVertical();

            void Finalize()
            {
                ObjectPropertyListContainer.OrganiseChildrenVertical();
                ObjectPropertyListContainer.ResizeToContent();
                SelectedObjectPropertyPanel.AddChild(ObjectPropertyListContainer);
                SelectedObjectPropertyPanel.SetSliderPosition();
            }
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
            AddObjectMenu = new SliderPanel(170, 400);
            Panel container = new Panel(1,1,invisible:true);
            container.AddChild(new TextPanel(150, 15, "Add object of type:", 10, false));
            foreach (Type typ in gameObjectTypes)
            {
                TextButton txtButton = new TextButton(150, 15, typ.Name, 10);
                container.AddChild(txtButton);
                txtButton.OnRelease += delegate() { CreateTypeConstructorMenu(typ); };
            }
            container.OrganiseChildrenVertical();
            container.ResizeToContent();
            AddObjectMenu.AddChild(container);
            AddObjectMenu.OrganiseChildrenVertical();
            (AddObjectMenu as SliderPanel).SetSliderBar(10,400);
        }

        void CreateHierarchy()
        {
            Panel targetWindow = HierarchyMenu;
            //change 'this' in the line below to whatever the main gameobject is
            //uh huh
            editorDisplay = new HierarchyItem(editor.mainGameObject, 0, (int)targetWindow.width - 30, 5, 5);
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
    }
}
