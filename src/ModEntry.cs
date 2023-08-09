using System;
using System.Collections.Generic;
using System.Diagnostics;
using MapPropertyExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

/// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    public const string DATA_PATH = "data.json";
    public const string OVERLAY_PROPERTY_TAG = "CustomMapOverlay";
    public static ModData Data;
    public static OverlayProperty CurrentOverlay;
    public static int CurrentOverlayID = 0;
    public static int WindowWidth = 0;
    public static int WindowHeight = 0;
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        helper.Events.Player.Warped += Player_Warped;
        helper.Events.Display.RenderingHud += Display_RenderingHud;
        Game1.game1.Window.ClientSizeChanged += Window_ClientSizeChanged;
        ReadData();
        ValidateData();
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        WindowWidth = (Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferWidth : Game1.game1.Window.ClientBounds.Width);
        WindowHeight = (Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferHeight : Game1.game1.Window.ClientBounds.Height);
    }

    private void Display_RenderingHud(object sender, StardewModdingAPI.Events.RenderingHudEventArgs e)
    {
        if (CurrentOverlay is null)
            return;

        if (CurrentOverlay.Animation.NumberOfFrames > 1)
        {
            if (Game1.ticks % CurrentOverlay.Animation.FrameDuration == 0)
            {
                if (CurrentOverlayID + 1 == CurrentOverlay.Animation.NumberOfFrames)
                {
                    CurrentOverlayID = 0;
                }
                else
                {
                    CurrentOverlayID++;
                }
            }
            Texture2D currentOverlayTexture = CurrentOverlay.ImageTextures[CurrentOverlayID];
            Game1.spriteBatch.Draw(currentOverlayTexture, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);
        }
        else
        {
            Texture2D currentOverlayTexture = CurrentOverlay.ImageTextures[0];
            Game1.spriteBatch.Draw(currentOverlayTexture, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);
        }
    }

    private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
    {
        foreach (OverlayProperty property in Data.OverlayProperties)
        {
            if (e.NewLocation.getMapProperty(OVERLAY_PROPERTY_TAG) == property.PropertyName) // if correct map
            {
                CurrentOverlayID = 0;
                CurrentOverlay = property;
                break;
            }
            else
            {
                CurrentOverlay = null;
            }
        }
    }

    public void ReadData()
    {
        Monitor.Log("Reading data.");
        ModData md = Helper.Data.ReadJsonFile<ModData>(DATA_PATH);
        if (md == null) // if no data file
        {
            Monitor.Log("No \'data.json\' file found (or file is invalid)! Generating generic data file...", LogLevel.Error);
            md = new ModData(new List<OverlayProperty> { new OverlayProperty("MapTest", "assets/Image", new OverlayAnimation(1, 60)) }); // generate a demo moddata file
            Helper.Data.WriteJsonFile(DATA_PATH, md); // save file
        }
        Data = md;
    }
    public void ValidateData()
    {
        foreach (OverlayProperty property in Data.OverlayProperties)
        {
            Monitor.Log($"Validating data for {property.PropertyName}.");
            property.ImagePath += ".png";
            for (int i = 0; i < property.Animation.NumberOfFrames; i++)
            {
                try
                {
                    string path = property.ImagePath;
                    if (i > 0)
                    {
                        path = path.Split('.')[0] + i.ToString() + ".png";
                    }
                    property.ImageTextures.Add(Helper.ModContent.Load<Texture2D>(path));
                }
                catch (Exception ex)
                {
                    Monitor.Log($"{ex.Message}. Failed when loading image asset '{property.ImagePath}'. Stack Trace:", LogLevel.Error);
                    Monitor.Log(ex.StackTrace, LogLevel.Error);
                }
            }

            if (property.Animation.FrameDuration <= 0)
            {
                Monitor.Log($"Invalid FrameDuration when loading image asset '{property.ImagePath}'.", LogLevel.Error);
            }
            if (property.Animation.NumberOfFrames <= 0)
            {
                Monitor.Log($"Invalid NumberOfFrames when loading image asset '{property.ImagePath}'.", LogLevel.Error);
            }
        }
    }
}