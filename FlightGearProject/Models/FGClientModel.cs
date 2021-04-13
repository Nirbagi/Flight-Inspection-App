using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace FlightGearProject.Models
{
    public class FGClientModel
    {
        // FlightGear IP Address & Port
        public String FGIp { get; set; } = "127.0.0.1";
        public int FGPort { get; set; } = 5400;//= 5400;
        public String CsvPath { get; set; } 
         = "C:/Users/Nir/Documents/Advance Prog 2/Project 1/FGClient/FGClient/reg_flight.csv";
        // Transmit speed to FlightGear - controls playback speed
        public int TranSpeed { get; set; } = 100;       
        public int CsvLineNum { get; set; } = 0;
        public int VideoSize { get; set; } = 0;
        // True - play forward ; False - play backwards
        public bool ForwardBackwardFlag { get; set; } = true;
        public bool PauseFlag { get; set; } = false;
        public TcpClient FGC { get; set; } = new TcpClient();
        public NetworkStream NetStream { get; set; }
        public String[] FileLines { get; set; }

        // this function initiate connection to FlightGear based on given settings from the user.
        public bool InitFGClient()
        {
            try
            {                
                FGC.Connect(FGIp, FGPort);
                NetStream = FGC.GetStream();
                FileLines = File.ReadAllLines(CsvPath);
                VideoSize = FileLines.Length;
                return true;
            }
            catch(Exception ex)
            {
               MessageBox.Show("Could not connect to server: " + ex.Message);
                return false;
            }
        }
        
        // this function starts the playback by sending lines from given csv file (contains data recorded from desired flight)
        // to FlightGear
        public void StartPlayCSV()
        {
            String nl = "\r\n";
            String line;
            String lineWNL;
            byte[] sendBytes;
            while (CsvLineNum < VideoSize)
            {                
                if (PauseFlag)
                    return;
                line = FileLines[CsvLineNum];
                lineWNL = String.Concat(line, nl);
                sendBytes = ASCIIEncoding.ASCII.GetBytes(lineWNL);
                NetStream.Write(sendBytes, 0, sendBytes.Length);
                NetStream.Flush();
                if (ForwardBackwardFlag)
                    CsvLineNum++;
                else
                {
                    CsvLineNum--;
                    // stop playback if reached to the beginning
                   if (CsvLineNum <= 0)
                        return;
                }
                    
                Thread.Sleep(TranSpeed);
            }
            // if reached to the end of the simulation - move back to the start and stop playing                        
            CsvLineNum = 0;
            line = FileLines[CsvLineNum];
            lineWNL = String.Concat(line, nl);
            sendBytes = ASCIIEncoding.ASCII.GetBytes(lineWNL);
            NetStream.Write(sendBytes, 0, sendBytes.Length);
            NetStream.Flush();            
            return;
        }
        public void CloseFGC()
        {
            FGC.Close();
            NetStream.Close();
        }
    }
}
