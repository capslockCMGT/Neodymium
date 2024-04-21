using System;
using System.Threading;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// The Input class contains functions for reading keys and mouse
	/// </summary>
	public class Input
	{
		/// <summary>
		/// Returns 'true' if given key is down, else returns 'false'
		/// </summary>
		/// <param name='key'>
		/// Key number, use Key.KEYNAME or integer value.
		/// </param>
		public static bool GetKey(int key) {
			return GLContext.GetKey(key);
		}
		
		/// <summary>
		/// Returns 'true' if specified key was pressed down during the current frame
		/// </summary>
		/// <param name='key'>
		/// Key number, use Key.KEYNAME or integer value.
		/// </param>
		public static bool GetKeyDown(int key) {
			return GLContext.GetKeyDown(key);
		}
		
		/// <summary>
		/// Returns 'true' if specified key was released during the current frame
		/// </summary>
		/// <param name='key'>
		/// Key number, use Key.KEYNAME or integer value.
		/// </param>
		public static bool GetKeyUp(int key) {
			return GLContext.GetKeyUp(key);
		}
		
		/// <summary>
		/// Returns true if any key is currently pressed.
		/// </summary>
		public static bool AnyKey() {
			return GLContext.AnyKey();
		}

		/// <summary>
		/// Returns true if any key was pressed down during the current frame.
		/// </summary>
		public static bool AnyKeyDown() {
			return GLContext.AnyKeyDown();
		}

		/// <summary>
		/// Returns 'true' if mousebutton is down, else returns 'false'
		/// </summary>
		/// <param name='button'>
		/// Number of button:
		/// 0 = left button
		/// 1 = right button
		/// 2 = middle button
		/// </param>
		public static bool GetMouseButton(int button) {
			return GLContext.GetMouseButton(button);
		}
		
		/// <summary>
		/// Returns 'true' if specified mousebutton was pressed down during the current frame
		/// </summary>
		/// <param name='button'>
		/// Number of button:
		/// 0 = left button
		/// 1 = right button
		/// 2 = middle button
		/// </param>
		public static bool GetMouseButtonDown(int button) {
			return GLContext.GetMouseButtonDown(button);
		}

		/// <summary>
		/// Returns 'true' if specified mousebutton was released during the current frame
		/// </summary>
		/// <param name='button'>
		/// Number of button:
		/// 0 = left button
		/// 1 = right button
		/// 2 = middle button
		/// </param>
		public static bool GetMouseButtonUp(int button)
		{
			return GLContext.GetMouseButtonUp(button); /*courtesy of LeonB*/
		}
		
		/// <summary>
		/// Gets the current mouse x position in pixels.
		/// </summary>
		public static int mouseX {
			get { return GLContext.mouseX; }
		}
		
		/// <summary>
		/// Gets the current mouse y position in pixels.
		/// </summary>
		public static int mouseY {
			get { return GLContext.mouseY; }
		}

		public static Vector2 mousePos
		{
			get { return new Vector2(mouseX, mouseY); }
		}
		public static Vector2 prevMousePos
		{
			get { return new Vector2(GLContext.prevMouseX, GLContext.prevMouseY); }
		}
		public static Vector2 mouseVelocity
		{
			get { return new Vector2(mouseX - GLContext.prevMouseX, mouseY - GLContext.prevMouseY); }
		}
        public static string GetClipboardText()
        {
            //thank you stackoverflow: https://github.com/slyd0g/SharpClipboard/blob/master/SharpClipboard/Program.cs
            string ReturnValue = string.Empty;
            Thread STAThread = new Thread(
                delegate ()
                {
                    ReturnValue = System.Windows.Forms.Clipboard.GetText();
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();

            return ReturnValue;
        }
    }
}