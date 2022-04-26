using System;
using System.Drawing;
using Xamarin.Forms;

namespace XamSvg.XamForms
{
    public static partial class LocationFetcher
    {
        /// <summary>
        /// Get the exact position of this view in screen coordinates using native methods 
        /// </summary>
        static PointF InternalGetCoordinates(VisualElement view) => throw new NotSupportedException();
        
        
        /// <summary>
        /// Get the exact bounds of this view in screen coordinates using native methods
        /// </summary>
        public static RectangleF GetAbsoluteBounds(VisualElement view)
        {
            var location = GetCoordinates(view);
            return new(location.X, location.Y, (float)view.Width, (float)view.Height);
        }
    }
}