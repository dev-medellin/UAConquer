﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Database
{
    public class NobilityTable
    {
        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
            {
                ini.FileName = fname;

                ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                ushort Face = ini.ReadUInt16("Character", "Face", 0);
                uint UID = ini.ReadUInt32("Character", "UID", 0);
                string Name = ini.ReadString("Character", "Name", "None");
                byte Gender = 0;
                if ((byte)(Body % 10) >= 3)
                    Gender = 0;
                else
                    Gender = 1;
                uint Mesh = (uint)(Face * 10000 + Body);
                ulong donation = ini.ReadUInt64("Character", "DonationNobility", 0);
                Role.Instance.Nobility nobility = new Role.Instance.Nobility(UID, Name, donation, Mesh, Gender);
                Program.NobilityRanking.UpdateRank(nobility);
            }
        }
    }
}
