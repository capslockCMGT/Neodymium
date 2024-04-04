﻿using GXPEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class InputField : Button
    {
        public string message;
        string displayedText;
        public State state;
        public int cursorPos;
        public enum State
        {
            DISPLAY,
            TYPE
        }
        public InputField(int width, int height, float x = 0, float y = 0, int fontSize = 15, bool invisible = false) : base(width, height, x, y, invisible)
        {
            TextSize(fontSize);
            TextAlign(CenterMode.Center, CenterMode.Center);
            OnClick += EnableTyping;
        }
        public override void Update()
        {
            base.Update();
            switch (state){
                case State.DISPLAY:
                    color = 0xffffffff;
                    break;
                case State.TYPE:
                    color = 0xff99ff99;
                    if (Input.GetMouseButtonDown(0) && !HitTest(Input.mouseX, Input.mouseY) || Input.GetKeyDown(Key.ENTER))
                    {
                        DisableTyping();
                    }
                    if (Input.AnyKeyDown())
                    {
                        for (int i=30; i<=90; i++)
                        {
                            if (Input.GetKeyDown(i))
                                displayedText += (char)i;
                        }
                        if (Input.GetKeyDown(Key.BACKSPACE))
                        {
                            if (displayedText.Length > 0)
                                displayedText = displayedText.Remove(displayedText.Length - 1);
                        }
                        UpdateDisplay();
                    }
                    break;
            }
        }
        public void EnableTyping()
        {
            if (state == State.TYPE) return;
            state = State.TYPE;
            displayedText = string.Empty;
            UpdateDisplay();
        }
        public void DisableTyping()
        {
            state = State.DISPLAY;
            message = displayedText;
            UpdateDisplay();
        }
        public void UpdateDisplay()
        {
            ClearTransparent();
            SetupTexture();
            Fill(255);
            Text(displayedText);
        }
    }
}