﻿using StereoKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StereoKitApp
{
    public class StereoApp
    {
        public SKSettings Settings => new SKSettings
        {
            appName = "BASTA Demo",
            assetsFolder = "Assets",
            displayPreference = DisplayMode.MixedReality
        };

        public class BoxData
        {
            public Pose Pose;
            public float Speed;
            public Model Model;
        }

        BoxData[] boxData;

        Pose menuPose = new Pose(0.5f, 0, -0.5f, Quat.LookDir(-1, 0, 1));
        Model basebox;
        Matrix floorTransform = Matrix.TS(new Vec3(0, -1.5f, 0), new Vec3(30, 0.1f, 30));
        Material floorMaterial;
        Solid floorSolid;
        Vec3 moveDir = new Vec3(0f, 0f, 0.1f);
        bool playing = false;
        static Random rand = new Random();
        int points;

        public void Init()
        {
            var mesh = Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f);
            basebox = new Model(mesh, Material.Default);
            boxData = new BoxData[3];
            boxData[0] = new BoxData
            {
                Model = basebox.Copy()
            };
            UpdateColor(new Color(1f, 0f, 0f, 0.5f), boxData[0]);
            boxData[1] = new BoxData
            {
                Model = basebox.Copy()
            };
            UpdateColor(new Color(0f, 1f, 0f, 0.5f), boxData[1]);
            boxData[2] = new BoxData
            {
                Model = basebox.Copy()
            };
            UpdateColor(new Color(0f, 0f, 1f, 0.5f), boxData[2]);

            foreach (var item in boxData)
            {
                ResetBox(item);
            }


            floorSolid = new Solid(World.HasBounds ? World.BoundsPose.position : new Vec3(0, -1.5f, 0), Quat.Identity, SolidType.Immovable);
            floorSolid.AddBox(new Vec3(20, .01f, 20));

            floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;

            Input.HandSolid(Handed.Max, false);
        }

        private static void ResetBox(BoxData item)
        {
            item.Pose = new Pose((float)rand.NextDouble() * .5f - .25f, (float)rand.NextDouble() * .5f - .25f, -1f, Quat.Identity);
            item.Speed = (float)rand.NextDouble() * 10f;
        }

        private static void UpdateColor(Color clr, BoxData boxData1)
        {
            foreach (var node in boxData1.Model.Nodes)
            {
                node.Material = Default.Material.Copy();
                node.Material.Transparency = Transparency.Blend;
                node.Material.SetColor("color", clr);
            }
        }

        public void Step()
        {
            if (SK.System.displayType == Display.Opaque)
                Default.MeshCube.Draw(floorMaterial, floorTransform);

            foreach (var item in boxData)
            {
                Renderer.Add(item.Model, item.Pose.ToMatrix(1f), Color.White);
            }
            if (playing)
            {
                Debug.WriteLine($"Bounds:{boxData[0].Model.Bounds}");
                foreach (var item in boxData)
                {
                    if (item.Pose.position.z > Input.Head.position.z)
                    {
                        ResetBox(item);
                        points--;
                        Sound.Unclick.Play(item.Pose.position);
                        continue;
                    }
                    for (int i = 0; i < (int)Handed.Max; i++)
                    {
                        Hand h = Input.Hand((Handed)i);
                        if (!h.IsTracked) continue;
                        if (item.Model.Bounds.Contains(h.palm.position - item.Pose.position, h.Get(FingerId.Index, JointId.Tip).position - item.Pose.position))
                        {
                            ResetBox(item);
                            points++;

                            Sound.Click.Play(item.Pose.position);
                            continue;
                        }
                    }
                    item.Pose.position += moveDir * Time.Elapsedf * item.Speed;
                }
            }


            UI.WindowBegin("Menu", ref menuPose);
            UI.Label($"Punkte:{points}");
            if (UI.Button("Start"))
            {
                points = 0;
                playing = true;
            }
            if (UI.Button("Stop"))
            {
                playing = false;
            }
            UI.WindowEnd();
        }
    }
}