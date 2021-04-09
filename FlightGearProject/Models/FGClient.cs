using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace FlightGearProject.Models
{
    public class FGClient
    {
        public String FGIp { get; set; } //= "127.0.0.1";
        public int FGPort { get; set; } //= 5400;
        public String CsvPath { get; set; } 
        // = "C:/Users/Nir/Documents/Advance Prog 2/Project 1/FGClient/FGClient/reg_flight.csv";
        public int TranSpeed { get; set; } = 100;
        public int Location { get; set; } = 0;
        public int VideoSize { get; set; } = 0;
        public bool FBFlag { get; set; } = true;
        public bool PauseFlag { get; set; } = false;
        public TcpClient FGC { get; set; } = new TcpClient();
        public NetworkStream NetStream { get; set; }
        public String[] FileLines { get; set; }
        public void InitFGClient()
        {
            try
            {                
                FGC.Connect(FGIp, FGPort);
                NetStream = FGC.GetStream();
                FileLines = File.ReadAllLines(CsvPath);
                VideoSize = FileLines.Length;   
            }
            catch(Exception ex)
            {
               MessageBox.Show("Could not connect to server: " + ex.Message);
            }
        }
               
        public void StartPlayCSV()
        {
            while (Location < VideoSize)
            {
                String nl = "\r\n";
                if (PauseFlag)
                    continue;
                String line = FileLines[Location];
                String lineWNL = String.Concat(line, nl);
                byte[] sendBytes = ASCIIEncoding.ASCII.GetBytes(lineWNL);
                NetStream.Write(sendBytes, 0, sendBytes.Length);
                NetStream.Flush();
                if (FBFlag)
                    Location++;
                else if (!FBFlag && Location > 0)
                    Location--;
                Thread.Sleep(TranSpeed);
            }
        }
        public void CloseFGC()
        {
            FGC.Close();
            NetStream.Close();
        }
    }
}
