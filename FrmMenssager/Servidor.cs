using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace FrmMenssager
{
     class Servidor_Chat
    {
        //Adquiere la conexion para ingresar al servidor
        private TcpListener server;
        private TcpClient client = new TcpClient();
        private IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 8000);
        private List<connetion> list = new List<connetion>();

        connetion con;

        private struct connetion
        {
            public NetworkStream stream; //Establece la conexion 
            public StreamWriter streamw;  //Lee 
            public StreamReader streamr; //Escribe 
            public string nick; //Variable para poder identificar
        }

        public Servidor_Chat()
        {
            inicio();
        }

        public void inicio()
        {
            Console.WriteLine("Servidor Activado!");
            server = new TcpListener(ipendpoint);
            server.Start();
            while (true)
            {
                client = server.AcceptTcpClient(); //Acepta el tcp
                con = new connetion(); //Estalece la conexion
                con.stream=client.GetStream();
                con.streamr=new StreamReader(con.stream); //Leer la escritura del stream
                con.streamw=new StreamWriter(con.stream); //Escribir la lectura del stream

                con.nick = con.streamr.ReadLine();
                list.Add(con);
                Console.WriteLine(con.nick + "Se a conectado");
                Thread t = new Thread(Escuchar_Conexion);
                t.Start();
            }
        }

        void Escuchar_Conexion() //Recive la conexion 
        {
            connetion hcon = con;
            do
            {
                try
                {
                    string tmp = hcon.streamr.ReadLine();
                    Console.WriteLine(hcon.nick+": "+ tmp);
                    foreach (connetion c in list)
                    {
                        try
                        {
                            c.streamw.WriteLine(hcon.nick + ": " + tmp);
                            c.streamw.Flush();
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    list.Remove(hcon);
                    Console.WriteLine(con.nick + "se a desconectado.");
                    break;
                }
            } while (true);
        }
    }
}
