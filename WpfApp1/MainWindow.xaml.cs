using Akka.Actor;
using OpenCvSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public class Image
        {
            public readonly Mat image;
            public Image(Mat image)
            {
                this.image = image;
            }
        }

        public class ViewModel
        {
            public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();
            public ReactiveProperty<Mat> Image { get; } = new ReactiveProperty<Mat>();
        }

        public class DemoActor : ReceiveActor
        {
            public ViewModel vm;

            public DemoActor(ViewModel vm)
            {
                this.vm = vm;
                Receive<string>(PrintString);
            }

            private void PrintString(string str)
            {
                vm.Message.Value = str;
            }
        }

        private IActorRef actor;

        public MainWindow()
        {
            InitializeComponent();

            var vm = new ViewModel();
            this.DataContext = vm;

            ActorSystem system = ActorSystem.Create("WpfApp1");
            actor = system.ActorOf(Props.Create<DemoActor>(vm), "demo_actor");

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 2,
                MinorVersion = 1
            };
            OpenTkControl.Start(settings);

            IObservable<RoutedEventArgs> observable = Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => (s, e) => h(e),
                h => button.Click += h,
                h => button.Click -= h);

            _ = observable.Subscribe(onNext: Button_Click_method);

            image.CvImageSubscribe(onNext: Color2Gray);
        }

        private void Button_Click_method(RoutedEventArgs e)
        {
            Mat m = new Mat("lena.png");

            actor.Tell(new Image(m));
            actor.Tell("Hello");
        }

        private void Color2Gray(Mat color)
        {
            Mat gray_image = new Mat();
            Cv2.CvtColor(color, gray_image, ColorConversionCodes.BGR2GRAY);

            gray.CvImage = gray_image;
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            var hue = 0.15f;
            var c = Color4.FromHsv(new Vector4(hue, 0.75f, 0.75f, 1));
            GL.ClearColor(c);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            GL.Begin(PrimitiveType.Triangles);

            GL.Color4(Color4.Red);
            GL.Vertex2(0.0f, 0.5f);

            GL.Color4(Color4.Green);
            GL.Vertex2(0.58f, -0.5f);

            GL.Color4(Color4.Blue);
            GL.Vertex2(-0.58f, -0.5f);

            GL.End();
            GL.Finish();
        }
    }
}
