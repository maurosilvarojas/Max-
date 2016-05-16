using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Threading;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Storage;
using System.Net;
using Tweetinvi;
using GIS = GHIElectronics.UWP.Shields;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.UI.Core;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Maximus
{
    /// <summary> //main client
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        string text = "Welcome to Maximus Io T";
        private GIS.FEZHAT hat;
        private DispatcherTimer timer;
        string illuminance;
        string currentTemperature;
        private bool next;
        private MqttClient client;
        byte[] message;
        string SOS_Status;
        string timeNow;






        public MainPage()
        {
            this.InitializeComponent();
            this.ListenForCommands();
            this.Setup();
            BackButton.Visibility = Visibility.Collapsed;
            MyFrame.Navigate(typeof(Control));
            TitleTextBlock.Text = "Control";
            Control.IsSelected = true;
            talker(text);
            DateTime localDate = DateTime.Now;
            timeNow = localDate.ToString();

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


        private async void talker(string phrase)
        {
            string Ssml =
            @"<speak version='1.0' " +
            "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US' rate='slow'>" +
            " <prosody contour='(0%,+80Hz) (10%,+80%) (40%,+80Hz)'></prosody> " +
            "<break time='500ms' />" + phrase +
            " <prosody rate='slow' contour='(0%,+20Hz) (10%,+30%) (40%,+10Hz)'></prosody>" +
            "</speak>";

            // The media object for controlling and playing audio.
            //MediaElement mediaElement = this.media;

            // The object for controlling the speech synthesis engine (voice).
            var synth = new SpeechSynthesizer();

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await synth.SynthesizeSsmlToStreamAsync(Ssml);

            // Send the stream to the media object.
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        async Task ListenForCommands()
        {
            var recognizer = new SpeechRecognizer();
            recognizer.Constraints.Add(new SpeechRecognitionListConstraint(new string[] {
                "Max, help me",
                "Max, call an ambulance",
                "Max, call the police",
                "Max, What time is it?",
                "Max, select Control",
                "Max, select Access",
                "Max, select Message",
                "Max, select ligths",
                "Max, select Statistics",
                "Max, select Settings",
                "Max, open the door" }));
            await recognizer.CompileConstraintsAsync();
            while (true)
            {
                var results = await recognizer.RecognizeAsync();
                //if (results?.Confidence != SpeechRecognitionConfidence.Medium) ;
                if (results?.Confidence != SpeechRecognitionConfidence.Rejected)
                {           // TODO: do something with the recognition results. 
                    switch (results.Text)
                    {
                        case "Max, help me":
                            
                            Debug.WriteLine("listening help");
                            twitterSender("Help needed");
                            talker("Emergency Mode Activated");
                            this.client.Publish("/SOS_Mode", Encoding.UTF8.GetBytes("on"));//status SOS_Mode topic published to the broker 
                            break;

                        case "Max, call an ambulance":
                            Debug.WriteLine("listening ambulance");
                            twitterSender("Ambulance");
                            talker("Emergency Mode Activated");
                            this.client.Publish("/SOS_Mode", Encoding.UTF8.GetBytes("on"));//status SOS_Mode topic published to the broker 
                            break;

                        case "Max, call the police":
                            Debug.WriteLine("listening police");
                            twitterSender("Police");
                            talker("Emergency Mode Activated");
                            this.client.Publish("/SOS_Mode", Encoding.UTF8.GetBytes("on"));//status SOS_Mode topic published to the broker 
                            break;

                        //case "Max, select Settings":
                        //    Debug.WriteLine("Max, select Settings");
                        //    talker("Settings Selected");
                        //    Settings.IsSelected = true;
                        //    break;

                        case "Max, What time is it?":
                            Debug.WriteLine("listening what time is it");
                            //localDate
                            talker("Its " + timeNow);
                            break;

                        case "Max, select Control":
                            Debug.WriteLine("Opening Control Frame");
                            talker("Control Selected");
                            Control.IsSelected = true;

                            break;

                        case "Max, select Access":
                            Debug.WriteLine("Opening Access Frame");
                            talker("Access Selected");
                            Access.IsSelected = true;

                            break;

                        case "Max, select Message":
                            Debug.WriteLine("Opening Message Frame");
                            talker("Message Selected");
                            Message.IsSelected = true;

                            break;

                        case "Max, select ligths":
                            Debug.WriteLine("Opening Settings Frame");
                            talker("Lights Selected");
                            Lights.IsSelected = true;

                            break;

                        case "Max, select Statistics":
                            Debug.WriteLine("Opening temperature Frame");
                            talker("Statistics Selected");
                            Temperature.IsSelected = true;

                            break;

                        case "Max, select Settings":
                            Debug.WriteLine("Opening Settings Frame");
                            talker("Settings Selected");
                            Settings.IsSelected = true;

                            break;

                        case "Max, open the door":
                            Debug.WriteLine("listening open the door");
                            talker("opening the door");
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void twitterSender(string sosNotification)
        {
            //twitter credentials
            string consumer_key = "********";
            string consumer_secret = "******************";
            string access_Token = "**************************";
            string access_Token_Secret = "**********************";

            Auth.SetUserCredentials(consumer_key, consumer_secret, access_Token, access_Token_Secret);
            DateTime localDate = DateTime.Now;
            Tweet.PublishTweet("Maximus IoT Notification : " + sosNotification + " at " + localDate + " #MaxIoT" +"by"+ "@redmaurosilva");
            Debug.WriteLine("Maximus IoT Automatic Notification : " + sosNotification + " at "+ localDate +" #MaxIoT");
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
                Control.IsSelected = true;
            }
        }

       

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Control.IsSelected)
            {
                BackButton.Visibility = Visibility.Collapsed;
                MyFrame.Navigate(typeof(Control));
                TitleTextBlock.Text = "Control";
            }
           else if (Help.IsSelected)
            {
                BackButton.Visibility = Visibility.Collapsed;
                MyFrame.Navigate(typeof(Help));
                TitleTextBlock.Text = "S.O.S";
            }
            else if (Access.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(Access));
                TitleTextBlock.Text = "Door";
                Debug.WriteLine("Accessing main Door menu"); 
            }
            else if (Message.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(Message));
                TitleTextBlock.Text = "Message";
            }
            else if (Lights.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(Lights));
                TitleTextBlock.Text = "Lights";
            }
            else if (Temperature.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(heaterPage));
                TitleTextBlock.Text = "Statistics";
            }
            else if (Settings.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(settings));
                TitleTextBlock.Text = "Settings";
            }

        }

        private async void Setup()
        {
            this.hat = await GIS.FEZHAT.CreateAsync();
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(1500);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }


        private void OnTick(object sender, object e)
        {
            illuminance = this.hat.GetLightLevel().ToString("P2");
            currentTemperature = this.hat.GetTemperature().ToString("N2");
            Debug.WriteLine("   Illuminance : "+ illuminance +"   temperature : "+ currentTemperature);// Current data from the fez sensors 
            this.client.Publish("/TempSensor", Encoding.UTF8.GetBytes(currentTemperature));// topic for temperature : /TempSensor
            this.client.Publish("/LightSensor", Encoding.UTF8.GetBytes(illuminance));// topic for illuminance : /LightSensor

            if (SOS_Status== "on")
            {
                ledColor("Red");
            }
            else
            {
                ledColor("Green");
            }
           
            if (this.hat.IsDIO18Pressed())
            {
                this.client.Publish("/SOS_Mode", Encoding.UTF8.GetBytes("off"));//status SOS_Mode topic published to the broker 
                Debug.WriteLine("18 was pressed - reset "); 
            }
            else if (this.hat.IsDIO22Pressed())// emergency mode 
            {
                Debug.WriteLine("22 was pressed - emergency");
                this.client.Publish("/SOS_Mode", Encoding.UTF8.GetBytes("on"));//status SOS_Mode topic published to the broker 
            }

        }

        private void emergencyMode()
        {
            do
            {
                Debug.WriteLine("emergency mode activated");
            }
            while (BackButton.Content == "&#xE814");
                
        }

        private void ledColor(string colorName)
        {
            switch (colorName)
            {
                case "Red":
                    this.hat.D2.Color = GIS.FEZHAT.Color.Red;
                    this.hat.D3.Color = GIS.FEZHAT.Color.Red;
                    var tr = Task.Run(async delegate
                    {
                        await Task.Delay(50);
                        this.hat.D2.Color = GIS.FEZHAT.Color.Black;
                        this.hat.D3.Color = GIS.FEZHAT.Color.Black;
                        return;
                    });
                    tr.Wait();

                    break;

                case "Green":
                    this.hat.D2.Color = GIS.FEZHAT.Color.Green;
                    this.hat.D3.Color = GIS.FEZHAT.Color.Green;
                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(50);
                        this.hat.D2.Color = GIS.FEZHAT.Color.Black;
                        this.hat.D3.Color = GIS.FEZHAT.Color.Black;
                        return;
                    });
                    t.Wait();

                    
                    break;

                
                default:
                    break;
            }
        }
        
    }
}
