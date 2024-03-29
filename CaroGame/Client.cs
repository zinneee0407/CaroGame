﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    class Client
    {
        // khai báo thông tin server
        //debug
        //private static string serverIp = "127.0.0.1";
        // real server
        private static string serverIp = "159.89.193.234";
        private static int serverPort = 12345;

        // tạo endpoint(điểm cuối giao tiếp) gồm ip và port của server
        //private static IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        private static TcpClient client = new TcpClient(serverIp, serverPort);

        // kiểm tra
        public static bool checkLogin = false;
        public static bool checkRegister = false;
        public static bool checkCreateRoom = false;
        public static bool checkJoinRoom = false;


        // thông tin user
        public static string user_id;
        public static string host_id;
        public static string join_id;
        public static string room_no;
        //public static string user_session = "";


        public static Label join_label;
        public static Label host_label;
        public static Label waiting_label;
        
        // khai báo kết nối
        //public static UdpClient client = null;
        //private static IPEndPoint serverEP = null;

        // khai báo worker
        public static BackgroundWorker workerListener = null;
        public static BackgroundWorker workerWaitForPlayer = null;
        public static BackgroundWorker workerChangeTurn = null;
        public static BackgroundWorker workerRefreshRoom = null;

        public static void InitClient()
        {
            // tạo udpclient
            //client = new UdpClient();

            // cho phép cancel worker
            workerListener = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            workerWaitForPlayer = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            workerChangeTurn = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            workerRefreshRoom = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            // thêm công việc cho worker
            workerListener.DoWork += DoReceiver;
            workerWaitForPlayer.DoWork += DoWaitForPlayer;
            workerChangeTurn.DoWork += DoChangeTurn;
            workerRefreshRoom.DoWork += DoRefreshRoom;

            // start worker
            workerListener.RunWorkerAsync();
        }

        public static void Login(string user_id, string user_pass)
        {
            string message = "login:" + user_id + ":" + user_pass;
            //try
            //{
                SendData(message);
            //} catch (Exception ex)
            //{
            //    MessageBox.Show("Cant connect to server");
            //} 
            
        }

        public static void Register(string user_id, string user_pass)
        {
            string message = "register:" + user_id + ":" + user_pass;
            SendData(message);
        }

        public static void Play(string user_id, string room_no, int x, int y)
        {
            string message = "play:" + user_id + ":" + room_no + ":" + x + ":" + y;
            SendData(message);
        }

        public static void CreateRoom(string user_id)
        {
            Random random = new Random();
            room_no = Convert.ToString(random.Next(1, 10000));
            string message = "create:" + user_id + ":" + room_no;
            SendData(message);
        }

        public static void JoinRoom(string user_id, string room_no)
        {
            string message = "join:" + user_id + ":" + room_no;
            SendData(message);
        }


        public static void UserOnline(string user_id)
        {
            string message = "online:" + user_id;
            SendData(message);
        }


        public static void UserOffline(string user_id)
        {
            string message = "offline:" + user_id;
            SendData(message);
        }


        private static void RefreshRoom()
        {
            string message = "refreshroom";
            SendData(message);
        }


        private static void SendData(string message)
        {
            // gửi dữ liệu lên server
            // chuyển dữ liệu từ string thành bytes
            byte[] data = Encoding.ASCII.GetBytes(message);

            // tạo 1 stream để để đọc ghi
            NetworkStream stream = client.GetStream();

            // gửi
            stream.Write(data, 0, data.Length);
            //client.Send(messageEncode, messageEncode.Length);

            //// nhận dữ liệu từ server
            //// tạo buffer lưu trữ dữ liệu nhận đc
            //data = new byte[256];

            //// đọc dữ liệu nhận về
            //string response = string.Empty;
            //int bytes = stream.Read(data, 0, data.Length);
            //response = Encoding.ASCII.GetString(data, 0, bytes);

            //// xử lý dữ liệu nhận về
            //string[] rp = response.Split(':');
            //switch (rp[0])
            //{
            //    case "play":
            //        int x = Convert.ToInt32(rp[1]);
            //        int y = Convert.ToInt32(rp[2]);

            //        if (Map.player_turn == 1)
            //        {
            //            BanCo.DanhCo(x, y, 2, Map.grs);
            //        }
            //        else if (Map.player_turn == 2)
            //        {
            //            BanCo.DanhCo(x, y, 1, Map.grs);
            //        }

            //        Map.turn++;
            //        //MessageBox.Show(Convert.ToString(Map.turn));
            //        break;
            //    case "login":
            //        if (rp[1].Equals("true"))
            //        {
            //            checkLogin = true;
            //        }
            //        break;
            //    case "register":
            //        if (rp[1].Equals("true"))
            //        {
            //            checkRegister = true;
            //        }
            //        break;
            //    case "create":
            //        if (rp[1].Equals("true"))
            //        {
            //            checkCreateRoom = true;
            //        }
            //        break;
            //    case "join":
            //        if (rp[1].Equals("true"))
            //        {
            //            host_id = rp[2];

            //            // set player turn
            //            Map.player_turn = Convert.ToInt32(rp[3]);

            //            // set turn = 0 (bắt đầu game)
            //            Map.turn = 0;

            //            checkJoinRoom = true;
            //        }
            //        else
            //        {
            //            MessageBox.Show("Phòng không tồn tại");
            //        }
            //        break;
            //    case "host":
            //        if (rp[1].Equals(user_id))
            //        {
            //            join_id = rp[2];

            //            // set player turn
            //            Map.player_turn = Convert.ToInt32(rp[3]);
            //        }
            //        break;
            //}

            // đóng stream
            //stream.Close();
        }


        private static void DoReceiver(object sender, DoWorkEventArgs e)
        {
            // connect to server
            // try catch to check server on/off
            //client.Connect(serverEP);

            while (true)
            {
                // cancel worker nếu có tín hiệu cancel gửi đến
                if (workerListener.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // nhận dữ liệu từ server
                // tạo buffer lưu trữ dữ liệu nhận đc
                byte[] data = new byte[256];

                NetworkStream stream = client.GetStream();

                // đọc dữ liệu nhận về
                string response = string.Empty;
                int bytes = stream.Read(data, 0, data.Length);
                response = Encoding.ASCII.GetString(data, 0, bytes);
                string[] rp = response.Split(':');

                /// <summary>
                /// play:user_session:user_id:x:y
                /// login:user_id:user_pass
                /// register:user_id:user_pass
                /// </summary>
                switch (rp[0])
                {
                    case "play":
                        int x = Convert.ToInt32(rp[1]);
                        int y = Convert.ToInt32(rp[2]);

                        if (Map.player_turn == 1)
                        {
                            //BanCo.DanhCo(x, y, 2, Map.grs);
                        }
                        else if (Map.player_turn == 2)
                        {
                            //BanCo.DanhCo(x, y, 1, Map.grs);
                        }

                        Map.turn++;
                        //MessageBox.Show(Convert.ToString(Map.turn));
                        break;
                    case "login":
                        if (rp[1].Equals("true"))
                        {
                            checkLogin = true;
                        }
                        break;
                    case "register":
                        if (rp[1].Equals("true"))
                        {
                            checkRegister = true;
                        }
                        break;
                    case "create":
                        if (rp[1].Equals("true"))
                        {
                            checkCreateRoom = true;
                        }
                        break;
                    case "join":
                        if (rp[1].Equals("true"))
                        {
                            host_id = rp[2];

                            // set player turn
                            Map.player_turn = Convert.ToInt32(rp[3]);

                            // set turn = 0 (bắt đầu game)
                            Map.turn = 0;

                            checkJoinRoom = true;
                        }
                        else
                        {
                            MessageBox.Show("Phòng không tồn tại");
                        }
                        break;
                    case "host":
                        if (rp[1].Equals(user_id))
                        {
                            join_id = rp[2];

                            // set player turn
                            Map.player_turn = Convert.ToInt32(rp[3]);
                        }
                        break;
                }
            }
        }




        private static void DoWaitForPlayer(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // cancel worker nếu có tín hiệu cancel gửi đến
                if (workerWaitForPlayer.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                if (join_id != null)
                {
                    // xóa dòng "Chờ người chơi"
                    waiting_label.Invoke((Action) delegate
                    {
                        waiting_label.Text = "";
                    });

                    // hiện tên người chơi vào phòng
                    join_label.Invoke((Action) delegate
                    {
                        join_label.Text = join_id;
                    });

                    // set turn = 0 (bắt đầu game)
                    Map.turn = 0;

                    // dừng worker
                    workerWaitForPlayer.CancelAsync();
                }

                Thread.Sleep(100);
            }
        }




        private static void DoChangeTurn(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // cancel worker nếu có tín hiệu cancel gửi đến
                if (workerChangeTurn.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (((Map.player_turn == 1) && (Map.turn % 2 == 0)) ||
                    ((Map.player_turn == 2) && (Map.turn % 2 > 0)))
                {
                    if (user_id.Equals(host_id))
                    {
                        // đổi màu nền tên người chơi
                        host_label.Invoke((Action)delegate
                        {
                            host_label.BackColor = Color.Green;
                        });

                        join_label.Invoke((Action)delegate
                        {
                            join_label.BackColor = Color.Transparent;
                        });
                    }

                    if (user_id.Equals(join_id))
                    {
                        // đổi màu nền tên người chơi
                        join_label.Invoke((Action)delegate
                        {
                            join_label.BackColor = Color.Green;
                        });

                        host_label.Invoke((Action)delegate
                        {
                            host_label.BackColor = Color.Transparent;
                        });
                    }
                    
                } else if (((Map.player_turn == 1) && (Map.turn % 2 > 0)) ||
                    ((Map.player_turn == 2) && (Map.turn % 2 == 0)))
                {
                    if (user_id.Equals(host_id))
                    {
                        // đổi màu nền tên người chơi
                        host_label.Invoke((Action)delegate
                        {
                            host_label.BackColor = Color.Transparent;
                        });

                        join_label.Invoke((Action)delegate
                        {
                            join_label.BackColor = Color.Red;
                        });
                    }

                    if (user_id.Equals(join_id))
                    {
                        // đổi màu nền tên người chơi
                        join_label.Invoke((Action)delegate
                        {
                            join_label.BackColor = Color.Transparent;
                        });

                        host_label.Invoke((Action)delegate
                        {
                            host_label.BackColor = Color.Red;
                        });
                    }
                }

                // chạy kiểm tra mỗi 0.1s
                Thread.Sleep(100);
            }
        }



        private static void DoRefreshRoom(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // cancel worker nếu có tín hiệu cancel gửi đến
                if (workerRefreshRoom.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // do something to refresh roomlist here
            }
        }



    }
}
