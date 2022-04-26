using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace XamSvg.XamForms
{
    public static partial class LocationFetcher
    {
        static System.Drawing.PointF InternalGetCoordinates(VisualElement element)
        {
            var renderer = Platform.GetRenderer(element);
            var nativeView = renderer.NativeView;
            var rect = nativeView.Superview.ConvertPointToView(nativeView.Frame.Location, null);
            return new System.Drawing.PointF((float)rect.X, (float)rect.Y);
        }
    }
}