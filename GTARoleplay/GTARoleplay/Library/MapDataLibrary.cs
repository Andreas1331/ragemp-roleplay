using System;
using System.Collections.Generic;
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
        private const string SAFECOORDDATA_FILE = @"data_files/GTAV_SafeCoord_Data.data";

        /* Object data file names and their max index */
        public static string BURNEDCARSDATA_FILE = @"data_files/BurnedCarsCoords_Data.data";
        public static string OUTSIDEDATA_FILE = @"data_files/OutsideObjectCoords_Data.data";
        public static string GARBAGEDATA_FILE = @"data_files/GarbageObjectCoords_Data.data";
        public static string ELECTRICALDATA_FILE = @"data_files/ElectricalObjectCoords_Data.data";

        private static Dictionary<string, int> MaxObjectIndexes = new Dictionary<string, int>()
        {
            {BURNEDCARSDATA_FILE, 49 },
            {OUTSIDEDATA_FILE, 7766 },
            {GARBAGEDATA_FILE, 8262 },
            {ELECTRICALDATA_FILE, 13477 }
        };

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

        public static Tuple<float, float> GetSafeCoordsAtXY(float posX, float posY)
        {
            if (!ContainPosition(posX, posY))
                return null;

            var mmf = MemoryMappedFile.CreateFromFile(SAFECOORDDATA_FILE, FileMode.Open);
            using (mmf)
            {
                var x = (int)posX - (int)mapX.Item1;
                var y = (long)(mapX.Item2 - mapX.Item1) * ((long)posY - (long)mapY.Item1);

                using (var accessor = mmf.CreateViewAccessor((y + x) * (4 * 2), (4 * 2)))
                {
                    return new Tuple<float, float>(accessor.ReadSingle(0), accessor.ReadSingle(4));
                }
            }
        }

        private static bool ContainPosition(float x, float y)
        {
            return mapX.Item1 <= x && x < 4100 && mapY.Item1 <= y && y <= mapY.Item2;
        }

        public static Tuple<float, float> GetStoredObjectCoords(int index, string fileName)
        {
            if (!ContainsObjectIndex(index, fileName))
                return null;

            var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open);
            using (mmf)
            {
                using (var accessor = mmf.CreateViewAccessor((index) * (4 * 2), (4 * 2)))
                {
                    return new Tuple<float, float>(accessor.ReadSingle(0), accessor.ReadSingle(4));
                }
            }
        }

        private static bool ContainsObjectIndex(int index, string fileName)
        {
            return index >= 0 && index < GetMaxObjectIndex(fileName);
        }

        public static int GetMaxObjectIndex(string fileName)
        {
            return MaxObjectIndexes.ContainsKey(fileName) ? MaxObjectIndexes[fileName] : 0;
        }
    }
}
