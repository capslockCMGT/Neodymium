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
        public Editor() : base(1200, 600, false, true, false, "GXP Editor")
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
            Panel test1 = new Panel(30, 50);
            Panel test2 = new Panel(270, 250);
            Panel test3 = new Panel(200, 200);
            uiManager.Add(leftPanel);
            leftPanel.AddChild(test1);
            leftPanel.AddChild(test2);
            leftPanel.AddChild(test3);
            leftPanel.OrganiseChildrenVertical();
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