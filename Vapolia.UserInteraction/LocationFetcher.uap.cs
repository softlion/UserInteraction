using Windows.UI.Xaml;
using Xamarin.Forms;

namespace Vapolia.UserInteraction
{
    public static partial class LocationFetcher
    {
        static System.Drawing.PointF InternalGetCoordinates(VisualElement element)
        {
            var renderer = Xamarin.Forms.Platform.UWP.Platform.GetRenderer(element);
            var nativeView = renderer.GetNativeElement();
            var elementVisualRelative = nativeView.TransformToVisual(Window.Current.Content);
            var point = elementVisualRelative.TransformPoint(new Windows.Foundation.Point(0, 0));
            return new System.Drawing.PointF((float)point.X, (float)point.Y);
        }
    }
}