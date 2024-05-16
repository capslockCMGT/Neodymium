using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.Physics;
using GXPEngine.UI;

namespace GXPEngine
{
    public class Neodymium : Game
    {
        bool showCursor;
        static bool enableControls = false;
        public static bool controlsEnabled { get {  return enableControls; } }
        public RotateAroundLevelCamera Camera;
        GameObject scene;
        MainMenu menu;
        LevelTransitioner transRights;
        Player player;
        Crane crane;
        ControlsTutorial controlsTutorial;
        Panel HUD;
        SoundChannel music;
        public Neodymium() : base(1200, 750, false, gameName:"Neodymium") 
        {
            Camera = new RotateAroundLevelCamera(new Camera(new ProjectionMatrix(80, 50, .1f, 100)));
            AddChild(Camera);

            transRights = new LevelTransitioner();
            AddChild(transRights);
            loadScene(transRights.CurrentScene);

            menu = new MainMenu();
            uiManager.Add(menu);
            //sl.


            music = new Sound("Sounds/Neodymium soundtrack.wav",true,true).Play(volume:.35f);
        }

        public void loadScene(int n)
        {
            scene?.Destroy();
            scene = Editor.GameObjectReader.ReadGameObjectTree("neodymium/Level"+n+".gxp3d");
            AddChild(scene);
            Checkpoint.AddCheckpointsToPlayer(scene.FindObjectOfType<Player>());
            cleanupPhysicsObjects(scene);
            scene.FindObjectOfType<Magnet>()?.DetectAttractable();
            player = scene?.FindObjectOfType<Player>();
            crane = scene?.FindObjectOfType<Crane>();
            if(player != null) player.finished += NextLevel;
        }
        void cleanupPhysicsObjects(GameObject p)
        {
            foreach(GameObject c in p.GetChildren())
            {
                cleanupPhysicsObjects(c);
                if(!(c is PhysicsObject)) continue;
                PhysicsObject obj = (PhysicsObject)c;
                obj.pos = obj.globalPosition;
                obj.prevPos = obj.globalPosition;
            }
        }
        void Update()
        {
            PhysicsObject.UpdateAll();
            if (enableControls)
            {
                //if (Input.GetKeyDown(Key.N))
                //    NextLevel();
                if (Input.GetKeyDown(Key.R))
                    transRights.Reload();
                //if (Input.GetKeyDown(Key.MINUS_UNDERSCORE))
                //    Camera.distance += 3;
                //if (Input.GetKeyDown(Key.EQUALS))
                //    Camera.distance -= 3;
            }
        }
        void NextLevel()
        {
            Console.WriteLine("LOL?");
            transRights.LevelTransition(transRights.CurrentScene + 1);
        }
        public void SetupHud()
        {
            controlsTutorial = new ControlsTutorial();
            game.AddChild(controlsTutorial);

            HUD = new Panel(game.width, game.height, invisible:true);
            uiManager.Add(HUD);
            Panel controls = new Panel("neodymium/buttons/MENU.png");
            controls.scale = 0.2f;
            controls.SetOrigin(controls.width,controls.height);
            controls.position = new Vector3(HUD.width - 20, HUD.height - 20,0);
            HUD.AddChild(controls);
        }
        public void StartGame()
        {
            SetupHud();
            enableControls = true;
            //if (crane != null) crane.enableControls= true;
            //if (player != null) player.enableControls= true;
            controlsTutorial.SetCraneHints(crane);
            controlsTutorial.SetPlayerHints(player); 
            Camera.CamEnabled = true;
        }
    }
}
