using StereoKit;

namespace SKTemplate_Multi;

class Program
{
	static void Main(string[] args)
	{

		var app = new StereoKitApp.App();
		// Initialize StereoKit
		// SKSettings settings = new SKSettings
		// {
        //     appName = "BASTA Demo",
        //     assetsFolder = "Assets",
        //     displayPreference = DisplayMode.MixedReality
		// };
		if (!SK.Initialize(app.Settings))
			return;
		app.Init();


		// Create assets used by the app
		// Pose  cubePose = new Pose(0, 0, -0.5f);
		// Model cube     = Model.FromMesh(
		// 	Mesh.GenerateRoundedCube(Vec3.One*0.1f, 0.02f),
		// 	Material.UI);

		// Matrix   floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
		// Material floorMaterial  = new Material("floor.hlsl");
		// floorMaterial.Transparency = Transparency.Blend;


		// Core application loop
		SK.Run(app.Step);
	}
}