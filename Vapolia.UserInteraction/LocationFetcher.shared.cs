using System;
using System.Drawing;
using Xamarin.Forms;

namespace XamSvg.XamForms
{
    public static partial class LocationFetcher
    {
        public static PointF GetCoordinates(VisualElement view) => InternalGetCoordinates(view);
    }
}