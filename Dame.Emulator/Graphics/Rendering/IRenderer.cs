namespace Dame.Graphics.Rendering
{
    public interface IRenderer
    {
        IRenderContext RenderContext { get; }

        void Render();
    }
}