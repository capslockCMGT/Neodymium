using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GXPEngine.UI;
using GXPEngine.Core;
using System.Windows.Forms;
using System.IO;
using GXPEngine.Editor;
using System.Drawing;
using System.Threading;

namespace GXPEngine.Editor
{
    public class Editor : Game
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

        EditorUIHandler _uiHandler;
        public EditorUIHandler uiHandler
        {
            get { return _uiHandler; }
        }

        string _loadedScene;
        public string loadedScene { get { return _loadedScene; } }

        bool TryRaycastNextFrame = false;

        public Editor() : base(1200, 600, false, true, true, "GXP Editor")
        { 
            SetupCam();
            _uiHandler = new EditorUIHandler();
            _transformGiz = new TransformGizmo();
            AddChild(_transformGiz);

            _uiHandler.SetupMainUI();
        }

        public void LoadScene()
        {
            _loadedScene = "";
            //quite remarkable
            Thread STAThread = new Thread(
            delegate ()
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = "";
                    ofd.Filter = "GXP3D Scene files (*.GXP3D)|*.gxp3d";
                    ofd.FilterIndex = 1;
                    ofd.Multiselect = false;
                    ofd.RestoreDirectory = true;
                    ofd.Title = "Select a scene to load...";

                    if (ofd.ShowDialog() != DialogResult.OK) return;
                    try { _loadedScene = ofd.FileName.Substring(Directory.GetCurrentDirectory().Length + 1).Replace('\\', '/'); } catch { }
                }
            });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();

            if (!File.Exists(_loadedScene)) { _loadedScene = null; return; }
            DestroyCurrentTree();
            _mainGameObject = GameObjectReader.ReadEditorGameObjectTree(_loadedScene);
            AddChild(_mainGameObject );
            selectedGameobject = _mainGameObject;
        }

        public void SaveSceneAs()
        {
            _loadedScene = "";
            //quite remarkable
            Thread STAThread = new Thread(
            delegate ()
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.InitialDirectory = "";
                    sfd.Filter = "GXP3D Scene files (*.GXP3D)|*.gxp3d";
                    sfd.FilterIndex = 1;
                    sfd.RestoreDirectory = true;
                    sfd.Title = "Save scene as...";

                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    try { _loadedScene = sfd.FileName.Substring(Directory.GetCurrentDirectory().Length + 1).Replace('\\', '/'); } catch { }
                }
            });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
            if (!File.Exists(_loadedScene)) { _loadedScene = null; return; }

            GameObjectWriter.WriteEditorGameObjectTree(mainGameObject, _loadedScene);
        }

        public void DestroyCurrentTree()
        {
            _mainGameObject?.Destroy();
            _mainGameObject = null;
            selectedGameobject = null;
            uiManager.RemoveAll();
            HierarchyItem.references.Clear();
            _uiHandler.Destroy();
            _uiHandler = new EditorUIHandler();
            _uiHandler.SetupMainUI();
        }

        public void AddGameObject(ConstructorInfo consInfo)
        {
            _uiHandler.SetActiveSideMenu(null);
            if (consInfo == null) return;

            Type gameObjectType = consInfo.DeclaringType;
            EditorGameObject newObject = new EditorGameObject(gameObjectType, consInfo);
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
            if(TryRaycastNextFrame && _uiHandler.millisSinceButtonPressed > 100)
            {
                Vector3 start = _mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 0.001f);
                Vector3 end = _mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 1);
                
                if(mainGameObject != null && !_transformGiz.RaycastOnClick(start, end))
                {
                    raycastResult slop = raycastThroughChildren(mainGameObject, start, end);
                    selectedGameobject = slop.hitObject;
                }
            }
            TryRaycastNextFrame = Input.GetMouseButtonDown(0);
            _uiHandler.UpdateHierarchy();
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
        }
        public override void Render(GLContext glContext)
        {
            base.Render(glContext);
            //are ya sinning son?
            if(!recursingRn)TransformGiz.TryRenderingHarder(glContext);
        }

        public override void Add(GameObject gameObject)
        {
            if(gameObject.GetType().Namespace == typeof(Editor).Namespace)
            base.Add(gameObject);
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