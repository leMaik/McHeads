/*
So you thought WPF and bindings were voodoo? - Nope, THIS is voodoo!
                           ,_/\_,',/\
                           \_ _ _ _ /
                           /        ``\
                         /`            `\
                        /  ,-,      ,-,  `,
                        | ( x )    ( x )  |
                        |  `-'      `-'   |   O
                        \       ==        /  /
                         \   -_ _ _ -    /  /
                       O  \_           _'  /
                        \  _>- - - - -<_  /
                        ,\`             `/,
                      ,'  \            ,/  `.
                    ,'     \           `^    `.
                  ,`        \,                 `.
                 ,          ^`                   `,
               ,'        ,               |\        ',
              ,         /|               \ `.        )
             /        ,' |                |  `',    ,`>
             |      ,'   /                \     ``-r_\`
           ,-\   ,'`    |                  |       `
          `-L /``       |                  |   -ZEUS-
             V          |                  |
*/

using System; 
using System.Text;
using System.Xml; 

namespace CornerHead {
    ///<summary>
    ///Einstellungen für CornerHead.
    ///Generiert am 22.02.2014 um 23:28 Uhr.
    ///</summary>
    public static class Settings {
        #region Private Variablen
        private static string _playername = "Dinnerbone";
        private static int _size = 200;
        private static bool _goaway = true;
        private static int _position = 0;
        private static double _opacity = 1;
        #endregion

        #region Öffentliche Felder
        public static string playername {
            get { return _playername; }
            set {
                _playername = value;
            }
        }

        public static int size {
            get { return _size; }
            set {
                _size = value;
            }
        }

        public static bool goaway {
            get { return _goaway; }
            set {
                _goaway = value;
            }
        }

        ///<summary>
        ///Die Positon des Kopfes. Erlaubt sind Werte von 0 bis 3, mit Bedeutung: oben links, oben rechts, unten links, unten rechts.
        ///</summary>
        public static int position {
            get { return _position; }
            set {
                _position = value;
            }
        }

        public static double opacity {
            get { return _opacity; }
            set {
                _opacity = value;
            }
        }

        #endregion

        ///<summary>
        ///Lädt die Einstellungen aus der angegebenen XML-Datei. Sollte dabei ein Fehler auftreten, wird die Datei mit den aktuellen Einstellungen überschrieben.
        ///</summary>
        ///<param name="filename">Dateiname</param>
        ///<param name="settingsVersion">Version der Einstellungen (bei neuerer Version werden vorhandene Einstellungen ignoriert, falls die neuen erzwungen werden)</param>
        public static void Load(String filename, String settingsVersion) {
            XmlNode temp;
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            String xmlVersion= doc.SelectSingleNode("/settings").Attributes["version"].Value;

            try {
                temp = doc.SelectSingleNode("//option[@name='playername']");
                if (temp != null)
                    playername = temp.InnerText;
                temp = doc.SelectSingleNode("//option[@name='size']");
                if (temp != null)
                    size = int.Parse(temp.InnerText);
                temp = doc.SelectSingleNode("//option[@name='goaway']");
                if (temp != null)
                    goaway = bool.Parse(temp.InnerText);
                temp = doc.SelectSingleNode("//option[@name='position']");
                if (temp != null)
                    position = int.Parse(temp.InnerText);
                temp = doc.SelectSingleNode("//option[@name='opacity']");
                if (temp != null)
                    opacity = double.Parse(temp.InnerText);
            } 
            catch {
                Save(filename, settingsVersion);
            }
        }

        ///<summary>
        ///Speichert die Einstellungen in die angegebene XML-Datei.
        ///</summary>
        ///<param name="filename">Dateiname</param>
        ///<param name="settingsVersion">Version der Einstellungen</param>
        public static void Save(String filename, String settingsVersion) {
            XmlTextWriter xml = new XmlTextWriter(filename, Encoding.UTF8);
            xml.Formatting = Formatting.Indented;
            xml.WriteStartDocument();
            xml.WriteStartElement("settings");
            xml.WriteAttributeString("version", settingsVersion);

            xml.WriteStartElement("option");
            xml.WriteAttributeString("name", "playername");
			if(playername != null) {
				xml.WriteCData(playername.ToString());
			} else {
				xml.WriteCData(String.Empty);
			}
            xml.WriteEndElement();

            xml.WriteStartElement("option");
            xml.WriteAttributeString("name", "size");
			xml.WriteCData(size.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("option");
            xml.WriteAttributeString("name", "goaway");
			xml.WriteCData(goaway.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("option");
            xml.WriteAttributeString("name", "position");
			xml.WriteCData(position.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("option");
            xml.WriteAttributeString("name", "opacity");
			xml.WriteCData(opacity.ToString());
            xml.WriteEndElement();

            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Close();
        }
    }
}

