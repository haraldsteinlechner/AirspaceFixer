using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aardvark.Base;
using Aardvark.Base.Incremental.CSharp;
using Aardvark.SceneGraph;
using System.Windows;
using Aardvark.SceneGraph.CSharp;
using Aardvark.Base.Rendering;
using Effects = Aardvark.Base.Rendering.Effects;
using System.Windows.Forms;

namespace AirspaceFixerSample
{

    public class GLInh : OpenTK.GLControl
    {
        public GLInh() : base() { }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OpenTK.Graphics.OpenGL4.GL.ClearColor(System.Drawing.Color.Blue);
            OpenTK.Graphics.OpenGL4.GL.Clear(OpenTK.Graphics.OpenGL4.ClearBufferMask.ColorBufferBit);
            base.SwapBuffers();
            Console.WriteLine("aJSDF");
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Aardvark.Application.WinForms.OpenGlApplication app;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Aardvark.Base.Aardvark.Init();

            var useAardvark = true;
            if (useAardvark)
            {
                app = new Aardvark.Application.WinForms.OpenGlApplication();

                app.Initialize(rc, 1);

                var renderControl = rc;

                var cone = IndexedGeometryPrimitives.solidCone(V3d.OOO, V3d.OOI, 1.0, 0.2, 12, C4b.Red).ToSg(); // build object from indexgeometry primitives
                var cube = SgPrimitives.Sg.box(Mod.Init(C4b.Blue), Mod.Init(Box3d.Unit)); // or directly using scene graph
                var initialViewTrafo = CameraViewModule.lookAt(V3d.III * 3.0, V3d.OOO, V3d.OOI);
                var controlledViewTrafo = Aardvark.Application.DefaultCameraController.control(renderControl.Mouse, renderControl.Keyboard,
                        renderControl.Time, initialViewTrafo);
                var frustum = renderControl.Sizes.Select(size => FrustumModule.perspective(60.0, 0.1, 10.0, size.X / (float)size.Y));

                var whiteShader = Aardvark.Base.Rendering.Effects.SimpleLighting.Effect;
                var trafo = Effects.Trafo.Effect;

                var currentAngle = 0.0;
                var angle = renderControl.Time.Select(t =>
                {
                    if (true)
                    {
                        return currentAngle += 0.001;
                    }
                    else return currentAngle;
                });
                var rotatingTrafo = angle.Select((whyCantShadowName) => Trafo3d.RotationZ(whyCantShadowName));

                var sg =
                    new[] { cone, cube.Trafo(rotatingTrafo) }
                    .ToSg()
                    .WithEffects(new[] { trafo, whiteShader })
                    .ViewTrafo(controlledViewTrafo.Select(c => c.ViewTrafo))
                    .ProjTrafo(frustum.Select(f => f.ProjTrafo()));

                renderControl.RenderTask =
                        Aardvark.Base.RenderTask.ofArray(
                                new[] {
                                app.Runtime.CompileClear(renderControl.FramebufferSignature, Mod.Init(C4f.Red)),
                                app.Runtime.CompileRender(renderControl.FramebufferSignature,sg)
                                 }
                         );

                //var r = new System.Windows.Forms.Form();
                //r.Controls.Add(renderControl);
                //r.Show();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Browser.Navigate("http://www.google.com");
            //rc.Show();
            //rc.RenderTask.Run(null, null);

        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            //if (Browser != null && Browser.CanGoForward)
            //    Browser.GoForward();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //if (Browser != null && Browser.CanGoBack)
            //    Browser.GoBack();
        }

        private void Browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;
        }

        private void txtUrl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Browser.Navigate(txtUrl.Text);
        }
    }
}
