using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;
using Transitions;




namespace FrmMenssager
{
    public partial class Form1 : Form
    {
        static private NetworkStream stream;
        static private StreamWriter streamw;
        static private StreamReader streamr;
        static private TcpClient cliente = new TcpClient();
        static private string nick = "Desconocida";

        private delegate void DaddItem(String s);
        private void AddItem(string s)
        {
            listBox1.Items.Add(s);
        }
        public Form1()
        {
            InitializeComponent();
        }

        void Listen()
        {
            while (cliente.Connected)
            {
                try
                {
                    this.Invoke(new DaddItem(AddItem), streamr.ReadLine());
                }
                catch (Exception)
                {
                    MessageBox.Show("No se ha podido establecer una conexion con el servidor");
                    Application.Exit();
                }
            }
        }

        void Conectar()
        {
            try
            {
                cliente.Connect("127.0.0.1",8000);
                if (cliente.Connected)
                {
                    Thread t = new Thread(Listen);

                    stream=cliente.GetStream();
                    streamw = new StreamWriter(stream);
                    streamr = new StreamReader(stream);

                    streamw.WriteLine(nick);
                    streamw.Flush();

                    t.Start();

                }
                else
                {
                    MessageBox.Show("Servidor no Disponible! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Servidor no disponible");
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 329; 250 -position
            btnEnviar.Location = new Point(-329, 250);
            listBox1.Location = new Point(-329, 23);
            txtMensaje.Location = new Point(-329, 250);



        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            streamw.WriteLine(txtMensaje.Text);
            streamw.Flush();
            txtMensaje.Clear();
        }

        private void btbnConectar_Click(object sender, EventArgs e)
        {
            nick = txtUsuario.Text;
            Conectar();

            Transition t = new Transition(new TransitionType_EaseInEaseOut(900));
            t.add(labelUsuario, "Left",555);
            t.add(txtUsuario, "Left", 555);

            t.add(btnEnviar, "Left", 283);
            t.add(btbnConectar, "Left", 555);
            t.add(listBox1, "Left", 26);
            t.add(txtMensaje, "Left", 26);
            t.run();
        }
    }
}
