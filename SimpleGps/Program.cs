using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoMini;

using System.IO;
using System.IO.Ports;

using uk.andyjohnson0.Netduino.Drivers.Display;
using uk.andyjohnson0.Netduino.Drivers.Gps;

using uk.andyjohnson0.Netduino.Utils;



// TODO:
// 1. Single-line display option
// 2. Unit tests
// 3. Display accuracy
// 4. Display signal strength indicator



namespace SimpleGps
{
    public class Program
    {
        public static void Main()
        {
            Program prog = new Program();
            prog.Start();
        }



        private enum DisplayMode
        {
            Osgb,
            LatLong
        }


        private Lcd lcd;
        private DisplayMode displayMode;
        private bool clearDisplay = true;
        private InterruptPort modeButton;
        private NmeaDecoder messageDecoder;
        private NmeaMonitor messageMonitor;
        private Thread gpsThread;
        private TimeZone timeZone;

        private const int uiTimerUpdateFreqMs = 1000 * 10;


        public void Start()
        {
            //Debug.EnableGCMessages(true);
            timeZone = TimeZoneFactory.Create(TimeZoneId.London);

            // Initialise display.
            displayMode = DisplayMode.Osgb;
            lcd = new Lcd(Lcd.Device.HD44780, Pins.GPIO_PIN_13, Pins.GPIO_PIN_14,
                          Pins.GPIO_PIN_15, Pins.GPIO_PIN_16, Pins.GPIO_PIN_17, Pins.GPIO_PIN_18);
            lcd.Reset();
            lcd.SetDisplay(true, false, false);
            lcd.SetFunction(Lcd.LineMode.TwoLine, Lcd.Font.FiveByTen);
            lcd.Clear();
            lcd.SetCursorAddress(0x00);
            lcd.Write("Simple GPS");
            lcd.SetCursorAddress(0x40);
            lcd.Write("Scanning...");
            modeButton = new InterruptPort(Pins.GPIO_PIN_19, true, Port.ResistorMode.Disabled,
                                           Port.InterruptMode.InterruptEdgeHigh);
            modeButton.OnInterrupt += new NativeEventHandler(modeButton_OnInterrupt);

            // Start GPS thread.
            messageDecoder = new NmeaDecoder();
            messageMonitor = new NmeaMonitor();
            gpsThread = new Thread(new ThreadStart(GpsThreadProc));
            gpsThread.Start();

            // Start UI update timer. Fire every ten seconds.
            Timer uiTimer = new Timer(new TimerCallback(UiUpdateProc), null, 0, uiTimerUpdateFreqMs);

            Thread.Sleep(Timeout.Infinite);
        }


        private LatLonCoord lastCoord = null;
        private OSGBGridRef lastGridRef = null;


        private void UiUpdateProc(object obj)
        {
            Debug.Print("UiUpdateProc()");
            UpdateUi();
        }


        private void UpdateUi()
        {
            Debug.Print("UpdateUi()");

            LatLonCoord coord = null;
            DateTime time = DateTime.MinValue;
            lock (messageMonitor)
            {
                coord = messageMonitor.LastFixCoord;
                time = messageMonitor.CurrentGpsTime;
            }

            if ((coord == null) || (time == DateTime.MinValue))
            {
                return;
            }

            lock (lcd)
            {
                if (clearDisplay)
                {
                    lcd.Clear();
                    clearDisplay = false;
                }

                switch (displayMode)
                {
                    case DisplayMode.Osgb:
                        if ((!coord.IsEqualTo(lastCoord)) || (lastGridRef == null))
                        {
                            // Fix has changed. Recalculate grid ref.
                            // TODO: Still sometimes get unnecessary recalcs here.
                            Debug.Print("Recalc: " +
                                        (lastCoord != null ? lastCoord.ToString() : "null") +
                                        "=>" +
                                        (coord != null ? coord.ToString() : "null"));
                            lastCoord = coord;
                            LatLonCoord coord2 = LatLonCoord.Convert(coord, CoordSystem.OSGB36);
                            lastGridRef = new OSGBGridRef(coord2);
                        }

                        lcd.SetCursorAddress(0x00);
//                        lcd.Write(time.ToString("HH:mm dd/MM/yy"));
                        lcd.Write(timeZone.ToLocalTime(time).ToString("HH:mm dd/MM/yy"));
                        lcd.SetCursorAddress(0x40);
                        lcd.Write(lastGridRef.ToString());
                        break;
                    case DisplayMode.LatLong:
                        lcd.SetCursorAddress(0x00);
                        lcd.Write("Lat " + (coord.LatitudeDegrees >= 0 ? "+" : "") + coord.LatitudeDegrees.ToString() + " " + coord.LatitudeMinutes.ToString("N3") + "'");
                        lcd.SetCursorAddress(0x40);
                        lcd.Write("Lng " + (coord.LongitudeDegrees >= 0 ? "+" : "") + coord.LongitudeDegrees.ToString() + " " + coord.LongitudeMinutes.ToString("N3") + "'");
                        break;
                }
            }
        }


        private void GpsThreadProc()
        {
            Debug.Print("GpsThreadProc()");
            // Create serial connection to GPS hardware. Use 4800 for EM406a and 9600 for Sparkfun breakout board.
            GenericSerialGps gps = new GenericSerialGps(SerialPorts.COM1, 4800);
            gps.MessageReceived += new GenericSerialGps.MessageReceivedHandler(gps_MessageReceived);
            gps.Start();

            Thread.Sleep(Timeout.Infinite);
        }



        // +53° 25.5117', -02° 13.9296'
        private void gps_MessageReceived(object sender, GenericSerialGps.MessageReceivedEventArgs e)
        {
            NmeaMessage msg = messageDecoder.Decode(e.Message);
            lock (messageDecoder)
            {
                messageMonitor.OnMessage(msg);
            }
            Thread.Sleep(100);
        }


        private void modeButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            modeButton.DisableInterrupt();
            // TODO: Race condition here. Use locking.
            Debug.Print("modeButton_OnInterrupt()");
            displayMode = (displayMode == DisplayMode.Osgb) ? DisplayMode.LatLong : DisplayMode.Osgb;
            clearDisplay = true;
            UpdateUi();
            modeButton.EnableInterrupt();
        }
    }
}
