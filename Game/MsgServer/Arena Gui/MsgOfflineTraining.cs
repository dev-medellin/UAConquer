using System.Runtime.InteropServices;

 
namespace TheChosenProject.Game.MsgServer
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct MsgOfflineTraining
  {
    public enum Mode
    {
      NotActive = 0,
      Hunting = 1,
      Shopping = 2,
      TrainingGroup = 4,
      Completed = 8,
    }
  }
}
