using System.IO;

 
namespace TheChosenProject.Database
{
  public class SilentWords
  {
    public static void Load()
    {
      string[] strArray1 = File.ReadAllLines(ServerKernel.CO2FOLDER + "silent.txt");
      int length = strArray1.Length;
      for (int index = 0; index < length; ++index)
      {
        string[] strArray2 = strArray1[index].Split();
        SilentWords.Words words = new SilentWords.Words()
        {
          Badwords = strArray2[0]
        };
        ServerKernel.SilentWords.Add(words);
      }
      ServerKernel.Log.SaveLog(string.Format("Loaded File SilentWords ({0}).", (object) ServerKernel.SilentWords.Count), true);
    }

    public class Words
    {
      public string Badwords;
    }
  }
}
