using System.Collections.Generic;
using System.IO;

 
namespace TheChosenProject.Database
{
  public class SpecialTitles
  {
    public uint ID;
    public string Name = "";
    public uint Time;
    public static Dictionary<uint, SpecialTitles> Titles = new Dictionary<uint, SpecialTitles>();

    public static void LoadDBInformation()
    {
      ServerKernel.EncryptFiles.Decrypt(ServerKernel.CO2FOLDER + "title_type.txt");
      foreach (string readAllLine in File.ReadAllLines(ServerKernel.CO2FOLDER + "title_type.txt"))
      {
        char[] chArray = new char[1]{ ' ' };
        string[] strArray = readAllLine.Split(chArray);
        SpecialTitles specialTitles = new SpecialTitles()
        {
          ID = uint.Parse(strArray[0]),
          Name = strArray[1],
          Time = uint.Parse(strArray[2])
        };
        SpecialTitles.Titles.Add(specialTitles.ID, specialTitles);
      }
      ServerKernel.EncryptFiles.Encrypt(ServerKernel.CO2FOLDER + "title_type.txt");
    }
  }
}
