using System;

namespace TheChosenProject.Client
{
	[Flags]
	public enum ServerFlag : ushort
	{
		None = 0,
		AcceptLogin = 1,
		CreateCharacter = 2,
		CreateCharacterSucces = 4,
		LoginFull = 8,
		SetLocation = 0x10,
		OnLoggion = 0x20,
		QueuesSave = 0x40,
		RemoveSpouse = 0x80,
		Disconnect = 0x100,
		UpdateSpouse = 0x200
	}
}
