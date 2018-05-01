﻿using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Bank
{
    public class Server
    {
	    private readonly TcpListener server;
	    public  readonly ObservableCollection<User> Users;

	    public bool Running;

	    public Server(string address = null, int port = 13000)
	    {
		    var ip = address == null 
			    ? IPAddress.Any 
			    : IPAddress.Parse(address); 

			Users  = new ObservableCollection<User>();
		    server = new TcpListener(ip, port);
			Running = false;
	    }

	    public bool Start()
	    {
		    try
		    {
			    server.Start();
			    var thread = new Thread(ServerThread);
				thread.Start();
			}
		    catch (SocketException)
		    {
			    server.Stop();
				return false;
		    }

		    Running = true;
		    return true;
	    }

	    private void ServerThread()
	    {
		    var bytes = new byte[256];

		    while (Running)
		    {
				// Blocking call to accept requests
			    TcpClient client = null;
			    try
			    {
				    client = server.AcceptTcpClient();
			    }
			    catch (SocketException)
			    {
				    client?.Close();
				    continue;
			    }
			    catch (TargetInvocationException)
			    {
					client?.Close();
					continue;
			    }

			    // Stream object for reading/writing
			    var stream = client.GetStream();

				// Get all data sent by client
			    int i;
			    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
			    {
				    var data = Encoding.ASCII.GetString(bytes, 0, i);
				    var ok   = Encoding.ASCII.GetBytes("OK");

					if (data == "STOP")
				    {
						stream.Write(ok, 0, ok.Length);
					    Running = false;

					    // UWP needs this to run on the main thread
					    if (Device.RuntimePlatform == Device.UWP)
						    Device.BeginInvokeOnMainThread(() => Users.Clear());
					    else
						    Users.Clear();

						break;
				    }

				    var dat = data.Split(',');

				    if (dat[0] == "JOIN")
				    {
					    var user = new User(dat[1], int.Parse(dat[2]), ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString());

						// UWP needs this to run on the main thread
						if (Device.RuntimePlatform == Device.UWP)
							Device.BeginInvokeOnMainThread(() => Users.Add(user));
						else
							Users.Add(user);
				    }

					// Send 'OK' back to the client
				    stream.Write(ok, 0, ok.Length);
			    }

				client.Close();
		    }

			server.Stop();
		}
    }
}
