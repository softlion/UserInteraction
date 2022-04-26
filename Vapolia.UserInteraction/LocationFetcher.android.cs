using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
namespace XamSvg.XamForms
{
    public static partial class LocationFetcher
    {
            static System.Drawing.PointF InternalGetCoordinates(VisualElement element)
            {
                var renderer = Platform.GetRenderer(element);
                var nativeView = renderer.View;
                var density = nativeView.Context!.Resources!.DisplayMetrics!.Density;

                var locationWindow = new int[2];
                nativeView.GetLocationInWindow(locationWindow);
                var locationOfRootWindow = new int[2];
                nativeView.RootView.FindViewById(Android.Resource.Id.Content).GetLocationInWindow(locationOfRootWindow);
            
                return new System.Drawing.PointF(locationWindow[0] / density, (locationWindow[1]-locationOfRootWindow[1]) / density);
            }
    }
}