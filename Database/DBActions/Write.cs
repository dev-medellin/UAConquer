﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TheChosenProject.Database.DBActions
{
    public enum Mode : byte
    {
        Open = 1,
        Create = 2
    }
    public class Write : IDisposable
    {
        private string location = "";
        private List<string> Items;

        private StreamWriter SW;



        public Write(string loc)
        {
            Items = new List<string>();
            location = ServerKernel.CO2FOLDER + loc;

        }
        public void ChangeLocation(string loc)
        {
            location = ServerKernel.CO2FOLDER + loc;

        }
        public void Dispose()
        {
            Items = null;
            location = string.Empty;
            SW = null;
        }
        public Write Add(string data)
        {
            Items.Add(data);
            return this;
        }

        public Write Execute(Mode mod)
        {
            bool Exit = true;
#pragma warning disable
            string Messaj = "";
#pragma warning restore

                if (mod == Mode.Open)
                {
                    if (Exit == File.Exists(location))
                    {
                        File.WriteAllText(location, string.Empty);
                   
                        using (SW = File.AppendText(location))
                        {
                            SW.WriteLine("Count=" + Items.Count);
                            for (int x = 0; x < Items.Count; x++)
                            {
                                SW.WriteLine(Items[x]);
                            }
                            SW.Close();
                        }
                    }
                    else
                    {
                        //MyConsole.WriteLine(Messaj = "Write new Reader " + location + " location");
                        using (SW = File.AppendText(location))
                        {
                            SW.WriteLine("Count=" + Items.Count);
                            for (int x = 0; x < Items.Count; x++)
                            {
                                SW.WriteLine(Items[x]);
                            }
                            SW.Close();
                        }
                    }
                }
                else
                {
                    using (SW = File.AppendText(location))
                    {
                        SW.WriteLine("Count=" + Items.Count);
                        for (int x = 0; x < Items.Count; x++)
                        {
                            SW.WriteLine(Items[x]);
                        }
                        SW.Close();
                    }
                }
                return this;
            }
        
       
    }
}
