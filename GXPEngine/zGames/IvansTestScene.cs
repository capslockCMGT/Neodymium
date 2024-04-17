using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using static System.Net.Mime.MediaTypeNames;

namespace GXPEngine
{
    internal class IvansTestScene : Game
    {
        bool showCursor;
        Player player;
        Camera cam;
        public IvansTestScene() : base(800, 600, false, true, false, "actual cool test scene")
        {
            cam = new Camera(new ProjectionMatrix(90, 90 * .75f, .1f, 10), true);
            RenderMain = false;
            AddChild(cam);
            cam.SetXY(0, 1, 0);

            SetupScene();

            player = new Player();
            AddChild(player);
            player.AssignCamera(cam);
            Gizmos.GetCameraSpace(cam);
            player.colliders.Add(new Box("cubeTex.png").collider);
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            game.ShowMouse(showCursor);
        }
        public void SetupScene()
        {
            for (int i = GetChildren().Count - 1; i >= 0; i--)
            {
                GameObject child = GetChildren()[i];
                if (!(child is Camera))
                    RemoveChild(child);
            }
            string sceneFolder = "testScene/";

            ModelRenderer floor = new ModelRenderer(sceneFolder + "floor.obj", sceneFolder + "floor_texture.png");
            AddChild(floor);
            floor.y = -2;
            ModelRenderer walls = new ModelRenderer(sceneFolder + "walls.obj", sceneFolder + "walls_texture.png");
            AddChild(walls);
            walls.y = -2;
            ModelRenderer lights = new ModelRenderer(sceneFolder + "lights.obj", "editor/whitePixel.png");
            AddChild(lights);
            lights.y = -2;
            ModelRenderer ceiling = new ModelRenderer(sceneFolder + "ceiling.obj", sceneFolder + "ceiling_texture.png");
            AddChild(ceiling);
            ceiling.y = -2;
            ModelRenderer tube1 = new ModelRenderer(sceneFolder + "tube1.obj", sceneFolder + "tube1_texture.png");
            AddChild(tube1);
            tube1.y = -2;
            ParticleSystem ps = new ParticleSystem(sceneFolder + "smoke.png", 1, 1, 1, ParticleSystem.EmitterType.rect, ParticleSystem.Mode.velocity, worldSpace: this);
            ps.startPos = new Vector3(0.265f, 1.44f - 2f, 2.2f);
            ps.startSpeedDelta = new Vector3(0.001f, 0.001f, 0.001f);
            ps.startSpeed = new Vector3(0.001f, -0.01f, -0.01f);
            ps.endSpeed = Vector3.zero;
            ps.endSpeedDelta = new Vector3(0, 0, 0);
            ps.startSize = 0.0005f;
            ps.endSize = 0.001f;
            ps.startAlpha = 1f;
            ps.endAlpha = 0f;
            ps.startColor = Color.Gray;
            ps.endColor = Color.White;
            ps.enabled = true;
            AddChild(ps);
        }
    }
}
