using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace lcore3.Client 
{
    public class Vehicle : BaseScript
    {
        public string modelName;
        public uint hash;
        public Vector3 coords;
        public int id;
        public Color color1;
        public Color color2;

        internal Vehicle(Vector3 spawnCoords, string model, float heading=0.0f, bool inside=false)
        {
            modelName = model;
            hash = (uint) GetHashKey(modelName);
            coords = spawnCoords;
            Task task = VehicleSpawn(heading, inside);
        }

        async private Task VehicleSpawn(float heading, bool inside)
        {
            if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
            {
                Debug.WriteLine("Try to create a vehicle but "+modelName+" isn't a vehicle model.");
                return;
            }

            RequestModel(hash);
            while (!HasModelLoaded(hash))
            {
                //Debug.WriteLine("Loading model : " + modelName);
                await Delay(10);
            }
            id = CreateVehicle(hash, coords.X, coords.Y, coords.Z, heading, true, true);
            SetModelAsNoLongerNeeded(hash);

            SetToDefault();

            if (inside)
            {
                SetPedIntoVehicle(Init.player.ped, id, -1);
            }
        }

        public void SetToDefault()
        {
            SetVehicleCanLeakOil(id, true);
            SetVehicleCanLeakPetrol(id, true);
            SetVehicleEngineCanDegrade(id, true);

            Color white = new Color(255, 255, 255);
            SetColors(white, white);

            Repair();
            Clean();
        }

        public void Repair()
        {
            for (int i=0; i<7; i++) {
                FixVehicleWindow(id, i);
            }
            SetVehicleEngineHealth(id, 1000f);
            SetVehicleBodyHealth(id, 1000f);
        }

        public void Clean()
        {
            SetVehicleDirtLevel(id, 0.0f);
        }

        public void SetColors(Color n_color1, Color n_color2)
        {
            color1 = n_color1;
            color2 = n_color2;
            SetVehicleCustomPrimaryColour(id, color1.R, color1.G, color1.B);
            SetVehicleCustomSecondaryColour(id, color2.R, color2.G, color2.B);
        }

        public void Delete()
        {
            DeleteVehicle(ref id);
        }
    }
}