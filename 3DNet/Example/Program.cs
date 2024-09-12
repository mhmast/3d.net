//using SharpDX;
//using SharpDX.Direct3D12;

//using SharpDX.Windows;
//using System.Runtime.InteropServices;
//using SharpDX.Direct3D;

//using Device = SharpDX.Direct3D12.Device;
//using SharpDX.DXGI;
//using Resource = SharpDX.Direct3D12.Resource;
////using Vector3 = SharpDX.Vector3;
//using Viewport = SharpDX.Viewport;
//using System;
//using System.Diagnostics;
//using InfoQueue = SharpDX.Direct3D12.InfoQueue;
//using SharpDX.D3DCompiler;
//using System.Threading;
//using Example;

using System;
using System.Diagnostics;
using System.Numerics;
class Program
{
    public static Vector3 ClampEpsilon(Vector3 vector3, float epsilon) => 
        new Vector3(
            vector3.X < epsilon && vector3.X > epsilon * -1 ? 0f : vector3.X, 
            vector3.Y < epsilon && vector3.Y > epsilon * -1 ? 0f : vector3.Y, 
            vector3.Z < epsilon && vector3.Z > epsilon * -1 ? 0f : vector3.Z);

    static void Main()
    {

        Vector3 originalRight = new Vector3(1, 0, 0);
        Vector3 originalForward = new Vector3(0, 0, 1);
        Vector3 originalUp = new Vector3(0, 1, 0);
        var newRight = originalRight;
        var newUp = originalUp;
        var newForward = originalForward;

        for (int i = 0; i < 10; i++)
        {
            var epsilon = 0.000001f;
            var degrees = 90f;
            var rads = degrees * (float)(Math.PI / 180f);
            var rotationQuat = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(newUp, rads));
            newRight = ClampEpsilon(Vector3.Transform(newRight, rotationQuat), epsilon);
            newUp = ClampEpsilon(Vector3.Transform(newUp, rotationQuat), epsilon);
            newForward = ClampEpsilon(Vector3.Transform(newForward, rotationQuat), epsilon);

            degrees = 90f;
            rads = degrees * (float)(Math.PI / 180f);
            rotationQuat = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(newRight, rads));
            newRight = ClampEpsilon(Vector3.Normalize(Vector3.Transform(newRight, rotationQuat)), epsilon);
            newUp = ClampEpsilon(Vector3.Normalize(Vector3.Transform(newUp, rotationQuat)), epsilon);
            newForward = ClampEpsilon(Vector3.Normalize(Vector3.Transform(newForward, rotationQuat)), epsilon);



            degrees = 270f;
            rads = degrees * (float)(Math.PI / 180f);
            rotationQuat = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(newRight, rads));
            newRight = ClampEpsilon(Vector3.Normalize(Vector3.Transform(newRight, rotationQuat)), epsilon);
            newUp = ClampEpsilon(Vector3.Normalize(Vector3.Transform(newUp, rotationQuat)), epsilon);
            newForward = ClampEpsilon(Vector3.Normalize(Vector3.Transform(newForward, rotationQuat)), epsilon);

            degrees = 270f;
            rads = degrees * (float)(Math.PI / 180f);
            rotationQuat = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(newUp, rads));
            newRight = ClampEpsilon(Vector3.Transform(newRight, rotationQuat), epsilon);
            newUp = ClampEpsilon(Vector3.Transform(newUp, rotationQuat), epsilon);
            newForward = ClampEpsilon(Vector3.Transform(newForward, rotationQuat), epsilon);




        }

        Console.WriteLine($"New Right Vector: {newRight}");
        //Console.WriteLine($"New Up Vector: {newUp}");
        //rotationQuat = Quaternion.CreateFromAxisAngle(newRight, rads);

        //Vector3 newRight2 = Vector3.Transform(newRight, rotationQuat);
        //Vector3 newUp2 = Vector3.Transform(newUp, rotationQuat);

        //Console.WriteLine($"New Right Vector: {newRight2}");
        //Console.WriteLine($"New Up Vector: {newUp2}");

        //rotationQuat = Quaternion.CreateFromAxisAngle(newRight2, rads);

        //Vector3 newRight3 = Vector3.Transform(newRight2, rotationQuat);
        //Console.WriteLine($"New Right Vector: {newRight3}");
        //// Output the new right vector

        //var form = new RenderForm("Hello Bundles")
        //{
        //    Width = 1280,
        //    Height = 800
        //};
        //form.Show();

        //using (var app = new HelloBundles())
        //{
        //    app.Initialize(form);

        //    using (var loop = new RenderLoop(form))
        //    {
        //        while (loop.NextFrame())
        //        {
        //            app.Update();
        //            app.Render();
        //        }
        //    }
        //} 
    }
}