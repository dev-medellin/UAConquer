using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.WindowsAPI;
using Extensions;
using TheChosenProject.Cryptography;
using TheChosenProject.Client;
using TheChosenProject.Ai;

namespace TheChosenProject.ServerSockets
{
    public class SecuritySocket
    {
        public ReceiveBuffer ReceiveBuff;

        public bool SetDHKey;

        public object SendRoot;

        public Action<SecuritySocket> OnDisconnect;

        private Action<SecuritySocket, Packet> OnReceiveHandler;

        public Socket Connection;

        public object Client;

        public Queue<byte[]> OnSend;

        public IDisposable[] TimerSubscriptions;

        public bool Alive;

        public GameCryptography Crypto;

        public Time32 LastReceive;

        public GameClient Game;

        public ServerSocket Server;

        public bool ConnectFull;

        public int tstgg;

        public ReceiveBuffer DHKeyBuffer = new ReceiveBuffer(1024, true);

        public ReceiveBuffer EncryptedDHKeyBuffer = new ReceiveBuffer(1024, true);

        private static byte[] Keys = new byte[4] { 18, 9, 17, 19 };

        public int nPutBytes;

        public byte[] packet;

        public object SynRoot = new object();

        public bool IsGameServer => Crypto != null;

        public string RemoteIp
        {
            get
            {
                try
                {
                    if (Connection == null)
                        return "NONE";
                    return (Connection.RemoteEndPoint as IPEndPoint).Address.ToString();
                }
                catch
                {
                    return "NONE";
                }
            }
        }

        public SecuritySocket(ServerSocket serversocket, Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            Server = serversocket;
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;
        }

        public SecuritySocket(Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;
        }

