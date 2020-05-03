using System;
using System.Threading.Tasks;
using Dame.Renderer;
using SFML.Graphics;
using SFML.Window;
using static Dame.Renderer.RenderingConstants;

namespace Dame
{
    class EmulatorWindow
    {
        private uint dpiMultiplier;
        private RenderWindow window;
        private SFMLRenderer emulatorRenderer;

        public SFMLRenderer EmulatorRenderer => emulatorRenderer;

        public EmulatorWindow(uint dpi)
        {
            dpiMultiplier = dpi;
            emulatorRenderer = new SFMLRenderer();
        }

        public Task RunAsync()
            => Task.Run(Run);

        public void Run()
        {
            var mode = VideoMode.DesktopMode;
            var style = Styles.Default;

            mode.Width = BackbufferWidth * dpiMultiplier;
            mode.Height = BackbufferHeight * dpiMultiplier;

            window = new RenderWindow(mode, "Dame", style);
            window.KeyPressed += OnKeyPressed;

            while (window.IsOpen)
            {
                window.WaitAndDispatchEvents();

                window.Clear(Color.White);

                window.Draw(emulatorRenderer);

                window.Display();
            }
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
        }
    }

    public class Tile : Transformable, Drawable
    {
        public void Draw(RenderTarget target, RenderStates states)
        {
            throw new NotImplementedException();
        }
    }
}