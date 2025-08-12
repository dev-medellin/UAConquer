using System.Collections.Generic;

 
namespace TheChosenProject
{
  public class Regions
  {
    public ushort EndX;
    public ushort EndY;
    public uint Lineage;
    public uint MapID;
    public string Name = string.Empty;
    public ushort StartX;
    public ushort StartY;

    public static Regions FindRegion(uint map, ushort x, ushort y)
    {
      List<Regions> regionsList;
      if (ServerKernel.MapRegions.TryGetValue(map, out regionsList))
      {
        foreach (Regions region in regionsList.ToArray())
        {
          if (ServerKernel.IsBetweenTwoPoints(x, y, region.StartX, region.StartY, region.EndX, region.EndY))
            return region;
        }
      }
      return Regions.GetDefault(map);
    }

    public static Regions GetDefault(uint mapid)
    {
      return new Regions() { MapID = mapid, Lineage = 0 };
    }
  }
}
