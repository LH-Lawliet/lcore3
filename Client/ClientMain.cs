using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace lcore3.Client
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }  
    }

    public class Init : BaseScript
    {
        static public Player player;
        public Init() 
        {
            Debug.WriteLine("lcore3 started");
            player = new Player(new Vector3(231.356f, -870.817f, 30.4921f), "mp_m_freemode_01");
            Debug.WriteLine("New player : "+player);
        }
        
    }
}