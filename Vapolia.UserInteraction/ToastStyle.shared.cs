namespace Vapolia.UserInteraction
{
    public enum ToastStyle
    {
        Custom,
        Info,
        Notice,
        Warning,
        Error
    }

    public static class Constants
    {
        public static Color[] ToastStyleBackgroundTint =
        {
            null,
            null,
            null,
            Colors.Orange,
            Colors.Red, 
        };
    }
}
