﻿/* DIRECTIVES */
using Cosmos.HAL;
using Cosmos.System;
using System.Collections.Generic;
using static HatchOS.HelperFunctions;

/* NAMESPACES */
namespace HatchOS
{
    /* CLASSES */
    internal class PowerFunctions
    {
        /* VARIABLES */
        public static List<string> PowerOptions = new List<string> { "-s" , "-r", "-a"};

        /* FUNCTIONS */
        // Draw the power menu and the selected option
        public static void DrawPowerMenu(PrismGraphics.Extentions.VMWare.SVGAIICanvas canvas, int Option)
        {
            canvas.DrawImage(0, 0, Kernel.PowerGradientBG, false);
            canvas.DrawString(0, 0, "[== CHOOSE A POWER OPTION ==]", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
            canvas.DrawString(0, Kernel.ScreenHeight - 16, "Press ESCAPE to return to the desktop", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);

            if (Option == 0)
            {
                canvas.DrawFilledRectangle(0, 16, 128, 16, 0, PrismGraphics.Color.White);
                canvas.DrawString(0, 16, "1. Shut down", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.Black);
                canvas.DrawString(0, 32, "2. Reboot", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.DrawString(0, 48, "3. ACPI", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
            }

            else if (Option == 1)
            {
                canvas.DrawFilledRectangle(0, 32, 128, 16, 0, PrismGraphics.Color.White);
                canvas.DrawString(0, 16, "1. Shut down", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.DrawString(0, 32, "2. Reboot", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.Black);
                canvas.DrawString(0, 48, "3. ACPI", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
            }
            else
            {
                canvas.DrawFilledRectangle(0, 48, 128, 16, 0, PrismGraphics.Color.White);
                canvas.DrawString(0, 16, "1. Shut down", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.DrawString(0, 32, "2. Reboot", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.DrawString(0, 48, "3. ACPI", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.Black);
            }

            canvas.Update();
        }

        // Show the power menu and let the user make a selection
        public static void PowerOff(PrismGraphics.Extentions.VMWare.SVGAIICanvas canvas, string mode)
        {
            // Display the power menu
            if(mode == "-sr")
            {
                int Option = 0;

                DrawPowerMenu(canvas, Option);

                // Wait for the user to select an option
                while (true)
                {
                    try
                    {
                        // Try to read a key frm the keyboard
                        KeyboardManager.TryReadKey(out var key);

                        // Call the garbage collector so we don't have as many memory leaks
                        Cosmos.Core.Memory.Heap.Collect();

                        // If the up arrow kes is pressed, change the power option
                        if (key.Key == ConsoleKeyEx.UpArrow)
                        {
                            Option--;
                            if (Option < 0)
                            {
                                Option = 2;
                            }
                        }

                        // If the down arrow kes is pressed, change the power option
                        if (key.Key == ConsoleKeyEx.DownArrow)
                        {
                            // Draw the power menu
                            Option++;
                            if(Option > 2)
                            {
                                Option = 0;
                            }
                        }

                        // If the enter key is pressed, shut down or reboot the system
                        if (key.Key == ConsoleKeyEx.Enter)
                        {
                            PowerOff(canvas, PowerOptions[Option]);
                        }

                        // If the escape key is pressed, close the power menu
                        if(key.Key == ConsoleKeyEx.Escape)
                        {
                            break;
                        }

                        // Only update the canvas if the user pressed any keys
                        if(key.Key != ConsoleKeyEx.NoName)
                        {
                            DrawPowerMenu(canvas, Option);
                        }
                    }
                    catch
                    {
                        canvas.Clear(PrismGraphics.Color.Black);
                        canvas.DrawImage(0, 0, Kernel.PowerGradientBG);
                        canvas.DrawImage((int)(800 / 2 - Kernel.OSLogo.Width / 2), (int)(Kernel.ScreenHeight / 2 - (Kernel.OSLogo.Height / 2)), Kernel.OSLogo);
                        canvas.DrawString(Kernel.ScreenHeight / 2 - ((27 * 8) / 2), (Kernel.ScreenHeight - 2) + 20, "HatchOS is shutting down...", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                        canvas.Update();
                        var Timer = new PIT.PITTimer(Shutdown, SecondsToNanoseconds(1), true);
                        Cosmos.HAL.Global.PIT.RegisterTimer(Timer);
                        while (true) ;
                    }
                }
            }

            // Restart the computer
            else if (mode == "-r")
            {
                canvas.Clear(PrismGraphics.Color.Black);
                canvas.DrawImage(0, 0, Kernel.PowerGradientBG);
                canvas.DrawImage((int)(Kernel.ScreenWidth / 2 - Kernel.OSLogo.Width / 2), (int)(Kernel.ScreenHeight / 2 - (Kernel.OSLogo.Height / 2)), Kernel.OSLogo);
                canvas.DrawString(Kernel.ScreenWidth / 2 - ((24 * 8) / 2), (Kernel.ScreenHeight / 2) + 20, "HatchOS is restarting...", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.Update();
                var Timer = new PIT.PITTimer(Restart, SecondsToNanoseconds(1), true);
                Cosmos.HAL.Global.PIT.RegisterTimer(Timer);
                while (true) ;
            }

            // Restart the computer using ACPI
            else if (mode == "-a")
            {
                canvas.Clear(PrismGraphics.Color.Black);
                canvas.DrawImage(0, 0, Kernel.PowerGradientBG);
                canvas.DrawImage((int)(Kernel.ScreenWidth / 2 - Kernel.OSLogo.Width / 2), (int)(Kernel.ScreenHeight / 2 - (Kernel.OSLogo.Height / 2)), Kernel.OSLogo);
                canvas.DrawString(Kernel.ScreenWidth / 2 - ((31 * 8) / 2), (Kernel.ScreenHeight / 2) + 20, "HatchOS is restarting (ACPI)...", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.Update();
                var Timer = new PIT.PITTimer(RebootACPI, SecondsToNanoseconds(1), true);
                Cosmos.HAL.Global.PIT.RegisterTimer(Timer);
                while (true) ;
            }

            // Turn off the computer
            else if (mode == "-s")
            {
                canvas.Clear(PrismGraphics.Color.Black);
                canvas.DrawImage(0, 0, Kernel.PowerGradientBG);
                canvas.DrawImage((int)(Kernel.ScreenWidth / 2 - Kernel.OSLogo.Width / 2), (int)(Kernel.ScreenHeight / 2 - (Kernel.OSLogo.Height / 2)), Kernel.OSLogo);
                canvas.DrawString(Kernel.ScreenWidth / 2 - ((27 * 8) / 2), (Kernel.ScreenHeight / 2) + 20, "HatchOS is shutting down...", PrismGraphics.Fonts.Font.Fallback, PrismGraphics.Color.White);
                canvas.Update();
                var Timer = new PIT.PITTimer(Shutdown, SecondsToNanoseconds(1), true);
                Cosmos.HAL.Global.PIT.RegisterTimer(Timer);
                while (true) ;
            }
        }

        // Restart the computer
        public static void Restart()
        {
            Cosmos.System.Power.Reboot();
        }

        // Turn off the computer
        public static void Shutdown()
        {
            Cosmos.System.Power.Shutdown();
        }

        // Restart the computer with ACPI
        public static void RebootACPI()
        {
            Cosmos.Core.ACPI.Reboot();
        }
    }
}
