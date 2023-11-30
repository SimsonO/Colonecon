using System;
using Microsoft.Xna.Framework;

public static class GlobalColorScheme
{
    // Define colors as static readonly properties or fields
    public static readonly Color BackgroundColor = new Color(51,41,115);
    public static readonly Color PrimaryColor = new Color(83,78,217);
    public static readonly Color AccentColor = new Color(191,63,123);
    public static readonly Color TileColor = new Color(83,78,217);
    public static readonly Color PlayerColor = new Color(242,68,114);
    public static readonly Color NPCFaction1Color =  new Color(242,93,80);
    public static readonly Color NPCFaction2Color =  new Color(198, 155, 109);
    
    public static Color AdjustIntensity(Color baseColor, float intensityFactor)
    {
        // Ensure the intensity factor is in the range 0 (darker) - 1 (original color) - more (lighter)
        intensityFactor = Math.Max(0, intensityFactor); // Remove the upper limit to allow for lighter colors

        float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }
        // Assuming intensityFactor < 1 for darker, intensityFactor > 1 for lighter colors
        float r = (intensityFactor < 1) ? 
            Lerp(baseColor.R, 0, 1 - intensityFactor) : 
            Lerp(baseColor.R, 255, intensityFactor - 1);

        float g = (intensityFactor < 1) ? 
            Lerp(baseColor.G, 0, 1 - intensityFactor) : 
            Lerp(baseColor.G, 255, intensityFactor - 1);

        float b = (intensityFactor < 1) ? 
            Lerp(baseColor.B, 0, 1 - intensityFactor) : 
            Lerp(baseColor.B, 255, intensityFactor - 1);

        // Ensure the RGB values are within the 0-255 range
        int ri = (int)MathHelper.Clamp(r, 0, 255);
        int gi = (int)MathHelper.Clamp(g, 0, 255);
        int bi = (int)MathHelper.Clamp(b, 0, 255);

        // Return the new color with the original alpha value
        return new Color(ri, gi, bi, baseColor.A);
    }
}


