using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace lcore3.Client
{
    public class LDebug : BaseScript
    {
        private Vehicle createdVehicle = null;
        public LDebug()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            RegisterCommand("car", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var model = "blista";
                if (args.Count > 0)
                {
                    model = args[0].ToString();
                }

                if (createdVehicle != null)
                {
                    createdVehicle.Delete();
                }

                createdVehicle = new Vehicle(Init.player.coords, model, Init.player.heading, true);
                
            }), false);
        }
    }
}