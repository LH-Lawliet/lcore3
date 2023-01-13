using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace lcore3.Client
{
    public class Player : BaseScript
    {
        public string modelName;
        public uint hash;
        public Vector3 coords;
        public float heading;
        public int ped;
        public int player;
        public bool spawned;

        public Player(Vector3 spawnCoords, string modelN)
        {
            RegisterScript(this);

            modelName = modelN;
            hash = (uint) GetHashKey(modelName);
            player = PlayerId();
            coords = spawnCoords;
            ped = GetPlayerPed(-1); // is it 0 ?
            spawned = false;

            Task task = SpawnPlayer();
 
            Tick += OnTick;
        }

        public override string ToString()
        {
            return "<Player> ("+modelName+", " + hash + ", " + player + ", " + coords + ")";
        }

        private async Task OnTick()
        {
            if (!spawned) { return ; }
            coords = Game.PlayerPed.Position;
            heading = Game.PlayerPed.Heading;
            await Delay(100);
        }

        public async Task SpawnPlayer()
        {
            FreezePlayer(true);
            await SetModel(modelName);
            SetVisible(true);
            RequestCollisionAtCoord(coords.X, coords.Y, coords.Z);
            Game.PlayerPed.Position = coords;
            Game.PlayerPed.Resurrect();
            ShutdownLoadingScreen();
            FreezePlayer(false);
            spawned = true;
        }

        public void SetVisible(bool visible) //because there is maybe some checks to do
        {
            SetEntityVisible(ped, visible, false);
        }

        public void FreezePlayer(bool freeze)
        {
            SetPlayerControl(player, !freeze, 0);
            SetEntityCollision(ped, !freeze, true);
            FreezeEntityPosition(ped, freeze);
        }

        public async Task SetModel(string modelN)
        {
            modelName = modelN;
            if (!IsModelInCdimage(hash) || !IsModelValid(hash))
            {
                Debug.WriteLine("Try to change a ped model, but " + modelName + " is not a ped model.");
                return;
            }

            RequestModel(hash);
            while (!HasModelLoaded(hash))
            {
                //Debug.WriteLine("Loading model : " + modelName);
                await Delay(10);
            }
            SetPlayerModel(player, hash);
            SetModelAsNoLongerNeeded(hash);
            ped = GetPlayerPed(-1);
            SetPedDefaultComponentVariation(ped);
        }
    }
}