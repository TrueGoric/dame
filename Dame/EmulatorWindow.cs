using System;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace Dame
{
    class EmulatorWindow
    {
        private uint dpiMultiplier;
        private RenderWindow window;

        public EmulatorWindow(uint dpi)
        {
            dpiMultiplier = dpi;
        }

        public Task RunAsync()
            => Task.Run(Run);

        public void Run()
        {
            var mode = VideoMode.DesktopMode;
            var style = Styles.Default;

            mode.Width = 160 * dpiMultiplier;
            mode.Height = 144 * dpiMultiplier;

            window = new RenderWindow(mode, "Dame", style);
            window.KeyPressed += OnKeyPressed;

            while (window.IsOpen)
            {
                window.WaitAndDispatchEvents();

                window.Display();
            }
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Key.Escape)
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