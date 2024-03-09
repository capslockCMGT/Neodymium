using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using GXPEngine.Core;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game {
	Quaternion rotate = new Quaternion(100,-.7f,.1f,.3f);
	Quaternion camRotate;
	EasyDraw canvas;
	EasyDraw slopvas;
	Camera cam;
	GameObject antiSlop;
	Box test;
	bool showCursor;
	public MyGame() : base(800, 600, false, true, false, "ligma")
	{
		rotate.Normalize();

		// Draw some things on a canvas:
		canvas = new EasyDraw(800, 600); 
        canvas.Clear(Color.MediumPurple);
		canvas.Fill(Color.Yellow);
		canvas.Ellipse(width / 2, height / 2, 200, 200);
		canvas.Fill(50);
		canvas.TextSize(32);
		canvas.TextAlign(CenterMode.Center, CenterMode.Center);
		canvas.Text("Welcome!", width / 2, height / 2);
		canvas.SetOrigin(width/2 , height/2);
		//canvas.position = new Vector3(width/2, height/2 , -500);
		//canvas.z = .5f;
		canvas.scale = 1 / 600f;
		// Add the canvas to the engine to display it:
		AddChild(canvas);

		slopvas = new EasyDraw(800, 600);
		slopvas.Clear(50, 100, 255, 255);
		AddChild(slopvas);
		slopvas.SetOrigin(width / 2, height / 2);
        slopvas.scale = 1 / 600f;
		slopvas.rotation = Quaternion.FromEulers(new Vector3(.25f * Mathf.PI, 0, 0));

		canvas.z = -1;
		//slopvas.z = 1f;

		Console.WriteLine("ligma initialized");

		camRotate.Eulers = new Vector3(0,0.0021f,0f);

		antiSlop = new Pivot();
		canvas.AddChild(antiSlop);

		//cam = new Camera(new ProjectionMatrix(new Vector2(4,3), .1f, 10), true);
		cam = new Camera(new ProjectionMatrix(90, 90*.75f, .1f, 10), true);
		RenderMain = false;
		AddChild(cam);

		test = new Box("cubeTex.png");
		test.z = 2;
		test.scale = .5f;
		AddChild(test);
	}

	// For every game object, Update is called every frame, by the engine:
	void Update() {
		canvas.Rotate(camRotate);
        //slopvas.Rotate(rotate);

        //cam.Rotate(camRotate);
        Transformable inv = canvas.Inverse();
		antiSlop.position = inv.position; 
		antiSlop.rotation = inv.rotation;
        antiSlop.scaleXYZ = inv.scaleXYZ;
		float msex = Input.mouseX/800f * Mathf.PI;
		float msey = Input.mouseY/600f * Mathf.PI;
		cam.rotation = Quaternion.FromRotationAroundAxis(0,1,0,msex);
		cam.Rotate(Quaternion.FromRotationAroundAxis(-1, 0, 0, msey));
        //cam.Rotate(Quaternion.FromRotationAroundAxis(cam.TransformDirection(-1, 0, 0), msey));
        cam.position = cam.TransformDirection(0, 0, 3);
		cam.z += 2;
		Gizmos.DrawBox(0,0,0, 150, 50, 150, canvas);
		Gizmos.DrawLine(0, 0, 0, 1f, 0, 0, this, 0xFFFF0000);
		Gizmos.DrawLine(0, 0, 0, 0, 1f, 0, this, 0xFF00FF00);
		Gizmos.DrawLine(0, 0, 0, 0, 0, 1f, this, 0xFF0000FF);

        if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
		game.ShowMouse(showCursor);
    }
    static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}