using Extensions;

 
namespace TheChosenProject.Role
{
  public class IEventRewards
  {
    public static Counter IDCounter = new Counter(10);
    public string Name;
    public uint ClaimConquerPoint;
    public uint ClaimMoney;
    public string ClamedItem;
    public string Data;

    public uint ID { get; set; }

    public static void Add(string name, uint cps, uint silver, string item, string data)
    {
      try
      {
        IEventRewards ieventRewards = new IEventRewards()
        {
          Name = name,
          ClaimConquerPoint = cps,
          ClaimMoney = silver,
          ClamedItem = item,
          Data = data
        };
        do
        {
          ieventRewards.ID = IEventRewards.IDCounter.Next;
        }
        while (ServerKernel.EventRewards.ContainsKey(ieventRewards.ID));
        if (ServerKernel.EventRewards.ContainsKey(ieventRewards.ID))
          return;
        ServerKernel.EventRewards.Add(ieventRewards.ID, ieventRewards);
      }
      catch
      {
      }
    }
  }
}
