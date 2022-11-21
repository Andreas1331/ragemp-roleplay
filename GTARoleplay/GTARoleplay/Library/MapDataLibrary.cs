using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace GTARoleplay.Library
{
    public class MapDataLibrary
    {
        /* The defined map borders used when getting data from files and from the data files */
        private static Tuple<float, float> mapX = new Tuple<float, float>(-4100f, 4300f);
        private static Tuple<float, float> mapY = new Tuple<float, float>(-4300f, 7825f);

        /* Data file names */
        private const string HEIGHTDATA_FILE = @"data_files/GTAV_HeightMap_Data.data";

        public static float GetHeightAtXY(float posX, float posY)
        {
            if (!ContainPosition(posX, posY))
                return 0f;

            MemoryMappedFile mmf;
            try
            {
                mmf = MemoryMappedFile.CreateFromFile(HEIGHTDATA_FILE, FileMode.Open);
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine("Directory not found: " + dirEx.Message + " proceeding with 0.0f");
                // Make this functional even if the file does not exit.
                return 0.0f;
            }

            using (mmf)
            {
                var x = (int)posX - (int)mapX.Item1;
                var y = (long)(mapX.Item2 - mapX.Item1) * ((long)posY - (long)mapY.Item1);

                using (var accessor = mmf.CreateViewAccessor((y + x) * 4, 4))
                {
                    return accessor.ReadSingle(0);
                }
            }
        }

        private static bool ContainPosition(float x, float y)
        {
            return mapX.Item1 <= x && x < 4100 && mapY.Item1 <= y && y <= mapY.Item2;
        }
    }
}
