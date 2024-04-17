using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine
{
    public class GameStartMenu : Game
    {
        Type[] games;
        public GameStartMenu() : base(400, 600, false, true, false, "pick game to start...")
        {
            AddChild(new Camera(new ProjectionMatrix(new Vector2(width, height), 0, 1)));
            games = GetGames();
            SliderPanel panel = new SliderPanel(width, height);
            uiManager.Add(panel);
            Panel p = new Panel(1, 1);
            panel.AddChild(p);
            for (int i = 0; i < games.Length; i++)
            {
                var game = games[i];
                IndexedInputField button = new IndexedInputField(i, width - 20, 20, 0, 0, game.Name);
                p.AddChild(button);
                button.IndexedOnTextChanged += StartGame;
            } 
            p.OrganiseChildrenVertical();
            p.ResizeToContent();
            panel.SetSliderBar(20, panel.height);
        }

        void StartGame(int index, string t)
        {
            DestroyRetainProgram();
            Console.WriteLine(t);
            games[index].GetConstructor(new Type[0]).Invoke(new object[0]);
            main.Start();
        }
        public static Type[] GetGames()
        {
            var type = typeof(Game);
            var assembly = type.Assembly;
            return assembly.GetTypes().Where(testc =>
            (testc.IsSubclassOf(type)) && testc.Name != typeof(GameStartMenu).Name).ToArray();
        }
    }
}