        public bool Connect(string IPAddres, ushort port, out Socket _socket)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult asyncResult;
            asyncResult = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IPAddres), port), null, null);
            uint count;
            count = 0u;
            while (!asyncResult.IsCompleted && count < 10)
            {
                count++;
                Thread.Sleep(100);
            }
            if (asyncResult.IsCompleted)
            {
                _socket.Blocking = false;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Socket obj;
                obj = _socket;
                int receiveBufferSize;
                receiveBufferSize = (_socket.SendBufferSize = 2048);
                obj.ReceiveBufferSize = receiveBufferSize;
                SocketPoll.ConnectionPoll.Add(this);
            }
            return asyncResult.IsCompleted;
        }

        public void Create(Socket _socket)
        {
            try
            {
                ReceiveBuff = new ReceiveBuffer(ServerKernel.Port_ReceiveSize);
                Connection = _socket;
                SetDHKey = false;
                Alive = true;
                LastReceive = Time32.Now;
                SendRoot = new object();
                OnSend = new Queue<byte[]>();
                Client = (Crypto = null);
                TimerSubscriptions = new IDisposable[0];
                SocketPoll.ConnectionPoll.Add(this);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public void SetCrypto(GameCryptography Crypt)
        {
            Crypto = Crypt;
        }

        public unsafe void Send(Packet msg)
        {
            try
            {
                if (!Alive)
                    return;
                lock (SendRoot)
                {
                    byte[] _buffer;
                    _buffer = new byte[msg.Size];
                    fixed (byte* ptr2 = _buffer)
                    {
                        msg.memcpy(ptr2, msg.Memory, msg.Size);
                    }
                    if (Crypto != null)
                        Crypto.Encrypt(_buffer);
                    else
                    {
                        fixed (byte* ptr = _buffer)
                        {
                            msg.memcpy(ptr, msg.Memory, msg.Size);
                        }
                    }
                    OnSend.Enqueue(_buffer);
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
        
        public bool IsCompleteDHKey(out int type)
        {
            type = 0;
            try
            {
                if (DHKeyBuffer.Length() < 8)
                    return false;
                byte[] buffer;
                buffer = new byte[Packet.SealSize];
                for (int x = 0; x < buffer.Length; x++)
                {
                    buffer[x] = DHKeyBuffer.buffer[x + (DHKeyBuffer.Length() - Packet.SealSize)];
                }
                string Text;
                Text = Encoding.ASCII.GetString(DHKeyBuffer.buffer);
                bool accept;
                accept = Text.Contains("TQClient");
                if (!Text.EndsWith("TQClient"))
                    type = 1;
                return accept;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                Disconnect();
                return false;
            }
        }

        public unsafe bool ReceiveDHKey()
        {
            try
            {
                if (Alive)
                {
                    int rec_type;
                    rec_type = 0;
                    if (!SetDHKey && Alive)
                    {
                        SocketError Socket_Error;
                        Socket_Error = SocketError.IsConnected;
                        int length;
                        length = DHKeyBuffer.MaxLength() - DHKeyBuffer.Length();
                        if (length <= 0)
                        {
                            Disconnect();
                            return false;
                        }
                        int ret;
                        ret = Connection.Receive(DHKeyBuffer.buffer, DHKeyBuffer.Length(), length, SocketFlags.None, out Socket_Error);
                        if (ret > 0)
                        {
                            Buffer.BlockCopy(DHKeyBuffer.buffer, DHKeyBuffer.Length(), EncryptedDHKeyBuffer.buffer, EncryptedDHKeyBuffer.Length(), ret);
                            EncryptedDHKeyBuffer.AddLength(ret);
                            if (Crypto != null)
                            {
                                byte[] ptr2;
                                ptr2 = new byte[ret];
                                Buffer.BlockCopy(DHKeyBuffer.buffer, DHKeyBuffer.Length(), ptr2, 0, ret);
                                Crypto.Decrypt(ptr2);
                                Buffer.BlockCopy(ptr2, 0, DHKeyBuffer.buffer, DHKeyBuffer.Length(), ret);
                            }
                            DHKeyBuffer.AddLength(ret);
                            if (IsCompleteDHKey(out rec_type))
                            {
                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;
                                    stream = rec.GetStream();
                                    stream.Seek(0);
                                    fixed (byte* ptr = DHKeyBuffer.buffer)
                                    {
                                        stream.memcpy(stream.Memory, ptr, DHKeyBuffer.Length());
                                    }
                                    stream.Size = DHKeyBuffer.Length();
                                    OnReceiveHandler?.Invoke(this, stream);
                                }
                            }
                        }
                        else
                        {
                            if (ret == 0)
                            {
                                if (Socket_Error != SocketError.WouldBlock)
                                    Disconnect();
                                return false;
                            }
                            if (Socket_Error != SocketError.WouldBlock)
                                Disconnect();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                Disconnect();
            }
            return false;
        }

        public bool ReceiveBuffer()
        {
            if (Alive)
                try
                {
                    if (!ConnectFull)
                        return false;
                    if (SetDHKey || Crypto == null)
                    {
                        try
                        {
                            if (!Alive)
                                return false;
                            if (!Receive())
                                return false;
                        }
                        catch (Exception e2)
                        {
                            ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
                            Disconnect();
                        }
                        return true;
                    }
                    ReceiveDHKey();
                }
                catch (Exception e)
                {
                    Disconnect();
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
            return false;
        }

        public unsafe bool HandlerBuffer()
        {
            int counts;
            counts = 30;
            while (counts > 0)
            {
                counts--;
                if (!Alive)
                    return false;
                try
                {
                    if (!ConnectFull)
                        return false;
                    if (ReceiveBuff.Length() == 0)
                        return false;
                    int Length;
                    Length = ReceiveBuff.ReadHead() + (IsGameServer ? 8 : 0);
                    if (Length < 2)
                        return false;
                    if (Length > 1024)
                    {
                        Disconnect();
                        return false;
                    }
                    if (Length > ReceiveBuff.Length())
                        return false;
                    LastReceive = Time32.Now;
                    Packet Stream;
                    Stream = PacketRecycle.Take();
                    Stream.Seek(0);
                    fixed (byte* ptr = ReceiveBuff.buffer)
                    {
                        Stream.memcpy(Stream.stream, ptr, Length);
                        if (Length < ReceiveBuff.Length())
                        {
                            fixed (byte* ptr2 = &ReceiveBuff.buffer[Length])
                            {
                                void* next_buffer;
                                next_buffer = ptr2;
                                Stream.memcpy(ptr, next_buffer, ReceiveBuff.Length() - Length);
                            }
                        }
                        Stream.Size = Length;
                        ReceiveBuff.DelLength(Length);
                    }
                    Stream.SeekForward(2);
                    OnReceiveHandler?.Invoke(this, Stream);
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
            }
            return false;
        }

        public static void Decrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(Keys[i % 4] ^ data[i]);
            }
        }

        public bool Receive()
        {
            if (Alive)
            {
                SocketError Socket_Error;
                Socket_Error = SocketError.IsConnected;
                try
                {
                    int length;
                    length = ReceiveBuff.MaxLength() - ReceiveBuff.Length();
                    int ret;
                    ret = Connection.Receive(ReceiveBuff.buffer, ReceiveBuff.Length(), length, SocketFlags.None, out Socket_Error);
                    if (ret > 0)
                    {
                        if (Crypto != null)
                        {
                            byte[] ptr;
                            ptr = new byte[ret];
                            Buffer.BlockCopy(ReceiveBuff.buffer, ReceiveBuff.Length(), ptr, 0, ret);
                            Crypto.Decrypt(ptr);
                            Buffer.BlockCopy(ptr, 0, ReceiveBuff.buffer, ReceiveBuff.Length(), ret);
                        }
                        ReceiveBuff.AddLength(ret);
                        Program.NetworkMonitor.Receive(ret);
                        return true;
                    }
                    if (ret == 0)
                    {
                        if (Socket_Error != SocketError.WouldBlock)
                            Disconnect();
                        return false;
                    }
                    if (Socket_Error != SocketError.WouldBlock)
                        Disconnect();
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                    ServerKernel.Log.SaveLog(Socket_Error.ToString(), true, LogType.WARNING);
                }
            }
            return false;
        }

        public bool CanDequeue(out byte[] data)
        {
            data = null;
            if (OnSend.Count > 0)
            {
                lock (SendRoot)
                {
                    data = OnSend.Dequeue();
                }
            }
            return data != null;
        }

        public static bool TrySend(SecuritySocket _socket)
        {
            if (_socket.Alive)
            {
                if (!_socket.ConnectFull)
                    return false;
                if (_socket.OnSend.Count > 2500)
                {
                    ServerKernel.Log.SaveLog("the sync is " + _socket.OnSend.Count, true, LogType.WARNING);
                    _socket.Disconnect();
                    return false;
                }
                SocketError sError;
                sError = SocketError.Success;
                int ret;
                ret = 0;
                if (_socket.packet != null)
                    try
                    {
                        int nLen;
                        nLen = _socket.packet.Length;
                        ret = _socket.Connection.Send(_socket.packet, _socket.nPutBytes, nLen - _socket.nPutBytes, SocketFlags.None, out sError);
                        if (ret > 0)
                        {
                            _socket.nPutBytes += ret;
                            Program.NetworkMonitor.Send(ret);
                            if (_socket.nPutBytes < nLen)
                            {
                                if (sError != SocketError.WouldBlock)
                                {
                                    _socket.Disconnect();
                                    return false;
                                }
                                return false;
                            }
                            _socket.nPutBytes = 0;
                            _socket.packet = null;
                        }
                        else if (ret <= 0)
                        {
                            if (sError != SocketError.WouldBlock)
                            {
                                _socket.Disconnect();
                                return false;
                            }
                            return false;
                        }
                    }
                    catch (Exception e2)
                    {
                        ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
                        _socket.Disconnect();
                        return false;
                    }
                if (_socket.CanDequeue(out _socket.packet))
                    try
                    {
                        if (!_socket.Alive || _socket.packet == null)
                            return false;
                        int nLen2;
                        nLen2 = _socket.packet.Length;
                        ret = _socket.Connection.Send(_socket.packet, _socket.nPutBytes, nLen2 - _socket.nPutBytes, SocketFlags.None, out sError);
                        if (ret > 0)
                        {
                            _socket.nPutBytes += ret;
                            if (_socket.nPutBytes < _socket.packet.Length)
                            {
                                if (sError != SocketError.WouldBlock)
                                {
                                    _socket.Disconnect();
                                    return false;
                                }
                                return false;
                            }
                            _socket.packet = null;
                            _socket.nPutBytes = 0;
                            return true;
                        }
                        if (sError != SocketError.WouldBlock)
                        {
                            _socket.Disconnect();
                            return false;
                        }
                        return false;
                    }
                    catch (Exception e)
                    {
                        ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                        _socket.Disconnect();
                        return false;
                    }
            }
            return false;
        }

        public void Disconnect(string szMsg = "")
        {
            lock (SynRoot)
            {
                if (!Alive && Game.Player.OfflineTraining != MsgOfflineTraining.Mode.Completed)
                    return;
                if (szMsg != "")
                    ServerKernel.Log.SaveLog($"kickoutsocket {Game.Player.Name}:{szMsg}", true, LogType.MESSAGE);
                SocketPoll.ConnectionPoll.Remove(this);
                ServerKernel.ServerManager?.RemoveUser(Game.Player.UID);
                OnSend.Clear();
                Server?.Clients.Remove(this);
                Alive = false;
                if (TimerSubscriptions != null)
                {
                    for (int i = 0; i < TimerSubscriptions.Length; i++)
                    {
                        TimerSubscriptions[i]?.Dispose();
                    }
                }
                try
                {
                    ws2_32.shutdown(Connection.Handle, ws2_32.ShutDownFlags.SD_BOTH);
                    ws2_32.closesocket(Connection.Handle);
                    Connection.Dispose();
                    GC.SuppressFinalize(Connection);
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
                finally
                {
                    if (!Game.Player.ContainFlag(MsgUpdate.Flags.OfflineMode))
                        OnDisconnect?.Invoke(this);
                    if (Game.Player.OfflineTraining == MsgOfflineTraining.Mode.Completed)
                        OnDisconnect?.Invoke(this);
                }
            }
        }
    }
}
