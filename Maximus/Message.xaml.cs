using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Maximus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Message : Page
    {
        private MqttClient client;
        byte[] message;
        string mainTopic;

        public Message()
        {
            this.InitializeComponent();
            // messageBox.Items.Add("hello");
            try
            {
                this.client = new MqttClient("broker.mqttdashboard.com");
                this.client.Connect(Guid.NewGuid().ToString());
                Debug.WriteLine("using broker.mqttdashboard.com");

            }
            catch (Exception)
            {
                try
                {
                    this.client = new MqttClient("iot.eclipse.org");
                    this.client.Connect(Guid.NewGuid().ToString());
                    Debug.WriteLine("using iot.eclipse.org");
                }
                catch (Exception)
                {
                    Debug.WriteLine("BROKER DOWN");
                }
            }

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
                string receivedData = parsingMsg.ToString();
                messageBox.Items.Add(receivedData);
            });
        }



        private void topicBox_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var combo = (ComboBox)sender;
            var item = (ComboBoxItem)combo.SelectedItem;
            try
            {
                mainTopic = item.ToString();
            }
            catch (Exception)
            {
                mainTopic = "/LightSensor";
                
            }
            this.client.Subscribe(new string[] { mainTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
    }
}
