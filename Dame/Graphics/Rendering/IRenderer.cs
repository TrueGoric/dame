namespace Dame.Graphics.Rendering
{
    interface IRenderer
    {
        IRenderContext RenderContext { get; }

        void Render();
    }
}