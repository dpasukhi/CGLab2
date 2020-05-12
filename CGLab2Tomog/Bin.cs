using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CGLab2Tomog
{
    class Bin
    {
        public static int x, y, z;
        public static short[] array;
        public Bin() { }

        public void ReadBIN(string path)
        {
            if (File.Exists(path))
            {
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                x = reader.ReadInt32();
                y = reader.ReadInt32();
                z = reader.ReadInt32();

                int size = x * y * z;
                array = new short[size];
                for (int i = 0; i < size; ++i)
                {
                    array[i] = reader.ReadInt16();
                }
            }
        }
    }
}

