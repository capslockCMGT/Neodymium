namespace GXPEngine.UI
{
    public class InputField : Button
    {
        public delegate void TextUpdated(string newText);
        public TextUpdated OnTextChanged = null;
        public TextUpdated OnStoppedTyping = null;
        public string message { get { return displayedText; } set { displayedText = value; UpdateDisplay(); } }
        string displayedText;
        public State state;
        public int cursorPos;
        bool _resetOnClick = false;

        static bool _anyTyping = false;
        public static bool AnyTyping { get { return _anyTyping; } }
        public enum State
        {
            DISPLAY,
            TYPE
        }
        public InputField(int width, int height, bool resetOnClick = false, float x = 0, float y = 0, int fontSize = 15, bool invisible = false) : base(width, height, x, y, invisible)
        {
            TextSize(fontSize);
            TextAlign(CenterMode.Min, CenterMode.Center);
            OnClick += EnableTyping;
            _resetOnClick = resetOnClick;
        }
        public InputField(int width, int height, float x = 0, float y = 0, string defaultText = "", int fontSize = 15, bool invisible = false) : this(width, height, false, x, y, fontSize, invisible)
        {
            message = defaultText;
        }
        public override void Update()
        {
            base.Update();
            switch (state)
            {
                case State.DISPLAY:
                    color = 0xffffffff;
                    break;
                case State.TYPE:
                    color = 0xff99ff99;
                    if (Input.GetMouseButtonDown(0) && !HitTest(Input.mouseX, Input.mouseY) || Input.GetKeyDown(Key.ENTER))
                    {
                        DisableTyping();
                    }
                    if (!Input.AnyKeyDown())
                        break;

                    for (int i = 30; i <= 90; i++)
                    {
                        if (Input.GetKeyDown(i))
                        {
                            char toadd = (char)i;
                            if (!(Input.GetKey(Key.LEFT_SHIFT) || Input.GetKey(Key.RIGHT_SHIFT))) toadd = char.ToLower(toadd);
                            displayedText += toadd;
                        }
                    }
                    if (Input.GetKeyDown(Key.BACKSPACE))
                    {
                        if (displayedText.Length > 0)
                            displayedText = displayedText.Remove(displayedText.Length - 1);
                    }
                    if (Input.GetKeyDown(Key.V) && Input.GetKey(Key.LEFT_CTRL))
                    {
                        displayedText = displayedText.Remove(displayedText.Length - 1);
                        displayedText += Input.GetClipboardText();
                    }
                    UpdateDisplay();

                    break;
            }
        }
        public void EnableTyping()
        {
            if (state == State.TYPE) return;
            state = State.TYPE;
            _anyTyping = true;
            if(_resetOnClick)
                displayedText = string.Empty;
            TextAlign(CenterMode.Max, CenterMode.Center);
            UpdateDisplay();
        }
        public void DisableTyping()
        {
            state = State.DISPLAY;
            _anyTyping = false;
            message = displayedText;
            TextAlign(CenterMode.Min, CenterMode.Center);
            UpdateDisplay();
            OnStoppedTyping?.Invoke(displayedText);
        }
        public void UpdateDisplay()
        {
            ClearTransparent();
            SetupTexture();
            Fill(255);
            Text(displayedText);
            OnTextChanged?.Invoke(displayedText);
        }
    }
}
