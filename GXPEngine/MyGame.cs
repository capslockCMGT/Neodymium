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
	Box test, test2;
	bool showCursor;
	Vector3 dir = new Vector3(1,0,0);
	
	int framesRotatedCube= 0;
	Quaternion cubeRotate = Quaternion.FromEulers(new Vector3(.01f, 0, 0));
	Quaternion cubeRotation = Quaternion.Identity;
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


        test2 = new Box("cubeTex.png");
        test = new Box("cubeTex.png");

        test.z = 2;
        test2.z = 2;
        
		test.scale = .5f;
        test2.scale = .5f;
        test.Rotate(new Quaternion(0.5709415f, 0.1675188f, 0.5709415f, 0.5656758f));
        AddChild(test);
		AddChild(test2);

		ModelRenderer sloppersludge = new ModelRenderer("editor/monki.obj", "editor/whitePixel.png");
		AddChild(sloppersludge);
		sloppersludge.x = 2;
		sloppersludge.scale = .2f;
    }

	// For every game object, Update is called every frame, by the engine:
	void Update() {
		canvas.Rotate(camRotate);
		//slopvas.Rotate(rotate);
        slopvas.rotation = Quaternion.LookTowards(slopvas.TransformPoint(0, .01f, 0) - cam.TransformPoint(0,0, 0), new Vector3(0, 1, 0));
        
        //cam.Rotate(camRotate);
        Transformable inv = canvas.Inverse();
		antiSlop.position = inv.position; 
		antiSlop.rotation = inv.rotation;
        antiSlop.scaleXYZ = inv.scaleXYZ;

		FirstPersonViewUpdate();
		Gizmos.DrawBox(0,0,0, 150, 50, 150, canvas);
		Gizmos.DrawLine(0, 0, 0, 1f, 0, 0, this, 0xFFFF0000);
		Gizmos.DrawLine(0, 0, 0, 0, 1f, 0, this, 0xFF00FF00);
		Gizmos.DrawLine(0, 0, 0, 0, 0, 1f, this, 0xFF0000FF);

		framesRotatedCube++;
		if(framesRotatedCube == 360)
		{
			switch(Utils.Random(0,6))
            {
                case 0:
                    cubeRotate = Quaternion.FromEulers(new Vector3(.0025f, 0, 0));
                    break;
                case 1:
                    cubeRotate = Quaternion.FromEulers(new Vector3(0, .0025f, 0));
                    break;
                case 2:
                    cubeRotate = Quaternion.FromEulers(new Vector3(0, 0, .0025f));
                    break;
                case 3:
                    cubeRotate = Quaternion.FromEulers(new Vector3(-.0025f, 0, 0));
                    break;
                case 4:
                    cubeRotate = Quaternion.FromEulers(new Vector3(0, -.0025f, 0));
                    break;
                case 5:
                    cubeRotate = Quaternion.FromEulers(new Vector3(0, 0, -.0025f));
                    break;
            }
			framesRotatedCube = 0;
		}
		cubeRotation = cubeRotation * cubeRotate;
		if (Time.time > 1000)
		{
			Console.WriteLine(test.rotation);
			test.rotation = Quaternion.SLerp(test.rotation, cubeRotation, Time.deltaTimeS*.7f);
		}
		

        if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
		game.ShowMouse(showCursor);

		if (Input.GetKeyDown(Key.E)) test2.DisplayExtents();
        if (Input.GetKeyDown(Key.Q)) test.DisplayExtents();
        if (Input.GetKey(Key.R)) dir = cam.position;
        if (Input.GetKey(Key.L)) test2.Move(Time.deltaTimeS, 0, 0);
        if (Input.GetKey(Key.J)) test2.Move(-Time.deltaTimeS, 0, 0);
        if (Input.GetKey(Key.I)) test2.Move(0, Time.deltaTimeS, 0);
        if (Input.GetKey(Key.K)) test2.Move(0, -Time.deltaTimeS, 0);

		Gizmos.DrawLine(dir.x, dir.y, dir.z, 0, 0, 0);

        if (test.collider.GetCollisionInfo(test2.collider) != null)
			Console.WriteLine("COLLIDED!! RAARR");
    }
	public void FirstPersonViewUpdate()
	{
        float msex = Input.mouseX / 800f * Mathf.PI;
        float msey = Input.mouseY / 600f * Mathf.PI;
        cam.rotation = Quaternion.FromRotationAroundAxis(0, 1, 0, msex);
        cam.Rotate(Quaternion.FromRotationAroundAxis(1, 0, 0, msey));
		//cam.Rotate(Quaternion.FromRotationAroundAxis(cam.TransformDirection(-1, 0, 0), msey));

		//minecraft creative mode controls
		if (Input.GetKey(Key.D))
			cam.Move(Time.deltaTimeS, 0, 0);
        if (Input.GetKey(Key.A))
            cam.Move(-Time.deltaTimeS, 0, 0);
		if (Input.GetKey(Key.W))
        {
            Vector3 delta = cam.TransformDirection(0, 0, 1);
            delta.y = 0;
            delta = delta.normalized() * (Time.deltaTimeS);
            cam.position -= delta;
        }
        if (Input.GetKey(Key.S))
		{
			Vector3 delta = cam.TransformDirection(0, 0, 1);
			delta.y = 0;
			delta = delta.normalized() * (Time.deltaTimeS);
			cam.position += delta;
        }
        if (Input.GetKey(Key.LEFT_SHIFT))
        {
            Vector3 delta = cam.TransformDirection(0, 1, 0);
            delta.x = 0; delta.z = 0;
			delta = delta.normalized() * (Time.deltaTimeS);
            cam.position -= delta;
        }
        if (Input.GetKey(Key.SPACE))
        {
            Vector3 delta = cam.TransformDirection(0, 1, 0);
            delta.x = 0; delta.z = 0;
			delta = delta.normalized() * (Time.deltaTimeS);
            cam.position += delta;
        }
    }
    static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}