using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    private Socket _tcpSocket;
    private UdpClient _udpClient;
    private IPAddress _serverIp;
    private const string ServerDomain = "zecocostudio.com";
    private const string ServerIP = "221.151.106.33";
    private const int ServerPort = 51234;
    private const int ServerLocalPort = 51235;
    private IPEndPoint _serverIpEndPoint;
    private bool _isFinishConnectServer = false;
    private bool _isSuccessConnectServer = false;
    public TMP_Text _text;

    private const int MaxPacketSize = 1500;
    struct TestSturct : IConvertBytes
    {
        private int a;
        private int b;

        public TestSturct(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public void ToBytes(WriteMemoryStream writeMemoryStream)
        {
            writeMemoryStream.WriteByInt32(a);
            writeMemoryStream.WriteByInt32(b);
        }

        public void ToData(ReadMemoryStream readMemoryStream)
        {
            a = readMemoryStream.ReadByInt32();
            b = readMemoryStream.ReadByInt32();
        }
    }
    private void Start()
    {
        Byte[] aa = new byte[500];
        for (int i = 0; i < 500; ++i)
        {
            aa[i] = (byte)i;
        }

        WriteMemoryStream writeMemoryStream = new WriteMemoryStream();
        TestSturct test = new TestSturct(10,20);
        
        writeMemoryStream.WriteByObject(test);
        ReadMemoryStream readMemoryStream = new ReadMemoryStream(writeMemoryStream.GetValidBuffer());
        Debug.Log(readMemoryStream.ReadByInt16());
        IConvertBytes testbb = new TestSturct();
        readMemoryStream.ReadByObject(testbb);
    }

    public Int32 ReadByInt32(Byte[] bytes)
    {
        Byte[] data = new byte[sizeof(Int32)];
        return BitConverter.ToInt32(bytes);
    }

    void OnApplicationQuit()
    {
        DisconnectServer();
    }

    // 서버 컴퓨터 서버
    public void StartConnectServer()
    {
        ConnectServer(ServerPort);
    }

    // 개인 컴퓨터 서버
    public void StartConnectLocalServer()
    {
        ConnectServer(ServerLocalPort);
    }

    private void ConnectServer(int port)
    {
        try
        {
            _serverIp = Dns.GetHostEntry(ServerDomain).AddressList[0];
        }
        catch (Exception e)
        {
            Debug.LogError("해당 도메인을 찾을 수 없습니다");
            throw;
        }

        _serverIpEndPoint = new IPEndPoint(_serverIp, port);
        _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _tcpSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        //_socket.Connect(_serverIpEndPoint);
        _tcpSocket.SendTimeout = 3000;
        _tcpSocket.ReceiveTimeout = 3000;
        StartCoroutine(LoadingConnectServerCoroutine());
        _tcpSocket.BeginConnect(_serverIpEndPoint, new AsyncCallback(Connected), _tcpSocket);
    }

    public void ConnectServerUdp()
    {
        ConnectServerToUdp(ServerPort);
    }

    public void ConnectServerLocalServerUdp()
    {
        
        ConnectServerToUdp(ServerLocalPort);
    }

    private void ConnectServerToUdp(int port)
    {
        try
        {
            foreach (IPAddress ip in Dns.GetHostEntry(ServerDomain).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _serverIp = ip;
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("해당 도메인을 찾을 수 없습니다");
            throw;
        }

        _serverIpEndPoint = new IPEndPoint(_serverIp, port);
        
        _text.text += _serverIp.ToString();
        _udpClient = new UdpClient();
    }

    private IEnumerator LoadingConnectServerCoroutine()
    {
        string text = "서버에 연결 시도 중";
        while (!_tcpSocket.Connected && !_isFinishConnectServer)
        {
            text += ".";
            Debug.Log(text);
            yield return new WaitForSeconds(1);
        }
    }

    private void Connected(IAsyncResult iar)
    {
        _tcpSocket = (Socket) iar.AsyncState;
        try
        {
            _tcpSocket.EndConnect(iar);
            _isSuccessConnectServer = true;
        }
        catch (SocketException)
        {
            Debug.Log("연결 실패");
        }

        _isFinishConnectServer = true;
    }

    private uint i = 0;
    private void Update()
    {
        ReceiveTCP();
        ReceiveUDP();
        if (_isSuccessConnectServer)
        {
            _isSuccessConnectServer = false;
            Invoke("SendStartMatchingPacket", 1);
        }

        if (_udpClient != null)
        {
            byte[] data = BitConverter.GetBytes(i++);
            _udpClient.Send(data, data.Length, _serverIpEndPoint);
        }
    }

    void SendPacket(object packet)
    {
        if (packet == null)
        {
            Debug.LogError("패킷이 없습니다");
        }

        try
        {
            byte[] sendPacket = StructToByteArray(packet);
            _tcpSocket.Send(sendPacket, 0, sendPacket.Length, SocketFlags.None);
            Debug.Log($"{sendPacket.Length.ToString()}byte 사이즈 데이터 전송");
            Logger logger;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    void ReceiveTCP()
    {
        int receive = 0;
        if (_tcpSocket == null || !_tcpSocket.Connected)
        {
            return;
        }

        if (_tcpSocket.Available == 0)
        {
            return;
        }

        byte[] packet = new byte[MaxPacketSize];
        try
        {
            receive = _tcpSocket.Receive(packet);
        }
        catch (Exception ex)
        {
            //Debug.Log(ex.ToString());
            return;
        }

        if (receive <= 0)
        {
            return;
        }

        int size = BitConverter.ToUInt16(packet);
        byte[] bytes = new byte[size];
        Debug.Log($"{size}의 데이터를 받았습니다");
        Array.Copy(packet, 0, bytes, 0, size);
    }

    private int ii = 0;
    void ReceiveUDP()
    {
        int receive = 0;
        if (_udpClient == null)
        {
            return;
        }

        if (_udpClient.Available == 0)
        {
            return;
        }

        byte[] packet;
        IPEndPoint epRemote = new IPEndPoint(IPAddress.Any, 0);
        try
        {
            packet = _udpClient.Receive(ref epRemote);
        }
        catch (Exception ex)
        {
            //Debug.Log(ex.ToString());
            return;
        }

        if (packet.Length <= 0)
        {
            return;
        }

        uint size = BitConverter.ToUInt32(packet);
        if (ii++ != size)
        {
            Debug.Log("패킷 손실");
        }
    }

    public void DisconnectServer()
    {
        if (_tcpSocket == null)
            return;

        if (_tcpSocket.Connected)
        {
            _tcpSocket.Close();
            _tcpSocket = null;
        }
    }

    byte[] StructToByteArray(object obj)
    {
        int size = Marshal.SizeOf(obj);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    T ByteArrayToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T) Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }
}
