using TheChosenProject.Struct;
using System.Collections.Generic;
using System.IO;

namespace TheChosenProject.Managers
{
    public class InsultManager : Dictionary<string, Insult>
    {
        public void VoteNotInsult(string text)
        {
            Insult currentWorking;
            if (TryGetValue(text, out currentWorking))
            {
                currentWorking.Votedfalse = (byte)(currentWorking.Votedtrue + 1);
            }
            else
            {
                Add(text, new Insult() { Text = text, Votedtrue = 0, Votedfalse = 1 });
            }
        }

        public void VoteInsult(string text)
        {
            Insult currentWorking;
            if (TryGetValue(text, out currentWorking))
            {
                currentWorking.Votedtrue += 1;
            }
            else
            {
                Add(text, new Insult() { Text = text, Votedtrue = 1, Votedfalse = 0 });
            }
        }

        public void CheckInsults(ref string Message)
        {
            string newMessage = "";
            foreach (Insult insult in Values)
            {
                if (Message.ToLower().Contains(insult.Text.ToLower()))
                {
                    if (insult.Votedtrue >= insult.Votedfalse)
                    {
                        newMessage = Message.ToLower().Replace(insult.Text.ToLower(), "***");
                    }
                }
            }
            if (newMessage == "")
            {
                return;
            }
            Message = newMessage;
        }

        public void Load()
        {
            unsafe
            {
                if (!File.Exists(ServerKernel.CO2FOLDER + "\\Insults.bin"))
                    File.Create(ServerKernel.CO2FOLDER + "\\Insults.bin").Dispose();
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(ServerKernel.CO2FOLDER + "\\Insults.bin", System.IO.FileMode.Open))
                {
                    Insult insult;
                    int count;
                    binary.Read(&count, sizeof(int));
                    for (int x = 0; x < count; x++)
                    {
                        binary.Read(&insult, sizeof(Insult));
                        Add(insult.Text, insult);
                    }
                    binary.Close();
                }
            }
        }

        public void Save()
        {
            unsafe
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(ServerKernel.CO2FOLDER + "\\Insults.bin", System.IO.FileMode.Create))
                {
                    int count = Values.Count;
                    Insult workinginsult;
                    binary.Write(&count, sizeof(int));
                    foreach (Insult insult in Values)
                    {
                        workinginsult = insult;
                        binary.Write(&workinginsult, sizeof(Insult));
                    }
                }
                binary.Close();
            }
        }
    }
}