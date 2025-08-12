using System;
using System.Linq;
using Extensions;
using TheChosenProject.Database.DBActions;

namespace TheChosenProject.Database
{
	public static class VIPExperience
	{
		public class Client
		{
			public uint StudentUID;

			public uint ExperienceUID;

			public string StudentName;

			public byte ExperienceLevShare;

			public DateTime ShareEnds;

			public override string ToString()
			{
				WriteLine writer;
				writer = new WriteLine('/');
				writer.Add(StudentUID).Add(ExperienceUID).Add(StudentName)
					.Add(ExperienceLevShare)
					.Add(ShareEnds.Ticks);
				return writer.Close();
			}
		}

		public static MyList<Client> VIPExperiencePoll = new MyList<Client>();

		public static bool CanShare(uint UID, uint ExperienceUID)
		{
			if ((from p in VIPExperiencePoll.GetValues()
				where p.ExperienceUID == UID
				select p).ToList().Count > 0)
				return false;
			if ((from p in VIPExperiencePoll.GetValues()
				where p.ExperienceUID == ExperienceUID
				select p).ToList().Count > 0)
				return false;
			if ((from p in VIPExperiencePoll.GetValues()
				where p.StudentUID == ExperienceUID
				select p).ToList().Count > 0)
				return false;
			if ((from p in VIPExperiencePoll.GetValues()
				where p.StudentUID == UID
				select p).ToList().Count > 0)
				return false;
			return true;
		}

		public static bool Add(Client x)
		{
			if (CanShare(x.StudentUID, x.ExperienceUID))
			{
				VIPExperiencePoll.Add(x);
				return true;
			}
			return false;
		}

		public static void Save()
		{
			using (Write writer = new Write("VIPExperience.txt"))
			{
				Client[] values;
				values = VIPExperiencePoll.GetValues();
				foreach (Client x in values)
				{
					writer.Add(x.ToString());
				}
				writer.Execute(Mode.Open);
			}
		}

		public static void Load()
		{
			using (Read Reader = new Read("VIPExperience.txt"))
			{
				if (Reader.Reader())
				{
					uint count;
					count = (uint)Reader.Count;
					for (uint i = 0; i < count; i++)
					{
						ReadLine readline;
						readline = new ReadLine(Reader.ReadString(""), '/');
						Client x;
						x = new Client
						{
							StudentUID = readline.Read((uint)0),
							ExperienceUID = readline.Read((uint)0),
							StudentName = readline.Read(""),
							ExperienceLevShare = readline.Read((byte)0),
							ShareEnds = DateTime.FromBinary(readline.Read(0L))
						};
						VIPExperiencePoll.Add(x);
					}
				}
			}
		}
	}
}
