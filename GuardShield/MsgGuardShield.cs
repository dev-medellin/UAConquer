using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMsgGuardShield
{
    public static class MsgGuardShield
    {
        [DllImport("GuardShield.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int IntlizingLoader(byte[] localip, byte[] XOR_KEY);

        [DllImport("GuardShield.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GuardEncrypt(byte[] buffer, int Length);

        [DllImport("GuardShield.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GuardDecrypt(byte[] buffer, int Length);

        [DllImport("GuardShield.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int Set_IV_Keys(int Seed);

        [DllImport("GuardShield.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int HandleConnect(byte[] buffer, int Length);


        private static string MagicHash, ConquerHashes, DLLHash/*, C3Hash*/, MagicEffectHash, StrResHash, DataServersHash;
        private static bool HdKeyVaild = true;
        public static bool CheakConquer = false;
        public static bool IsGameServer = false;
        public static GuardCipher GCipher;
        public static string IPAdresss;
        public static void Load(string LocalIp, bool GameServer = false)
        {
            IPAdresss = LocalIp;
            IsGameServer = GameServer;
            IntlizingLoader(Encoding.ASCII.GetBytes(LocalIp), Encoding.ASCII.GetBytes("X565474EEFFF_FEZS"));
            GCipher = new GuardCipher(GameServer);
            if (GameServer)
            {
                CheakConquer = true;
                ConquerHashes = CalculateMD5(@"Files\Conquer.exe");
                MagicHash = CalculateMD5(@"Files\ini\magictype.dat");
                MagicEffectHash = CalculateMD5(@"Files\ini\MagicEffect.ini");
                StrResHash = CalculateMD5(@"Files\ini\StrRes.ini");
                DataServersHash = CalculateMD5(@"Files\COServer.dat");
                // C3Hash = CalculateMD5(@"Files\ini\c3.wdb");
                DLLHash = CalculateMD5(@"Files\COServer.dll");
            }
        }
        private static string CalculateMD5(string filename)
        {
            try
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = System.IO.File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        string H = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        return H;
                    }
                }
            }
            catch
            {
                HdKeyVaild = false;
                Console.WriteLine(string.Format("{0} Not Find Hash", filename));
            }
            return "";
        }
        public static bool Validated(string conquer, string magicType, string magicEffect, string c3_WDB, string dLL_Hash, string StrRes, string Data_Servers, out string filechanged)
        {
            filechanged = "";
            if (CheakConquer) {
                if (conquer != ConquerHashes)
                    filechanged += " Conquer.exe";
            }

            if (magicType != MagicHash)
                filechanged += " magictype.dat";
            if (magicEffect != MagicEffectHash)
                filechanged += " MagicEffect.ini";
            //if (c3_WDB != C3Hash)
            //    filechanged += " c3.wdb";
            if (dLL_Hash != DLLHash)
                filechanged += " COServer.dll";
            if (StrRes != StrResHash)
                filechanged += " StrRes.ini";
            if (Data_Servers != DataServersHash)
                filechanged += " COServer.dat";
            return filechanged == "" ? true : false;
        }
        public static void ReportLogg(String Name, string reason)
        {
            DateTime timer = DateTime.Now;
            string logs = string.Format("CHEAT [Player] {0} -- REASON: {1}", Name, !String.IsNullOrEmpty(reason) ? reason : "Abnormal operation");
            OnDequeue(logs, timer.Millisecond);
        }
        private static void OnDequeue(object obj, int time)
        {
            try
            {
                if (obj is string)
                {
                    string text = obj as string;
                    string identifier = text.Substring(0, text.IndexOf("]") + 1);
                    string UnhandledExceptionsPath = Application.StartupPath + "\\LoaderLogg\\";
                    if (!Directory.Exists(UnhandledExceptionsPath))
                        Directory.CreateDirectory(UnhandledExceptionsPath);
                    UnhandledExceptionsPath += "[Logs]\\";
                    if (!Directory.Exists(UnhandledExceptionsPath))
                        Directory.CreateDirectory(UnhandledExceptionsPath);
                    UnhandledExceptionsPath += identifier + "\\";
                    if (!Directory.Exists(UnhandledExceptionsPath))
                        Directory.CreateDirectory(UnhandledExceptionsPath);
                    string fileName = UnhandledExceptionsPath + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";
                    if (!File.Exists(fileName))
                        File.WriteAllLines(fileName, new string[0]);
                    using (var SW = File.AppendText(fileName))
                    {
                        SW.WriteLine(text.Replace(identifier, DateTime.Now.ToString("[hh:mm:ss tt]:")));
                        SW.Close();
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
       
        public static string RemoveIllegalCharacters(this string str, bool path, bool file)
        {
            string myString = str;
            if (file)
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    myString = myString.Replace(c, '_');
            if (path)
                foreach (char c in System.IO.Path.GetInvalidPathChars())
                    myString = myString.Replace(c, '_');
            return myString;
        }
        public static byte[] RequestOpenedProcesses(MsgOpenedProcesses.Type Type = MsgOpenedProcesses.Type.Start)
        {

            byte[] buffer = new byte[4 + 30 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgRequestOpenedProcesses;
                        *((byte*)(Buffer + 4)) = (byte)Type;
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
        public static byte[] RequestMachineInfo()
        {
            byte[] buffer = new byte[4 + 30 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgRequestMachineInfo;
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
       
        public static byte[] PingStatuesLoader(string PingStatuesMsg = "")
        {
            byte[] buffer = new byte[6 + PingStatuesMsg.Length + 2 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.PingStatuesLoader;
                        *((byte*)(Buffer + 5)) = (byte)PingStatuesMsg.Length;
                        ushort i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(PingStatuesMsg).Length)
                        {
                            *((byte*)(Buffer + 6 + i)) = (byte)System.Text.Encoding.Default.GetBytes(PingStatuesMsg)[i];
                            i++;
                        }
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
        public static byte[] CommandToClient(string command = "")
        {
            if (command.Length < 5)
                return new byte[0];
            byte[] buffer = new byte[6 + command.Length + 2 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgWinExec;
                        *((byte*)(Buffer + 4)) = (byte)command.Length;
                        ushort i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(command).Length)
                        {
                            *((byte*)(Buffer + 5 + i)) = (byte)System.Text.Encoding.Default.GetBytes(command)[i];
                            i++;
                        }
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
        public static byte[] TerminateLoader(string MessageCaption = "", string MessageText = "")
        {
            byte[] buffer = new byte[6 + MessageCaption.Length + MessageText.Length + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgTerminateLoader;
                        *((byte*)(Buffer + 4)) = (byte)MessageCaption.Length;
                        ushort i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(MessageCaption).Length)
                        {
                            *((byte*)(Buffer + 5 + i)) = (byte)System.Text.Encoding.Default.GetBytes(MessageCaption)[i];
                            i++;
                        }
                        *((byte*)(Buffer + 5 + System.Text.Encoding.Default.GetBytes(MessageCaption).Length)) = (byte)MessageText.Length;
                        i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(MessageText).Length)
                        {
                            *((byte*)(Buffer + 6 + i + System.Text.Encoding.Default.GetBytes(MessageCaption).Length)) = (byte)System.Text.Encoding.Default.GetBytes(MessageText)[i];
                            i++;
                        }
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }


        public static byte[] RequestJumpInfo(JumpType type, byte Value)
        {
            byte[] buffer = new byte[12 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgJumpTaks;
                        *((byte*)(Buffer + 4)) = (byte)type;
                        *((byte*)(Buffer + 5)) = (byte)Value;
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
        public enum PacketIDs : ushort
        {
            MsgAuthentication = 0xDEAD,
            MsgRequestOpenedProcesses,
            MsgOpenedProcesses,
            MsgTerminateLoader,
            MsgLoader,
            MsgRequestMachineInfo,
            MsgMachineInfo,
            MsgLoginGame,
            PingStatuesLoader,
            MsgRequestScreenPacket,
            MsgScreenPacket,
            MsgPing,
            MsgJumpTaks,
            MsgWinExec,
            RetLoginPage,
        }

        public enum JumpType : byte
        {
            SetJumpFar = 0,
            HighJump = 1,
        }
        public class MsgJumpTaks
        {
            public JumpType ActionType;
            public MsgJumpTaks(byte[] Buffer, GuardShield Guard = null)
            {
                if (Buffer != null && Buffer.Length >= 4 + 1 + 2)
                {
                    if (HdKeyVaild && BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgJumpTaks)
                    {
                        if (GCipher.HandleBuffer(ref Buffer, true) == 0)
                            return;
                        if (Guard != null)
                            Guard.HandleOwner(Buffer);
                        BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                        rdr.BaseStream.Seek(4, SeekOrigin.Current);
                        ActionType = (JumpType)rdr.ReadByte(); // 4
                        rdr.Close();
                    }
                }
            }
        }

        public class MsgLoader
        {
            [Flags]
            public enum LoaderMessage : byte
            {
                MemoryChanged = 1,
                AutoHunting,
                AutoClick,
                ScriptAutoClick,
                AutoHotkey,
                ScriptAutoHotkey,
                ClientFilesScaning,
                SpeedHack,
                FunctionChanged,
                Injectdll,
                InjectCode,
                Aimbot,
                ZoomHack,
                DebuggerPresent,
                SuspendThreads,
                AbnormalOperation,
                ChangeItems,
                ChangeSpellsID,
                ChangeAction,
            }
            public LoaderMessage Type;
            public byte GuardVersion;
            public string strParam, Conquer, MagicType, MagicEffect, C3_WDB, DLL_Hash, StrRes, Data_Servers;
            public uint dwParam1, dwParam2;

            public MsgLoader(byte[] Buffer, GuardShield Guard = null)
            {
                if (Buffer != null && Buffer.Length >= 4 + 1 + 2)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgLoader)
                    {
                        if (HdKeyVaild)
                        {
                            if (GCipher.HandleBuffer(ref Buffer, true) == 0)
                                return;
                            if (Guard != null)
                                Guard.HandleOwner(Buffer);

                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            Type = (LoaderMessage)rdr.ReadByte();
                            if (Type == LoaderMessage.ClientFilesScaning)
                            {
                                Conquer = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                DLL_Hash = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                MagicType = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                MagicEffect = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                StrRes = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                C3_WDB = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                Data_Servers = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                GuardVersion = rdr.ReadByte();
                            }
                            else if (Type == LoaderMessage.ChangeItems || Type == LoaderMessage.ChangeSpellsID || Type == LoaderMessage.ChangeAction)
                            {
                                strParam = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                dwParam1 = rdr.ReadUInt32();
                                dwParam2 = rdr.ReadUInt32();
                            }
                            else
                            {

                                strParam = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                            }
                            rdr.Close();
                        }
                    }
                }
            }
        }
        public class MsgMachineInfo
        {
            public string MachineName, MacAddress;
            public uint HDSerial;
            public bool VMware, VPSMachine;

            public MsgMachineInfo(byte[] Buffer, GuardShield Guard = null)
            {
                if (Buffer != null)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgMachineInfo)
                    {
                        if (HdKeyVaild)
                        {
                            if (GCipher.HandleBuffer(ref Buffer, true) == 0)
                                return;
                            if (Guard != null)
                                Guard.HandleOwner(Buffer);
                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            MacAddress = System.Text.Encoding.Default.GetString(rdr.ReadBytes(16)).Replace("\0", "");
                            MachineName = System.Text.Encoding.Default.GetString(rdr.ReadBytes(32)).Replace("\0", "");
                            HDSerial = rdr.ReadUInt32();
                            VMware = rdr.ReadByte() == 1;
                            VPSMachine = rdr.ReadByte() == 1;
                            rdr.Close();
                        }
                    }
                }
            }
        }

        public class MsgLoginGame
        {
            public string MachineName, MacAddress, Username, HWID, Language;
            public uint Key, AccountHash, HDSerial, EncryptAttack, WidthScreen, HeightScreen, CryptoKey;
            public bool VMware, VPSMachine, ShowHPbar;
            public bool AllowLogin = false;
            public MsgLoginGame(byte[] Buffer, GuardShield Guard = null)
            {

                if (Buffer != null)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgLoginGame)
                    {
                        if (HdKeyVaild)
                        {
                            GCipher.HandleBuffer(ref Buffer, true);
                            if (Guard != null)
                                Guard.HandleOwner(Buffer);
                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            CryptoKey = rdr.ReadUInt32();
                            if (CryptoKey != 0xF8E44A2)
                            {
                                Guard.Disconnect();
                                throw new Exception("Invalid buffer Info");
                            }
                            Key = rdr.ReadUInt32();
                            AccountHash = rdr.ReadUInt32();
                            MachineName = Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadByte())).Replace("\0", "");
                            MacAddress = Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadByte())).Replace("\0", "");
                            Username = Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadByte())).Replace("\0", "");
                            HWID = Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadByte())).Replace("\0", "");
                            Language = Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadByte())).Replace("\0", "");
                            HDSerial = rdr.ReadUInt32();
                            EncryptAttack = rdr.ReadUInt32();
                            VMware = rdr.ReadByte() == 1;
                            VPSMachine = rdr.ReadByte() == 1;
                            ShowHPbar = rdr.ReadByte() == 1;
                            WidthScreen = rdr.ReadUInt16();
                            HeightScreen = rdr.ReadUInt16();
                            AllowLogin = true;
                            rdr.Close();
                        }
                    }
                }
            }
        }
        public class MsgOpenedProcesses
        {
            public enum Type : byte
            {
                Start,
                Insert,
                Finish,

                Start_ProcessQuery,
                Insert_ProcessQuery,
                Finish_ProcessQuery,
            }

            public Type ActionType;
            public List<string> Processes;
            public string strParam, StrdwParam1;
            public uint dwParam1, dwParam2, dwParam3;

            public MsgOpenedProcesses(byte[] Buffer, GuardShield Guard = null)
            {
                if (Buffer != null && Buffer.Length >= 4 + 1 && BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgOpenedProcesses)
                {
                    if (HdKeyVaild)
                    {
                        if (GCipher.HandleBuffer(ref Buffer, true) == 0)
                            return;
                        if (Guard != null)
                            Guard.HandleOwner(Buffer);
                        BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                        rdr.BaseStream.Seek(4, SeekOrigin.Current);
                        var SwitchType = rdr.ReadByte();
                        if (!Enum.IsDefined(typeof(Type), SwitchType))
                        {
                            Console.WriteLine("Erorr Defined OpenedProcesses > ActionType " + SwitchType);
                            rdr.Close();
                            return;
                        }
                        ActionType = (Type)SwitchType;
                        if (ActionType == Type.Insert)
                        {
                            int count = rdr.ReadUInt16();
                            Processes = new List<string>(count);
                            for (int i = 0; i < count; i++)
                                Processes.Add(System.Text.Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadUInt16())).Replace("\0", ""));
                        }
                        else if (ActionType == Type.Insert_ProcessQuery)
                        {
                            strParam = System.Text.Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadUInt16())).Replace("\0", "");
                            dwParam1 = rdr.ReadUInt32();
                            dwParam2 = rdr.ReadUInt32();
                            dwParam3 = rdr.ReadUInt32();
                            if (dwParam3 > 0)
                                StrdwParam1 = dwParam3.ToString("x");
                        }
                        rdr.Close();
                    }
                }
            }
        }
    }

    public class GuardCipher
    {
        public int PublicSeed = 0;
        public GuardCipher(bool IsGameServer = false)
        {
            this.IsGameServer = IsGameServer;
            this.PublicSeed = new Random().Next();
            MsgGuardShield.Set_IV_Keys(PublicSeed);
        }
        public int HandleBuffer(ref byte[] data, bool isDecrypt)
        {
            if (data == null || data.Length <= 4)
                return 0;
            if (!isDecrypt)
                PadToMultipleOf(ref data);

            int length = System.BitConverter.ToUInt16(data, 0);
            if (data.Length - 8 != length && data.Length != length)
                throw new Exception("Invalid buffer length");

            if ((length - PADDING_SIZE) % 16 != 0)
                throw new Exception("Invalid buffer Padding");

            var result_Length = isDecrypt
               ? MsgGuardShield.GuardDecrypt(data, length)
               : MsgGuardShield.GuardEncrypt(data, length);
            return result_Length;
        }

        private void PadToMultipleOf(ref byte[] src, int pad = PADDING_SIZE * 4)
        {
            int OldLength = src.Length - PADDING_SIZE - PADDING_REDUCE;
            if (OldLength % pad == 0)
            {
                return;
            }
            int len = (src.Length + pad - 1) / pad * pad;
            Array.Resize(ref src, len + PADDING_REDUCE + PADDING_SIZE);
            unsafe
            {
                fixed (byte* Buffer = src)
                    *((ushort*)(Buffer + 0)) = (ushort)(src.Length - PADDING_REDUCE);
            }
        }

        private bool IsGameServer = false;
        private const byte PADDING_SIZE = 4;
        private readonly int TQSeal_SIZE = 8;
        private int PADDING_REDUCE
        {
            get { return IsGameServer ? TQSeal_SIZE : 0; }
        }
    }
    public class srand
    {
        private int _seed = 0;
        public short rand()
        {
            _seed *= 0x343fd;
            _seed += 0x269ec3;
            return (short)((_seed >> 0x10) & 0x7fff);
        }
        public srand(int seed)
        {
            _seed = seed;
        }
    }
}
