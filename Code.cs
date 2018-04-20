using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Reflection;

namespace TextRenderingSample
{
    class TextRendering
    {
        /// <summary>
        /// Creates instance of GlyphRun, with option to control Text Formatting Mode (Display or Ideal). Returns null instead of throwing
        /// exceptions. See
        /// https://referencesource.microsoft.com/#PresentationCore/Core/CSharp/System/Windows/Media/GlyphRun.cs,74b10b2dd2d99a80,references.
        /// </summary>
        public static GlyphRun TryCreateGlyphRun(
            GlyphTypeface glyphTypeface,
            int bidiLevel,
            bool isSideways,
            double renderingEmSize,
            float pixelsPerDip,
            IList<ushort> glyphIndices,
            Point baselineOrigin,
            IList<double> advanceWidths,
            IList<Point> glyphOffsets,
            IList<char> characters,
            string deviceFontName,
            IList<ushort> clusterMap,
            IList<bool> caretStops,
            XmlLanguage language,
            TextFormattingMode textLayout)
        {
            return _TryCreateGlyphRun?.Invoke(
                glyphTypeface,
                bidiLevel,
                isSideways,
                renderingEmSize,
                pixelsPerDip,
                glyphIndices,
                baselineOrigin,
                advanceWidths,
                glyphOffsets,
                characters,
                deviceFontName,
                clusterMap,
                caretStops,
                language,
                textLayout
            );
        }

        /// <summary>Pixels per Device-independent-pixel</summary>
        public static float PixelsPerDip => ((float)(_GetDpi?.Invoke() ?? 96)) / 96;

        private static TryCreateGlyphRunDelegate _TryCreateGlyphRun { get; }

        private static Func<int> _GetDpi { get; }

        static TextRendering()
        {
            try 
            {
                var tryCreateGlyphRunMethod = typeof(GlyphRun).GetMethod("TryCreate", BindingFlags.NonPublic | BindingFlags.Static);
                _TryCreateGlyphRun = TryCreateGlyphRunMethod.CreateDelegate(typeof(TryCreateGlyphRunDelegate)) as TryCreateGlyphRunDelegate;

                var utilType = typeof(GlyphRun).Assembly.GetType("MS.Internal.FontCache.Util");
                var getDpiProperty = utilType.GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic);
                _GetDpi = getDpiProperty.GetGetMethod(true).CreateDelegate(typeof(Func<int>)) as Func<int>;
            }
            catch
            {
                // In case of exception binding methods via reflection, let unset delegates remain null. Public methods will return null.
            }
        }
    }
}