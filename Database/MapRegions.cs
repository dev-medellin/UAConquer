using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

 
namespace TheChosenProject
{
  public class MapRegions : ConcurrentDictionary<uint, List<Regions>>
  {
    public void Load()
    {
      using (StreamReader streamReader = new StreamReader((Stream) File.Open(ServerKernel.CO2FOLDER + "region.ini", FileMode.Open)))
      {
        while (!streamReader.EndOfStream)
        {
          string[] strArray = streamReader.ReadLine().Replace("  ", " ").Split(' ');
          if (uint.Parse(strArray[1]) == 8)
          {
            Regions regions = new Regions()
            {
              MapID = (uint) ushort.Parse(strArray[0]),
              StartX = ushort.Parse(strArray[2]),
              StartY = ushort.Parse(strArray[3]),
              EndX = (ushort) ((uint) ushort.Parse(strArray[2]) + (uint) ushort.Parse(strArray[4])),
              EndY = (ushort) ((uint) ushort.Parse(strArray[3]) + (uint) ushort.Parse(strArray[5])),
              Name = strArray[6] != "none" ? strArray[6] : strArray[7],
              Lineage = uint.Parse(strArray[8])
            };
            if (!this.ContainsKey(regions.MapID))
              this.TryAdd(regions.MapID, new List<Regions>());
            this[regions.MapID].Add(regions);
          }
        }
        streamReader.Close();
      }
    }
  }
}
