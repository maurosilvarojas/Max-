using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tweetinvi;
using uPLibrary.Networking.M2Mqtt;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.UI.Core;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Maximus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Help : Page
    {
        private MqttClient client;
        byte[] message;
        string SOS_Status;
        public Help()
        {
            this.InitializeComponent();
            this.client = new MqttClient("broker.mqttdashboard.com");
            try
            {
                this.client.Connect(Guid.NewGuid().ToString());
                Debug.WriteLine("using broker.mqttdashboard.com");

            }
            catch (Exception)
            {
                try
                {
                    this.client = new MqttClient("iot.eclipse.org");
                    Debug.WriteLine("using iot.eclipse.org");
                }
                catch (Exception)
                {
                    Debug.WriteLine("BROKER DOWN");
                }
            }
            this.client.Connect(Guid.NewGuid().ToString());
            this.client.Subscribe(new string[] { "/SOS_Mode" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            this.client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }



        async void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            message = e.Message;
            StringBuilder parsingMsg = new StringBuilder();
            foreach (var value in message)
            {
                parsingMsg.Append(Char.ConvertFromUtf32(value));
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SOS_Status = parsingMsg.ToString();


            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string sosNotification = "S.O.S Mode Activated";
            string consumer_key = "k5KFkv92QqmDRbryMpEjrqpAL";
            string consumer_secret = "u3DAAYLcMhQnpqvx3rIoPPJw8R1Xhd5s4HTOEhiAIk4zGRxJmJ";
            string access_Token = "704418398449565696-6jlI6ECK1l46h00DS1S5T7iHlxEhCOZ";
            string access_Token_Secret = "xb2MkVGhI4r64AlXSLbNgDV4dqJH8EYocwaYXOLYDOwiv";

            Auth.SetUserCredentials(consumer_key, consumer_secret, access_Token, access_Token_Secret);
            DateTime localDate = DateTime.Now;
            Tweet.PublishTweet("Maximus IoT Notification : " + sosNotification + " at " + localDate + " #MaxIoT by @redmaurosilva");
            Debug.WriteLine("#testing Maximus IoT Automatic Notification : " + sosNotification + " at " + localDate + " #MaxIoT");



        }
    }
    
}
