using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class LevelTransitioner : GameObject
    {
        Sprite transition;
        public LevelTransitioner()
        {
            transition = new Sprite("editor/whitePixel.png");
            transition.width = game.width;
            transition.height = game.height;
            game.uiManager.Add(transition);
            transition.alpha = 0;
        }
        void Update()
        {
            if (inLevelTransitionAnim) AnimateLevelTransition();
        }

        private float timeTransition;
        bool loaded = true;
        bool inLevelTransitionAnim;
        int nextLevel;
        float transitionSpeed;
        int _currentScene = 1;
        public int CurrentScene { get { return _currentScene; } }
        public void Reload()
        {
            LevelTransition(_currentScene, .6f);
        }
        public void LevelTransition(int level, float transitionTime = 2f)
        {
            loaded = false;
            inLevelTransitionAnim = true;
            nextLevel = level;
            transitionSpeed = transitionTime;
        }
        void AnimateLevelTransition()
        {
            timeTransition += Time.deltaTimeS;
            if (loaded)
            {
                transition.alpha = (2 * transitionSpeed - timeTransition) * 1.4f / transitionSpeed;
                if (timeTransition > 2 * transitionSpeed)
                {
                    inLevelTransitionAnim = false;
                    timeTransition = 0;
                }
            }
            else
            {
                transition.alpha = timeTransition * 1.4f / transitionSpeed;
                if (timeTransition > transitionSpeed)
                {
                    (game as Neodymium).loadScene(nextLevel);
                    _currentScene = nextLevel;
                    loaded = true;
                }
            }
            transition.alpha = Mathf.Clamp(transition.alpha, 0, 1);
            SpatialSound.globalVolume = 1 - transition.alpha;
        }
    }
}
