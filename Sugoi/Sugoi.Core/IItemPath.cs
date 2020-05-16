namespace Sugoi.Core
{
    public interface IItemPath
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        int MaximumFrame
        {
            get;
        }

        int DirectionX
        {
            get;
        }

        int DirectionY
        {
            get;
        }

        void GetPosition(int currentFrame, out int offsetX, out int offsetY);
    }
}