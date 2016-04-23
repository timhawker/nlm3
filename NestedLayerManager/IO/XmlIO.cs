using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Autodesk.Max;
using MaxCustomControls;

using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.IO
{
    public class XmlIO
    {
        private MaxForm MaxForm;

        private readonly String XmlPath = "C:/ProgramData/Nested Layer Manager 3/NestedLayerManager.xml";

        public XmlIO(MaxForm maxForm)
        {
            MaxForm = maxForm;
            maxForm.FormClosing += new FormClosingEventHandler(Save);
            Load();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(XmlPath))
                {
                    XDocument xml = XDocument.Load(XmlPath);
                    XElement root = xml.Root;

                    XElement size = root.Element("Size");
                    if (size != null)
                    {
                        XElement width = size.Element("Width");
                        XElement height = size.Element("Height");
                        if (width != null && height != null)
                        {
                            MaxForm.Size = new Size(Convert.ToInt32(width.Value), Convert.ToInt32(height.Value));
                        }
                    }
                    XElement location = root.Element("Location");
                    if (location != null)
                    {
                        XElement x = location.Element("X");
                        XElement y = location.Element("Y");
                        if (x != null && y != null)
                        {
                            // Setting location does nothing before the form is shown.
                            // And setting location after the form is shown looks like crap.
                            // Thankfully, setting these two properties works :)
                            MaxForm.StartPosition = FormStartPosition.Manual;
                            MaxForm.Left = Convert.ToInt32(x.Value);
                            MaxForm.Top = Convert.ToInt32(y.Value);
                        }
                    }
                }
            }
            catch
            {
                MaxListener.PrintToListener("ERROR: Unable to read XML file");
            }
        }

        public void Save(Object sender, FormClosingEventArgs e)
        {
            try
            {
                // Ensure directory exists.
                Directory.CreateDirectory(Path.GetDirectoryName(XmlPath));
                // Write XML file.
                new XDocument(
                    new XElement("NestedLayerManager",
                        new XElement("Size",
                            new XElement("Height", MaxForm.Height),
                            new XElement("Width", MaxForm.Width)
                        ),
                        new XElement("Location",
                            new XElement("X", MaxForm.Location.X),
                            new XElement("Y", MaxForm.Location.Y)
                        )
                    )
                )
                .Save(XmlPath);
            }
            catch
            {
                MaxListener.PrintToListener("ERROR: Unable to save XML file");
            }
        }
    }
}
