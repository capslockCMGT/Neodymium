using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing; // For Font
using System.Drawing.Text; // For PrivateFontCollection
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace GXPEngine
{
    /// <summary>
    /// The Utils class contains a number of useful functions.
    /// </summary>
	public static class Utils
	{
		static private Random random = new Random();

		static Dictionary<string, PrivateFontCollection> fontIndex=null;

		/// <summary>
		/// Creates a font from a font file (extension: ttf), with the given point size and font style.
		/// </summary>
		/// <param name="filename">The font file (should be of type .ttf)</param>
		/// <param name="fontSize">The size in points</param>
		/// <param name="fontStyle">The font style (pass e.g. FontStyle.Italic|FontStyle.Bold here)</param>
		/// <returns></returns>
		public static Font LoadFont(string filename, float fontSize, FontStyle fontStyle = FontStyle.Regular) {
			if (fontIndex==null) {
				fontIndex=new Dictionary<string, PrivateFontCollection>();
			}
			if (!fontIndex.ContainsKey(filename)) {
				fontIndex[filename]=new PrivateFontCollection();
				fontIndex[filename].AddFontFile(filename);
				//Console.WriteLine("Loaded new font: "+fontIndex[filename].Families[0]);
			} 
			return new Font(fontIndex[filename].Families[0], fontSize, fontStyle);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														CalculateFrameRate()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the current frame rate in frames per second.
		/// Deprecated use game.fps instead!
		/// </summary>
		public static int frameRate {
			get {
				return Game.main.currentFps;
			}
		}		
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Random()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a random value between the specified min (inclusive) and max (exclusive).
		/// If you want to receive an integer value, use two integers as parameters to this function.
		/// </summary>
		/// <param name='min'>
		/// Inclusive minimum value: lowest possible random value.
		/// </param>
		/// <param name='max'>
		/// Exclusive maximum value: the returned value will be smaller than this value.
		/// </param>
		public static int Random (int min, int max) {
			return random.Next(min, max);
		}

        public static float Random (float min, float max) {
			return (float)(random.NextDouble() * (max - min) + min);
        }
        public static Vector3 Random(Vector3 pos, Vector3 delta)
        {
			return new Vector3(
					Random(pos.x - delta.x, pos.x + delta.x),
                    Random(pos.y - delta.y, pos.y + delta.y),
                    Random(pos.z - delta.z, pos.z + delta.z));
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														print()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Shows output on the console window.
        /// Basically, a shortcut for Console.WriteLine() that allows for multiple parameters.
        /// </summary>
        public static void print(params object[] list) {
			for (int i = 0; i < list.Length; i++) {
				if (list[i] != null) Console.Write(list[i].ToString() + " "); else Console.Write("null ");
			}
			Console.WriteLine();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														RectsOverlap()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns 'true' if the two specified rectangles overlap or 'false' otherwise.
		/// </summary>
		/// <param name='x1'>
		/// The x position of the first rectangle.
		/// </param>
		/// <param name='y1'>
		/// The y position of the first rectangle.
		/// </param>
		/// <param name='width1'>
		/// The width of the first rectangle.
		/// </param>
		/// <param name='height1'>
		/// The height of the first rectangle.
		/// </param>
		/// <param name='x2'>
		/// The x position of the second rectangle.
		/// </param>
		/// <param name='y2'>
		/// The y position of the second rectangle.
		/// </param>
		/// <param name='width2'>
		/// The width of the second rectangle.
		/// </param>
		/// <param name='height2'>
		/// The height of the second rectangle.
		/// </param>
		public static bool RectsOverlap(float x1, float y1, float width1, float height1,
		                                float x2, float y2, float width2, float height2) {
			if (x1 > x2 + width2) return false;
			if (y1 > y2 + height2) return false;
			if (x2 > x1 + width1) return false;
			if (y2 > y1 + height1) return false;
			return true;
		}

		public static string OpenFile(string windowName = "Select a scene to load...", string fileTypes = "GXP3D Scene files (*.GXP3D)|*.gxp3d")
		{
			string res = "";
            Thread STAThread = new Thread(
            delegate ()
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = "";
                    ofd.Filter = fileTypes;
                    ofd.FilterIndex = 1;
                    ofd.Multiselect = false;
                    ofd.RestoreDirectory = true;
                    ofd.Title = windowName;

                    if (ofd.ShowDialog() != DialogResult.OK) return;
                    try { res = ofd.FileName.Substring(Directory.GetCurrentDirectory().Length + 1).Replace('\\', '/'); } catch { }
                }
            });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
			return res;
        }

		public static string SaveFile(string windowName = "Save scene as...", string fileTypes = "GXP3D Scene files (*.GXP3D)|*.gxp3d")
		{
			string res = "";
            Thread STAThread = new Thread(
            delegate ()
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.InitialDirectory = "";
                    sfd.Filter = fileTypes;
                    sfd.FilterIndex = 1;
                    sfd.RestoreDirectory = true;
                    sfd.Title = windowName;

                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    try { res = sfd.FileName.Substring(Directory.GetCurrentDirectory().Length + 1).Replace('\\', '/'); } catch { }
                }
            });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
			return res;
        }
	}
}


