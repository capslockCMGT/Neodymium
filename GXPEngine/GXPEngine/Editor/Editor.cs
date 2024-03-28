using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.UI;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class Editor : Game
    {
        EditorCamera mainCam;
        public Editor() : base(1200, 600, false, true, true, "GXP Editor")
        { 
            SetupCam();
            SetupUI();
        }

        void Update()
        {
            DrawEditorGrid();
        }

        void SetupCam()
        {
            RenderMain = false;
            mainCam = new EditorCamera();
            mainCam.position = new Vector3(1, 1, 3);
            AddChild(mainCam);
        }

        void SetupUI()
        {
            Panel leftPanel = new Panel(300,height);
            Panel buttonHolder = new Panel(1, 1, invisible: true);
            buttonHolder.scale = 3;
            buttonHolder.AddChild(new TexturedButton("editor/buttons/AddObject.png", "editor/buttons/AddObjectHover.png", "editor/buttons/AddObjectClick.png"));
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