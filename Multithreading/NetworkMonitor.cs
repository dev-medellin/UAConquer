using System.Threading;

 
namespace TheChosenProject.Multithreading
{
    public sealed class NetworkMonitor
    {
        private long totalRecvBytes;

        private int totalRecvPackets;

        private long totalSentBytes;

        private int totalSentPackets;

        private int recvBytes;

        private int recvPackets;

        private int sentBytes;

        private int sentPackets;

        public int PacketsSent => sentPackets;

        public int PacketsRecv => recvPackets;

        public int BytesSent => sentBytes;

        public int BytesRecv => recvBytes;

        public long TotalPacketsSent => totalSentPackets;

        public long TotalPacketsRecv => totalRecvPackets;

        public long TotalBytesSent => totalSentBytes;

        public long TotalBytesRecv => totalRecvBytes;

        public string UpdateStats(int interval)
        {
            double download;
            download = (double)recvBytes / (double)interval * 8.0 / 1024.0;
            double upload;
            upload = (double)sentBytes / (double)interval * 8.0 / 1024.0;
            int sent;
            sent = sentPackets;
            int recv;
            recv = recvPackets;
            Interlocked.Exchange(ref recvBytes, 0);
            Interlocked.Exchange(ref sentBytes, 0);
            Interlocked.Exchange(ref recvPackets, 0);
            Interlocked.Exchange(ref sentPackets, 0);
            return $"Network(↑{upload:F2} kbps [{sent:0000}], ↓{download:F2} kbps [{recv:0000}])";
        }

        public void Send(int aLength)
        {
            Interlocked.Increment(ref sentPackets);
            Interlocked.Increment(ref totalSentPackets);
            Interlocked.Add(ref sentBytes, aLength);
            Interlocked.Add(ref totalSentBytes, aLength);
        }

        public void Receive(int aLength)
        {
            Interlocked.Increment(ref recvPackets);
            Interlocked.Increment(ref totalRecvPackets);
            Interlocked.Add(ref recvBytes, aLength);
            Interlocked.Add(ref totalRecvBytes, aLength);
        }
    }
}
