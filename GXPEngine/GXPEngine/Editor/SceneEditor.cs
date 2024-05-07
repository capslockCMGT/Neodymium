using System;
using System.IO;
using System.Reflection;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine.Editor.Exclusives
{
    public class SceneEditor : Game
    {
        EditorCamera _mainCam;
        public EditorCamera mainCam
        {
            get { return _mainCam; }
        }

        EditorGameObject _mainGameObject;
        public EditorGameObject mainGameObject
        {
            get { return _mainGameObject; }
        }

        TransformGizmo _transformGiz;
        public TransformGizmo TransformGiz
        {
            get { return _transformGiz; }
        }

        EditorGameObject _selectedGameObject;
        public EditorGameObject selectedGameobject
        {
            get { return _selectedGameObject; }
            set
            {
                _selectedGameObject = value;
                _uiHandler.UpdateGameObjectPropertyMenu();
            }
        }
        //i cant even be fucked anymore
        public EditorGameObject hoveredObject;

        EditorUIHandler _uiHandler;
        public EditorUIHandler uiHandler
        {
            get { return _uiHandler; }
        }

        EditorGameObject clipboardObject;

        string _loadedScene;
        public string loadedScene { get { return _loadedScene; } }

        bool TryRaycastNextFrame = false;

        public SceneEditor() : base(1200, 600, false, true, true, "GXP Editor")
        {
            SetupCam();
            _uiHandler = new EditorUIHandler();
            _transformGiz = new TransformGizmo();
            AddChild(_transformGiz);
            _mainCam.RenderTarget.onAfterDepthSortedRender += _transformGiz.TryRenderingHarder;

            _uiHandler.SetupMainUI();
        }

        public void LoadScene()
        {
            string newscene = Utils.OpenFile();

            if (!File.Exists(newscene))
            {
                newscene = null;
                return;
            }
            DestroyCurrentTree(delegate ()
            {
                _loadedScene = newscene;
                EditorActionRegister.AddObject(GameObjectReader.ReadEditorGameObjectTree(_loadedScene));
            });
        }

        public void SaveSceneAs()
        {
            _loadedScene = Utils.SaveFile();

            //am i stupid? dont check if a file exists when saving it dumbass
            //if (!File.Exists(_loadedScene)) { _loadedScene = null; return; }

            GameObjectWriter.WriteEditorGameObjectTree(mainGameObject, _loadedScene);
        }

        public void DestroyCurrentTree(UI.Button.NoArgs onComplete)
        {
            _uiHandler.DestroySceneConfirmationDialog(DoItForSure);
            void DoItForSure()
            {
                _mainGameObject?.Destroy();
                _mainGameObject = null;
                selectedGameobject = null;
                uiManager.RemoveAll();
                HierarchyItem.references.Clear();
                EditorActionRegister.Clear();
                _uiHandler.Destroy();
                _uiHandler = new EditorUIHandler();
                _uiHandler.SetupMainUI();
                onComplete?.Invoke();
            }
        }

        public void AddGameObject(ConstructorInfo consInfo)
        {
            _uiHandler.SetActiveSideMenu(null);
            if (consInfo == null) return;

            Type gameObjectType = consInfo.DeclaringType;
            EditorGameObject newObject = new EditorGameObject(gameObjectType, consInfo);
            EditorActionRegister.AddObject(newObject);
        }
        public void AddGameObject(EditorGameObject newObject)
        {
            if (selectedGameobject != null)
            {
                selectedGameobject.AddChild(newObject);
            }
            else if (_mainGameObject == null || !_mainGameObject.InHierarchy())
            {
                _mainGameObject = newObject;
                AddChild(newObject);
            }
            else _mainGameObject.AddChild(newObject);
            selectedGameobject = newObject;
        }

        void Update()
        {
            DrawEditorGizmos();
            Raycast();
            _uiHandler.UpdateHierarchy();
            if (Input.GetKey(Key.LEFT_CTRL) && !InputField.AnyTyping)
            {
                if (Input.GetKeyDown(Key.C) && selectedGameobject != null)
                {
                    clipboardObject?.Destroy();
                    clipboardObject = selectedGameobject.GetDuplicateWithChildren();
                }
                if (Input.GetKeyDown(Key.V) && selectedGameobject != null && clipboardObject != null)
                    EditorActionRegister.AddObject(clipboardObject.GetDuplicateWithChildren(), "Duplicated ");
                if (Input.GetKeyDown(Key.Z))
                    if (Input.GetKey(Key.LEFT_SHIFT))
                        EditorActionRegister.Redo();
                    else EditorActionRegister.Undo();
            }
        }

        void Raycast()
        {
            bool clicked = TryRaycastNextFrame && _uiHandler.millisSinceButtonPressed > 100;
            TryRaycastNextFrame = Input.GetMouseButtonDown(0);

            if (mainGameObject == null) return;

            Vector3 start = _mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 0.001f);
            Vector3 end = _mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 1);

            bool gizInTheWay;
            if (clicked) gizInTheWay = _transformGiz.RaycastOnClick(start, end);
            else gizInTheWay = _transformGiz.TryRaycast(start, end);

            if (gizInTheWay) return;

            raycastResult slop = raycastThroughChildren(mainGameObject, start, end);
            if(clicked) selectedGameobject = slop.hitObject;
            else hoveredObject = slop.hitObject;
        }

        void SetupCam()
        {
            RenderMain = false;
            _mainCam = new EditorCamera();
            _mainCam.position = new Vector3(1, 1, 3);
            AddChild(_mainCam);
        }

        void DrawEditorGizmos()
        {
            Gizmos.DrawLine(0, 0, 0, 1f, 0, 0, this, 0xFFFF0000, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 1f, 0, this, 0xFF00FF00, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 0, 1f, this, 0xFF0000FF, 5);
            for (int i = 0; i < 22; i++)
            {
                //dw abt it
                uint col = i == 5 || i == 16 ? 0xFFFFFFFF : 0xFF777777;
                if (i < 11) Gizmos.DrawLine(-6, 0, i - 5, 6, 0, i - 5, this, col, 1);
                else Gizmos.DrawLine(i - 16, 0, -6, i - 16, 0, 6, this, col, 1);
            }
            if (selectedGameobject != null && typeof(Box).IsAssignableFrom(selectedGameobject.ObjectType))
                Gizmos.DrawBox(0, 0, 0, 2, 2, 2, selectedGameobject, 0xFFFF9900, 8);
            if (hoveredObject != null && typeof(Box).IsAssignableFrom(hoveredObject.ObjectType))
                Gizmos.DrawBox(0, 0, 0, 2, 2, 2, hoveredObject, 0xFFFFFFAA, 8);
        }
        bool yuck = false;
        public override void Add(GameObject gameObject)
        {
            if (gameObject.GetType().Namespace == typeof(SceneEditor).Namespace || yuck)
                base.Add(gameObject);
            yuck = false;
        }
        public static void AddToUpdate(GameObject gameObject)
        {
            if (main is SceneEditor)
            {
                (main as SceneEditor).yuck = true;
                main.Add(gameObject);
            }
        }

        raycastResult raycastThroughChildren(EditorGameObject toCast, Vector3 rayStart, Vector3 rayEnd)
        {
            raycastResult result = new raycastResult();
            result.hitObject = null;
            result.distance = float.MaxValue;
            Vector3 normal;
            foreach (GameObject go in toCast.GetChildren())
            {
                if (go.collider == null) continue;


                if (go is EditorGameObject) result.setIfCloser(raycastThroughChildren((EditorGameObject)go, rayStart, rayEnd));
                else
                {
                    float point = float.MaxValue;
                    bool hit = false;
                    if (go.collider is BoxCollider)
                        hit = ((BoxCollider)go.collider).RayCast(rayStart, rayEnd, out point, out normal);
                    else hit = go.collider.RayCastTest(rayStart, rayEnd);

                    if (hit && toCast != selectedGameobject)
                        result.setIfCloser(toCast, point < float.MaxValue ? point : (go.TransformPoint(0, 0, 0) - rayStart).Magnitude());
                }
            }
            float d = float.MaxValue;
            if (toCast.collider != null && toCast != selectedGameobject)
                ((BoxCollider)toCast.collider).RayCast(rayStart, rayEnd, out d, out normal);
            result.setIfCloser(toCast, d);
            return result;
        }
        struct raycastResult
        {
            public EditorGameObject hitObject;
            public float distance;

            public void setIfCloser(EditorGameObject hitObject, float distance)
            {
                if (this.distance > distance)
                {
                    this.distance = distance;
                    this.hitObject = hitObject;
                }
            }
            public void setIfCloser(raycastResult result)
            {
                setIfCloser(result.hitObject, result.distance);
            }
        }

    }
}