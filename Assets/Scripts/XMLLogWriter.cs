using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;
using RTS;


public class XMLLogWriter {
  

	private string fileName;
	private List<NASATasker> gamePlayers = new List<NASATasker>();


	private static XMLLogWriter instance;
	public static XMLLogWriter Instance {
		get {
			if (instance == null) {
				instance = new XMLLogWriter ();
			}

			return instance;
		}
	}

    public void setFileName(string inFileName)
    {
        fileName = inFileName;
        if (File.Exists(this.filePath()))
        {
            //File.Delete(this.filePath());
        }

        gamePlayers.Clear();
    }

    public string filePath()
    {
        string filepath = fileName;//Application.dataPath +
        return filepath;
    }

	public void log(NASATasker player)
    {
        gamePlayers.Add(player);
        writeXml();
    }

    private void writeXml()
    {

        if (File.Exists(this.filePath()))
        {
            File.Delete(this.filePath());
        }

        XmlTextWriter textWriter = new XmlTextWriter(this.filePath(), null);
        // Opens the document
        textWriter.WriteStartDocument();
        textWriter.WriteComment("This document contains the player details that have been created.");
        textWriter.WriteStartElement("NASATaskLoadindex");
        textWriter.WriteWhitespace("\n");


		foreach (NASATasker player in gamePlayers)
        {

            textWriter.WriteStartElement("NASATaskIndex");
            textWriter.WriteElementString("Name", player.name);
            textWriter.WriteElementString("Task", player.task);
			textWriter.WriteElementString("Date", player.date);
            textWriter.WriteElementString("MentalDemand", player.mentalDemandValue.ToString());
			textWriter.WriteElementString("PhysicalDemand", player.physicalDemandValue.ToString());
			textWriter.WriteElementString("TemporaralDemand", player.temporalDemandValue.ToString());
			textWriter.WriteElementString("Performance", player.performanceDemandValue.ToString());
			textWriter.WriteElementString("Effort", player.effortDemandValue.ToString());
			textWriter.WriteElementString("Frustration", player.fustationDemandValue.ToString());

            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");
        }



        textWriter.WriteEndElement();
        textWriter.WriteEndDocument();

       

        textWriter.Close();


    }



}
